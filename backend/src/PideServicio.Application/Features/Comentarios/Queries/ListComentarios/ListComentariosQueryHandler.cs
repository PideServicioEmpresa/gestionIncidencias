namespace PideServicio.Application.Features.Comentarios.Queries.ListComentarios;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Comentarios.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListComentariosQueryHandler
    : IQueryHandler<ListComentariosQuery, ListResult<ComentarioDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketComentarioRepository _comentarioRepository;

    public ListComentariosQueryHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepository,
        ITicketComentarioRepository comentarioRepository)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepository = ticketRepository;
        _comentarioRepository = comentarioRepository;
    }

    public async Task<Result<ListResult<ComentarioDto>>> Handle(
        ListComentariosQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<ListResult<ComentarioDto>>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado<ListResult<ComentarioDto>>();

        var ticket = await _ticketRepository.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado<ListResult<ComentarioDto>>("Ticket", request.TicketId);

        if (actor.Rol != RolTipo.SUPERADMIN && ticket.EmpresaId != actor.EmpresaId)
            return Result.NoPermitido<ListResult<ComentarioDto>>("No tiene acceso a este ticket.");

        var tieneAccesoAdministrativo = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN or RolTipo.SUPERVISOR;
        bool incluirInternos = tieneAccesoAdministrativo || ticket.TecnicoId == actor.Id;

        var comentarios = await _comentarioRepository.ListarPorTicketAsync(
            request.TicketId,
            incluirInternos,
            cancellationToken);

        // Resolver nombres de autores en paralelo (IDs únicos)
        var uniqueIds = comentarios.Select(c => c.AutorId).Distinct().ToList();
        var autoresTasks = uniqueIds
            .Select(id => _usuarioRepository.ObtenerPorIdAsync(id, cancellationToken))
            .ToList();
        var autores = await Task.WhenAll(autoresTasks);

        var autorDict = uniqueIds
            .Zip(autores)
            .Where(x => x.Second is not null)
            .ToDictionary(x => x.First, x => $"{x.Second!.Nombre} {x.Second!.Apellido}");

        var dtos = comentarios.Select(c =>
        {
            autorDict.TryGetValue(c.AutorId, out var nombre);
            return c.Adapt<ComentarioDto>() with { AutorNombre = nombre };
        }).ToList();

        return Result.Exito(ListResult<ComentarioDto>.Crear(dtos));
    }
}
