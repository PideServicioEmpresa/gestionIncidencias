namespace PideServicio.Application.Features.Tickets.Commands.CambiarPrioridad;

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
    private readonly IAuditService _auditService;

    public CambiarPrioridadCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _auditService = auditService;
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
