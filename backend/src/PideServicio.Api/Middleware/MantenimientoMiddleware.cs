namespace PideServicio.Api.Middleware;

using Microsoft.Extensions.Caching.Memory;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Enums;

/// <summary>
/// Si MODO_MANTENIMIENTO = "true", devuelve 503 a todos los usuarios excepto SUPERADMIN.
/// Lee el parámetro con un cache de 30 s para no golpear la BD en cada request.
/// El bypass de SUPERADMIN se cachea 5 min por ID de usuario.
/// </summary>
public sealed class MantenimientoMiddleware
{
    private const string CacheKeyModo = "mant:modo";
    private static readonly TimeSpan TtlModo = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan TtlSuperAdmin = TimeSpan.FromMinutes(5);

    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public MantenimientoMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IParametroRepository parametroRepo,
        ICurrentUserService currentUserService,
        IUsuarioRepository usuarioRepo)
    {
        // Rutas de salud: nunca bloquear
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/ping", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Leer parámetro de mantenimiento (con cache de corta duración)
        if (!_cache.TryGetValue<bool>(CacheKeyModo, out var modoMantenimiento))
        {
            try
            {
                var param = await parametroRepo.ObtenerPorClaveAsync(
                    "MODO_MANTENIMIENTO", ct: context.RequestAborted);
                modoMantenimiento = param?.Valor.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
            }
            catch
            {
                modoMantenimiento = false;
            }
            _cache.Set(CacheKeyModo, modoMantenimiento, TtlModo);
        }

        if (!modoMantenimiento)
        {
            await _next(context);
            return;
        }

        // Mantenimiento activo → verificar si el usuario es SUPERADMIN
        var claims = currentUserService.UsuarioActual;
        if (claims is not null)
        {
            var cacheKey = $"mant:sa:{(claims.Id != Guid.Empty ? claims.Id.ToString() : claims.AuthId)}";
            if (!_cache.TryGetValue<bool>(cacheKey, out var esSuperAdmin))
            {
                try
                {
                    var actor = claims.Id != Guid.Empty
                        ? await usuarioRepo.ObtenerPorIdAsync(claims.Id, context.RequestAborted)
                        : await usuarioRepo.ObtenerPorAuthIdAsync(claims.AuthId, context.RequestAborted);
                    esSuperAdmin = actor?.Rol == RolTipo.SUPERADMIN;
                }
                catch
                {
                    esSuperAdmin = false;
                }
                _cache.Set(cacheKey, esSuperAdmin, TtlSuperAdmin);
            }

            if (esSuperAdmin)
            {
                await _next(context);
                return;
            }
        }

        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(new
        {
            exitoso = false,
            error = new
            {
                codigo = "SISTEMA_EN_MANTENIMIENTO",
                mensaje = "El sistema está temporalmente en mantenimiento. Por favor, vuelve a intentarlo en unos minutos. Disculpa las molestias."
            }
        });
    }
}
