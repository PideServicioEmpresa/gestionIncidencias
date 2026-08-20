namespace PideServicio.Api.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Enums;

/// <summary>
/// Resuelve el rol del usuario contra la base de datos para satisfacer
/// <see cref="RolMinimoRequirement"/>. Cachea el rol unos segundos para no consultar en
/// cada petición, siguiendo el mismo patrón que MantenimientoMiddleware.
/// </summary>
public sealed class RolDesdeBaseDeDatosHandler : AuthorizationHandler<RolMinimoRequirement>
{
    private static readonly TimeSpan TtlRol = TimeSpan.FromSeconds(30);

    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RolDesdeBaseDeDatosHandler> _logger;

    public RolDesdeBaseDeDatosHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        IMemoryCache cache,
        ILogger<RolDesdeBaseDeDatosHandler> logger)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RolMinimoRequirement requirement)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return; // sin identidad: se deja fallar la autorización

        var cacheKey = $"authz:rol:{(claims.Id != Guid.Empty ? claims.Id.ToString() : claims.AuthId.ToString())}";

        if (!_cache.TryGetValue<RolTipo?>(cacheKey, out var rol))
        {
            try
            {
                var usuario = claims.Id != Guid.Empty
                    ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id)
                    : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId);

                // Un usuario inactivo o inexistente no obtiene rol
                rol = usuario is { Activo: true } ? usuario.Rol : null;
            }
            catch (Exception ex)
            {
                // Si la BD falla, no se concede acceso: la autorización simplemente no
                // se satisface y el usuario recibe 403 en lugar de un 500.
                _logger.LogError(ex, "No se pudo resolver el rol del usuario para autorizar la petición.");
                return;
            }

            _cache.Set(cacheKey, rol, TtlRol);
        }

        if (rol.HasValue && requirement.RolesPermitidos.Contains(rol.Value))
            context.Succeed(requirement);
    }
}
