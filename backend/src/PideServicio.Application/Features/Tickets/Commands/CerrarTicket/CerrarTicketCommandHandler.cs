namespace PideServicio.Application.Features.Tickets.Commands.CerrarTicket;

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
    private readonly IAuditService _auditService;

    public CerrarTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        INotificationService notificationService,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _notificationService = notificationService;
        _auditService = auditService;
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

            if (ticket.TecnicoId.HasValue)
            {
                await _notificationService.EnviarAsync(
                    ticket.TecnicoId.Value,
                    "Ticket cerrado",
                    $"El ticket {ticket.Codigo.Valor} ha sido cerrado por el solicitante.",
                    tipoEvento: "ticket.cerrado",
                    datos: new Dictionary<string, string>
                    {
                        ["ticketId"] = ticket.Id.ToString(),
                        ["codigo"] = ticket.Codigo.Valor
                    },
                    cancellationToken: cancellationToken);
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
