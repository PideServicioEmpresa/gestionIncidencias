namespace PideServicio.Application.Features.Tickets.Commands.CambiarPrioridad;

using Microsoft.Extensions.Logging;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CambiarPrioridadCommandHandler : ICommandHandler<CambiarPrioridadCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly ILogger<CambiarPrioridadCommandHandler> _logger;

    public CambiarPrioridadCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        INotificationService notificationService,
        IEmailService emailService,
        IAuditService auditService,
        ILogger<CambiarPrioridadCommandHandler> logger)
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

    public async Task<Result> Handle(CambiarPrioridadCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        if (actor.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido("Solo administradores pueden cambiar la prioridad de un ticket.");

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado("Ticket", request.TicketId);

        var prioridadAnterior = ticket.PrioridadEfectiva;

        try
        {
            ticket.CambiarPrioridad(request.NuevaPrioridad, actor.Id);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.PRIORIDAD_CAMBIADA,
                actorId: actor.Id,
                metadata: $"{{\"anterior\":\"{prioridadAnterior}\",\"nueva\":\"{ticket.PrioridadEfectiva}\"}}");

            await _historialRepo.CrearAsync(historial, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "CAMBIAR_PRIORIDAD",
                new { Prioridad = prioridadAnterior.ToString() },
                new { Prioridad = ticket.PrioridadEfectiva.ToString() },
                cancellationToken);

            // Notificación push al técnico asignado — fire-and-forget con try-catch explícito
            if (ticket.TecnicoId.HasValue)
            {
                var notifTecnicoId = ticket.TecnicoId.Value;
                var notifTicketId = ticket.Id;
                var notifCodigo = ticket.Codigo.Valor;
                var notifTitulo = ticket.Titulo;
                var notifPrioridadAnterior = prioridadAnterior;
                var notifPrioridadNueva = ticket.PrioridadEfectiva;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.EnviarAsync(
                            notifTecnicoId,
                            "Prioridad modificada",
                            $"La prioridad del ticket {notifCodigo} cambió de {notifPrioridadAnterior} a {notifPrioridadNueva}: {notifTitulo}",
                            tipoEvento: "ticket.prioridad_cambiada",
                            ticketId: notifTicketId,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget EnviarAsync (tecnico) para ticket {Codigo}", notifCodigo);
                    }
                });

                // Email al técnico — fire-and-forget con try-catch explícito
                var tecnico = await _usuarioRepository.ObtenerPorIdAsync(ticket.TecnicoId.Value, cancellationToken);
                if (tecnico is not null)
                {
                    var correoTecnico = tecnico.Correo.Valor;
                    var codigoTicket = ticket.Codigo.Valor;
                    var tituloTicket = ticket.Titulo;
                    var prioridadAnteriorStr = prioridadAnterior.ToString();
                    var prioridadNuevaStr = ticket.PrioridadEfectiva.ToString();

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.NotificarCambioPrioridadTecnicoAsync(
                                correoTecnico: correoTecnico,
                                codigo: codigoTicket,
                                titulo: tituloTicket,
                                prioridadAnterior: prioridadAnteriorStr,
                                prioridadNueva: prioridadNuevaStr,
                                cancellationToken: CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error en fire-and-forget NotificarCambioPrioridadTecnicoAsync para ticket {Codigo}", codigoTicket);
                        }
                    });
                }
            }

            return Result.Exito();
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
