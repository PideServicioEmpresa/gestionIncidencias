namespace PideServicio.Application.Features.Roles.Queries.GetPermisos;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;

public sealed class GetPermisosQueryHandler : IQueryHandler<GetPermisosQuery, ListResult<PermisoDto>>
{
    private readonly IPermisoRepository _permisoRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPermisosQueryHandler(
        IPermisoRepository permisoRepository,
        ICurrentUserService currentUserService)
    {
        _permisoRepository = permisoRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ListResult<PermisoDto>>> Handle(GetPermisosQuery request, CancellationToken ct)
    {
        var actor = _currentUserService.UsuarioActual;
        if (actor is null)
            return Result.NoAutorizado<ListResult<PermisoDto>>();

        if (!actor.EsSuperAdmin)
            return Result.NoPermitido<ListResult<PermisoDto>>(
                "Solo el SuperAdministrador puede consultar el catálogo completo de permisos.");

        var permisos = await _permisoRepository.ListarTodosAsync(ct);

        var items = permisos
            .Select(p => p.Adapt<PermisoDto>())
            .ToList();

        return Result.Exito<ListResult<PermisoDto>>(ListResult<PermisoDto>.Crear(items));
    }
}
