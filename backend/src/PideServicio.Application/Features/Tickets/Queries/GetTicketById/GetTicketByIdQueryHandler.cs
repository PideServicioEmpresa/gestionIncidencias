namespace PideServicio.Application.Features.Tickets.Queries.GetTicketById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Tickets.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetTicketByIdQueryHandler : IQueryHandler<GetTicketByIdQuery, TicketDetalleDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IAreaRepository _areaRepo;
    private readonly ITipoServicioRepository _tipoServicioRepo;

    public GetTicketByIdQueryHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ISucursalRepository sucursalRepo,
        IAreaRepository areaRepo,
        ITipoServicioRepository tipoServicioRepo)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _sucursalRepo = sucursalRepo;
        _areaRepo = areaRepo;
        _tipoServicioRepo = tipoServicioRepo;
    }

    public async Task<Result<TicketDetalleDto>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<TicketDetalleDto>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado<TicketDetalleDto>();

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.Id, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado<TicketDetalleDto>("Ticket", request.Id);

        if (actor.Rol == RolTipo.SUPERADMIN)
        {
            // SuperAdmin ve cualquier ticket
        }
        else if (actor.Rol is RolTipo.ADMIN or RolTipo.SUPERVISOR)
        {
            if (ticket.EmpresaId != actor.EmpresaId)
                return Result.NoPermitido<TicketDetalleDto>("No tiene acceso a este ticket.");
        }
        else if (actor.Rol == RolTipo.TECNICO)
        {
            if (ticket.TecnicoId != actor.Id)
                return Result.NoPermitido<TicketDetalleDto>("Solo puede ver los tickets asignados a usted.");
        }
        else if (actor.Rol == RolTipo.TRABAJADOR)
        {
            // Trabajador puede ver tickets que creó o que tiene asignados
            if (ticket.SolicitanteId != actor.Id && ticket.TecnicoId != actor.Id)
                return Result.NoPermitido<TicketDetalleDto>("No tiene acceso a este ticket.");
        }
        else
        {
            // Usuario/otros: solo tickets creados por ellos
            if (ticket.SolicitanteId != actor.Id)
                return Result.NoPermitido<TicketDetalleDto>("Solo puede ver los tickets creados por usted.");
        }

        // Resolver nombres en paralelo para evitar múltiples round-trips secuenciales
        var tipoServicioTask = _tipoServicioRepo.ObtenerPorIdAsync(ticket.TipoServicioId, cancellationToken);
        var sucursalTask     = _sucursalRepo.ObtenerPorIdAsync(ticket.SucursalId, cancellationToken);
        var areaTask         = _areaRepo.ObtenerPorIdAsync(ticket.AreaId, cancellationToken);
        var tecnicoTask      = ticket.TecnicoId.HasValue
            ? _usuarioRepository.ObtenerPorIdAsync(ticket.TecnicoId.Value, cancellationToken)
            : Task.FromResult<PideServicio.Domain.Entities.Usuario?>(null);

        await Task.WhenAll(tipoServicioTask, sucursalTask, areaTask, tecnicoTask);

        var tipoServicio = tipoServicioTask.Result;
        var sucursal     = sucursalTask.Result;
        var area         = areaTask.Result;
        var tecnico      = tecnicoTask.Result;

        var dto = ticket.Adapt<TicketDetalleDto>() with
        {
            TipoServicioNombre = tipoServicio?.Nombre,
            SucursalNombre     = sucursal?.Nombre,
            AreaNombre         = area?.Nombre,
            TecnicoNombre      = tecnico is null ? null : $"{tecnico.Nombre} {tecnico.Apellido}",
        };

        return Result.Exito(dto);
    }
}
