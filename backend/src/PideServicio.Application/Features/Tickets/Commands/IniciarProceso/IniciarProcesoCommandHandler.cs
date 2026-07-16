namespace PideServicio.Application.Features.Tickets.Commands.IniciarProceso;

using Microsoft.Extensions.Logging;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class IniciarProcesoCommandHandler : ICommandHandler<IniciarProcesoCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly ILogger<IniciarProcesoCommandHandler> _logger;

    public IniciarProcesoCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        INotificationService notificationService,
        IEmailService emailService,
        IAuditService auditService,
        ILogger<IniciarProcesoCommandHandler> logger)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _notificationService = notificationService;
        _emailService = emailService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result> Handle(IniciarProcesoCommand request, CancellationToken cancellationToken)
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

        var esTecnicoAsignado = ticket.TecnicoId == actor.Id;
        var tieneAccesoAdministrativo = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN or RolTipo.SUPERVISOR;
        if (!esTecnicoAsignado && !tieneAccesoAdministrativo)
            return Result.NoPermitido("Solo el técnico asignado o un administrador puede iniciar el proceso.");

        var estadoAnterior = ticket.Estado;

        try
        {
            ticket.IniciarProceso(actor.Id);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.ESTADO_CAMBIADO,
                actorId: actor.Id,
                estadoAnterior: estadoAnterior,
                estadoNuevo: TicketEstadoTipo.EN_PROCESO);

            await _historialRepo.CrearAsync(historial, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "INICIAR_PROCESO",
                new { Estado = estadoAnterior.ToString() },
                new { Estado = ticket.Estado.ToString() },
                cancellationToken);

            // Notificaciones push — fire-and-forget con try-catch explícito
            var notifSolicitanteId = ticket.SolicitanteId;
            var notifTicketId = ticket.Id;
            var notifEmpresaId = ticket.EmpresaId;
            var notifCodigo = ticket.Codigo.Valor;
            var notifTitulo = ticket.Titulo;

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.EnviarAsync(
                        notifSolicitanteId,
                        "Ticket en proceso",
                        $"El técnico ha iniciado la atención del ticket {notifCodigo}: {notifTitulo}",
                        tipoEvento: "ticket.en_proceso",
                        ticketId: notifTicketId,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en fire-and-forget EnviarAsync (solicitante) para ticket {Codigo}", notifCodigo);
                }
            });

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.EnviarAGestoresYSuperAdminsAsync(
                        notifEmpresaId,
                        "Ticket en proceso",
                        $"El técnico inició la atención del ticket {notifCodigo}: {notifTitulo}",
                        tipoEvento: "ticket.en_proceso",
                        ticketId: notifTicketId,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en fire-and-forget EnviarAGestoresYSuperAdminsAsync para ticket {Codigo}", notifCodigo);
                }
            });

            // Email al solicitante — fire-and-forget con try-catch explícito
            var solicitante = await _usuarioRepository.ObtenerPorIdAsync(ticket.SolicitanteId, cancellationToken);
            if (solicitante is not null)
            {
                var correoSolicitante = solicitante.Correo.Valor;
                var codigoTicket = ticket.Codigo.Valor;
                var tituloTicket = ticket.Titulo;
                var tecnicoNombre = actor.NombreCompleto;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.NotificarTicketEnProcesoAsync(
                            correoSolicitante: correoSolicitante,
                            codigo: codigoTicket,
                            titulo: tituloTicket,
                            tecnico: tecnicoNombre,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget NotificarTicketEnProcesoAsync para ticket {Codigo}", codigoTicket);
                    }
                });
            }

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
