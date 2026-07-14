namespace PideServicio.Application.Features.Tickets.Queries.GetTicketHistorial;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Tickets.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetTicketHistorialQueryHandler : IQueryHandler<GetTicketHistorialQuery, ListResult<TicketHistorialDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;

    public GetTicketHistorialQueryHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
    }

    public async Task<Result<ListResult<TicketHistorialDto>>> Handle(
        GetTicketHistorialQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<ListResult<TicketHistorialDto>>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado<ListResult<TicketHistorialDto>>();

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado<ListResult<TicketHistorialDto>>("Ticket", request.TicketId);

        if (actor.Rol == RolTipo.SUPERADMIN)
        {
            // SuperAdmin ve el historial de cualquier ticket
        }
        else if (actor.Rol is RolTipo.ADMIN or RolTipo.SUPERVISOR)
        {
            if (ticket.EmpresaId != actor.EmpresaId)
                return Result.NoPermitido<ListResult<TicketHistorialDto>>("No tiene acceso a este ticket.");
        }
        else if (actor.Rol == RolTipo.TECNICO)
        {
            if (ticket.TecnicoId != actor.Id)
                return Result.NoPermitido<ListResult<TicketHistorialDto>>("Solo puede ver el historial de tickets asignados a usted.");
        }
        else
        {
            if (ticket.SolicitanteId != actor.Id)
                return Result.NoPermitido<ListResult<TicketHistorialDto>>("Solo puede ver el historial de tickets creados por usted.");
        }

        var entradas = await _historialRepo.ListarPorTicketAsync(request.TicketId, cancellationToken);

        // Resolver nombres de actores en paralelo (IDs únicos, excluyendo null)
        var uniqueIds = entradas
            .Where(e => e.ActorId.HasValue)
            .Select(e => e.ActorId!.Value)
            .Distinct()
            .ToList();

        var actoresTasks = uniqueIds
            .Select(id => _usuarioRepository.ObtenerPorIdAsync(id, cancellationToken))
            .ToList();
        var actores = await Task.WhenAll(actoresTasks);

        var actorDict = uniqueIds
            .Zip(actores)
            .Where(x => x.Second is not null)
            .ToDictionary(x => x.First, x => $"{x.Second!.Nombre} {x.Second!.Apellido}");

        var dtos = entradas.Select(e =>
        {
            string? nombre = null;
            if (e.ActorId.HasValue) actorDict.TryGetValue(e.ActorId.Value, out nombre);
            return e.Adapt<TicketHistorialDto>() with { ActorNombre = nombre };
        }).ToList();

        return Result.Exito(ListResult<TicketHistorialDto>.Crear(dtos));
    }
}
