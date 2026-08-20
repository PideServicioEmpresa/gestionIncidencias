namespace PideServicio.Application.Features.Roles.Queries.GetPermisosPorRol;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetPermisosPorRolQueryHandler : IQueryHandler<GetPermisosPorRolQuery, ListResult<PermisoDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPermisosPorRolQueryHandler(
        IRolRepository rolRepository,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService)
    {
        _rolRepository = rolRepository;
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ListResult<PermisoDto>>> Handle(GetPermisosPorRolQuery request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<ListResult<PermisoDto>>();

        // Rol y empresa desde la BD: el claim puede venir sin enriquecer (ver GetRoles).
        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<ListResult<PermisoDto>>();

        var esSuperAdmin = actorDb.Rol == RolTipo.SUPERADMIN;
        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<ListResult<PermisoDto>>(
                "Solo administradores pueden consultar los permisos de un rol.");

        // Si no es SuperAdmin, solo puede consultar su propia empresa
        var empresaId = request.EmpresaId;
        if (!esSuperAdmin && empresaId.HasValue && empresaId.Value != actorDb.EmpresaId)
            return Result.NoPermitido<ListResult<PermisoDto>>(
                "No puede consultar los permisos de otra empresa.");

        var permisos = await _rolRepository.ListarPermisosDeRolAsync(request.Rol, empresaId, ct);

        var items = permisos
            .Select(p => p.Adapt<PermisoDto>())
            .ToList();

        return Result.Exito<ListResult<PermisoDto>>(ListResult<PermisoDto>.Crear(items));
    }
}
