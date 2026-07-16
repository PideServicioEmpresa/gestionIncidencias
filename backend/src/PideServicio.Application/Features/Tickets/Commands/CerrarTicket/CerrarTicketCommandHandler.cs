namespace PideServicio.Application.Features.Tickets.Commands.CerrarTicket;

using Microsoft.Extensions.Logging;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CerrarTicketCommandHandler : ICommandHandler<CerrarTicketCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly ILogger<CerrarTicketCommandHandler> _logger;

    public CerrarTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        INotificationService notificationService,
        IEmailService emailService,
        IAuditService auditService,
        ILogger<CerrarTicketCommandHandler> logger)
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

    public async Task<Result> Handle(CerrarTicketCommand request, CancellationToken cancellationToken)
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
            return Result.NoPermitido("Solo el solicitante o un administrador puede cerrar el ticket.");

        var estadoAnterior = ticket.Estado;

        try
        {
            ticket.Cerrar(actor.Id, request.Valoracion);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.ESTADO_CAMBIADO,
                actorId: actor.Id,
                estadoAnterior: estadoAnterior,
                estadoNuevo: TicketEstadoTipo.CERRADO);

            await _historialRepo.CrearAsync(historial, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "CERRAR",
                new { Estado = estadoAnterior.ToString() },
                new { Estado = ticket.Estado.ToString(), ticket.Valoracion },
                cancellationToken);

            // Notificaciones push — fire-and-forget con try-catch explícito
            var notifTicketId = ticket.Id;
            var notifEmpresaId = ticket.EmpresaId;
            var notifCodigo = ticket.Codigo.Valor;
            var notifTitulo = ticket.Titulo;

            if (ticket.TecnicoId.HasValue)
            {
                var notifTecnicoId = ticket.TecnicoId.Value;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.EnviarAsync(
                            notifTecnicoId,
                            "Ticket cerrado",
                            $"El ticket {notifCodigo} ha sido cerrado por el solicitante.",
                            tipoEvento: "ticket.cerrado",
                            ticketId: notifTicketId,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget EnviarAsync (tecnico) para ticket {Codigo}", notifCodigo);
                    }
                });
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.EnviarAGestoresYSuperAdminsAsync(
                        notifEmpresaId,
                        "Ticket cerrado",
                        $"El ticket {notifCodigo} fue cerrado: {notifTitulo}",
                        tipoEvento: "ticket.cerrado",
                        ticketId: notifTicketId,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en fire-and-forget EnviarAGestoresYSuperAdminsAsync para ticket {Codigo}", notifCodigo);
                }
            });

            var codigoTicket = ticket.Codigo.Valor;
            var tituloTicket = ticket.Titulo;
            var valoracion = ticket.Valoracion?.ToString();

            // Email al solicitante — fire-and-forget con try-catch explícito
            var correoSolicitante = esSolicitante
                ? actor.Correo.Valor
                : (await _usuarioRepository.ObtenerPorIdAsync(ticket.SolicitanteId, cancellationToken))?.Correo.Valor;

            if (!string.IsNullOrWhiteSpace(correoSolicitante))
            {
                var correoSolicitanteCaptura = correoSolicitante;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.NotificarTicketCerradoAsync(
                            correoSolicitante: correoSolicitanteCaptura,
                            codigo: codigoTicket,
                            titulo: tituloTicket,
                            valoracion: valoracion,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget NotificarTicketCerradoAsync para ticket {Codigo}", codigoTicket);
                    }
                });
            }

            // Email al técnico — fire-and-forget con try-catch explícito
            if (ticket.TecnicoId.HasValue)
            {
                var tecnico = await _usuarioRepository.ObtenerPorIdAsync(ticket.TecnicoId.Value, cancellationToken);
                if (tecnico is not null)
                {
                    var correoTecnico = tecnico.Correo.Valor;
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.NotificarTicketCerradoTecnicoAsync(
                                correoTecnico: correoTecnico,
                                codigo: codigoTicket,
                                titulo: tituloTicket,
                                valoracion: valoracion,
                                cancellationToken: CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error en fire-and-forget NotificarTicketCerradoTecnicoAsync para ticket {Codigo}", codigoTicket);
                        }
                    });
                }
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
