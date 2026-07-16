namespace PideServicio.Application.Features.Tickets.Commands.ReabrirTicket;

using Microsoft.Extensions.Logging;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ReabrirTicketCommandHandler : ICommandHandler<ReabrirTicketCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly IMotivoRechazoRepository _motivoRechazoRepo;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly ILogger<ReabrirTicketCommandHandler> _logger;

    public ReabrirTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        IMotivoRechazoRepository motivoRechazoRepo,
        INotificationService notificationService,
        IEmailService emailService,
        IAuditService auditService,
        ILogger<ReabrirTicketCommandHandler> logger)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _motivoRechazoRepo = motivoRechazoRepo;
        _notificationService = notificationService;
        _emailService = emailService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result> Handle(ReabrirTicketCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado("Ticket", request.TicketId);

        var esSolicitante = ticket.SolicitanteId == actor.Id;
        var tieneAccesoAdministrativo = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN or RolTipo.SUPERVISOR;
        if (!esSolicitante && !tieneAccesoAdministrativo)
            return Result.NoPermitido("Solo el solicitante o un administrador puede reabrir el ticket.");

        var existeMotivo = await _motivoRechazoRepo.ExisteAsync(request.MotivoRechazoId, cancellationToken);
        if (!existeMotivo)
            return Result.NoEncontrado("MotivoRechazo", request.MotivoRechazoId);

        var esOtro = await _motivoRechazoRepo.EsOtroAsync(request.MotivoRechazoId, cancellationToken);
        if (esOtro && string.IsNullOrWhiteSpace(request.ComentarioRechazo))
            return Result.ErrorValidacion("ComentarioRechazo", "El comentario es obligatorio cuando el motivo es 'Otro'.");

        var tecnicoAnteriorId = ticket.TecnicoId;
        var estadoAnterior = ticket.Estado;

        try
        {
            ticket.Reabrir(actor.Id, request.MotivoRechazoId, request.ComentarioRechazo);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.ESTADO_CAMBIADO,
                actorId: actor.Id,
                estadoAnterior: estadoAnterior,
                estadoNuevo: TicketEstadoTipo.REABIERTO,
                motivoRechazoId: request.MotivoRechazoId,
                comentarioRechazo: request.ComentarioRechazo);

            await _historialRepo.CrearAsync(historial, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "REABRIR",
                new { Estado = estadoAnterior.ToString() },
                new { Estado = ticket.Estado.ToString(), request.MotivoRechazoId, request.ComentarioRechazo },
                cancellationToken);

            // Notificaciones push — fire-and-forget con try-catch explícito
            var notifTicketId = ticket.Id;
            var notifEmpresaId = ticket.EmpresaId;
            var notifCodigo = ticket.Codigo.Valor;
            var notifTitulo = ticket.Titulo;

            if (tecnicoAnteriorId.HasValue)
            {
                var notifTecnicoAnteriorId = tecnicoAnteriorId.Value;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.EnviarAsync(
                            notifTecnicoAnteriorId,
                            "Ticket rechazado",
                            $"El ticket {notifCodigo} ha sido rechazado y está pendiente de reasignación.",
                            tipoEvento: "ticket.rechazado",
                            ticketId: notifTicketId,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget EnviarAsync (tecnico anterior) para ticket {Codigo}", notifCodigo);
                    }
                });

                // Email al técnico anterior — fire-and-forget con try-catch explícito
                var tecnico = await _usuarioRepository.ObtenerPorIdAsync(tecnicoAnteriorId.Value, cancellationToken);
                if (tecnico is not null)
                {
                    var correoTecnico = tecnico.Correo.Valor;
                    var codigoTicket = ticket.Codigo.Valor;
                    var tituloTicket = ticket.Titulo;
                    var motivo = !string.IsNullOrWhiteSpace(request.ComentarioRechazo)
                        ? request.ComentarioRechazo
                        : "El solicitante rechazó la atención";

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.NotificarTicketReabiertoAsync(
                                correoTecnico: correoTecnico,
                                codigo: codigoTicket,
                                titulo: tituloTicket,
                                motivo: motivo,
                                cancellationToken: CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error en fire-and-forget NotificarTicketReabiertoAsync para ticket {Codigo}", codigoTicket);
                        }
                    });
                }
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.EnviarAGestoresYSuperAdminsAsync(
                        notifEmpresaId,
                        "Ticket reabierto",
                        $"El ticket {notifCodigo} ha sido rechazado y requiere reasignación.",
                        tipoEvento: "ticket.rechazado",
                        ticketId: notifTicketId,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en fire-and-forget EnviarAGestoresYSuperAdminsAsync para ticket {Codigo}", notifCodigo);
                }
            });

            return Result.Exito();
        }
        catch (TransicionEstadoInvalidaException ex)
        {
            return Result.Fallo(ex.Message);
        }
        catch (TicketCerradoException ex)
        {
            return Result.Fallo(ex.Message);
        }
        catch (TicketCanceladoException ex)
        {
            return Result.Fallo(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
