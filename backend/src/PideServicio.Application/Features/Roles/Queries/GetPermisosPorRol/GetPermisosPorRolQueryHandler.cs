namespace PideServicio.Application.Features.Roles.Queries.GetPermisosPorRol;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;

public sealed class GetPermisosPorRolQueryHandler : IQueryHandler<GetPermisosPorRolQuery, ListResult<PermisoDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPermisosPorRolQueryHandler(
        IRolRepository rolRepository,
        ICurrentUserService currentUserService)
    {
        _rolRepository = rolRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ListResult<PermisoDto>>> Handle(GetPermisosPorRolQuery request, CancellationToken ct)
    {
        var actor = _currentUserService.UsuarioActual;
        if (actor is null)
            return Result.NoAutorizado<ListResult<PermisoDto>>();

        if (!actor.EsAdmin && !actor.EsSuperAdmin)
            return Result.NoPermitido<ListResult<PermisoDto>>(
                "Solo administradores pueden consultar los permisos de un rol.");

        // Si no es SuperAdmin, solo puede consultar su propia empresa
        var empresaId = request.EmpresaId;
        if (!actor.EsSuperAdmin && empresaId.HasValue && empresaId.Value != actor.EmpresaId)
            return Result.NoPermitido<ListResult<PermisoDto>>(
                "No puede consultar los permisos de otra empresa.");

        var permisos = await _rolRepository.ListarPermisosDeRolAsync(request.Rol, empresaId, ct);

        var items = permisos
            .Select(p => p.Adapt<PermisoDto>())
            .ToList();

        return Result.Exito<ListResult<PermisoDto>>(ListResult<PermisoDto>.Crear(items));
    }
}
