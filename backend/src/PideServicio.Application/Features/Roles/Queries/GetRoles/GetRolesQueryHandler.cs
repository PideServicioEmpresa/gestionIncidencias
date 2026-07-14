namespace PideServicio.Application.Features.Roles.Queries.GetRoles;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;

public sealed class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, ListResult<RolDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetRolesQueryHandler(
        IRolRepository rolRepository,
        ICurrentUserService currentUserService)
    {
        _rolRepository = rolRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ListResult<RolDto>>> Handle(GetRolesQuery request, CancellationToken ct)
    {
        var actor = _currentUserService.UsuarioActual;
        if (actor is null)
            return Result.NoAutorizado<ListResult<RolDto>>();

        if (!actor.TieneAccesoAdministrativo)
            return Result.NoPermitido<ListResult<RolDto>>("Solo administradores y supervisores pueden consultar el catálogo de roles.");

        var roles = await _rolRepository.ListarTodosAsync(ct);

        var items = roles
            .Select(r => r.Adapt<RolDto>())
            .ToList();

        return Result.Exito<ListResult<RolDto>>(ListResult<RolDto>.Crear(items));
    }
}
