namespace PideServicio.Application.Features.Roles.Queries.GetRoles;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, ListResult<RolDto>>
{
    private readonly IRolRepository _rolRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetRolesQueryHandler(
        IRolRepository rolRepository,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService)
    {
        _rolRepository = rolRepository;
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ListResult<RolDto>>> Handle(GetRolesQuery request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<ListResult<RolDto>>();

        // El rol se toma de la BD, no del claim: si el hook de Supabase no enriquece el
        // token, CurrentUserService cae a un rol por defecto (USUARIO) y un Admin real
        // sería rechazado. El resto de handlers del sistema ya resuelven el actor así.
        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<ListResult<RolDto>>();

        if (actorDb.Rol is not (RolTipo.SUPERADMIN or RolTipo.ADMIN or RolTipo.SUPERVISOR))
            return Result.NoPermitido<ListResult<RolDto>>("Solo administradores y supervisores pueden consultar el catálogo de roles.");

        var roles = await _rolRepository.ListarTodosAsync(ct);

        var items = roles
            .Select(r => r.Adapt<RolDto>())
            .ToList();

        return Result.Exito<ListResult<RolDto>>(ListResult<RolDto>.Crear(items));
    }
}
