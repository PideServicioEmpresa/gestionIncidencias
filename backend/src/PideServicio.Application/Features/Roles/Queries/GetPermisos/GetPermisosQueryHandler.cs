namespace PideServicio.Application.Features.Roles.Queries.GetPermisos;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetPermisosQueryHandler : IQueryHandler<GetPermisosQuery, ListResult<PermisoDto>>
{
    private readonly IPermisoRepository _permisoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPermisosQueryHandler(
        IPermisoRepository permisoRepository,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService)
    {
        _permisoRepository = permisoRepository;
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ListResult<PermisoDto>>> Handle(GetPermisosQuery request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<ListResult<PermisoDto>>();

        // Rol desde la BD: el claim puede venir sin enriquecer (ver GetRoles).
        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<ListResult<PermisoDto>>();

        if (actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<ListResult<PermisoDto>>(
                "Solo el SuperAdministrador puede consultar el catálogo completo de permisos.");

        var permisos = await _permisoRepository.ListarTodosAsync(ct);

        var items = permisos
            .Select(p => p.Adapt<PermisoDto>())
            .ToList();

        return Result.Exito<ListResult<PermisoDto>>(ListResult<PermisoDto>.Crear(items));
    }
}
