namespace PideServicio.Infrastructure.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    // ClaimsPrincipal se resuelve de forma lazy en cada llamada al getter para evitar
    // capturar un HttpContext nulo durante la construccion del objeto en el pipeline de DI.
    private ClaimsPrincipal? Usuario => httpContextAccessor.HttpContext?.User;

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated is true;

    public CurrentUser? UsuarioActual
    {
        get
        {
            var usuario = Usuario;
            if (usuario?.Identity?.IsAuthenticated is not true)
                return null;

            // Ruta principal: claims enriquecidos por custom_access_token_hook
            if (Guid.TryParse(usuario.FindFirstValue("user_id"), out var id) &&
                Guid.TryParse(usuario.FindFirstValue("auth_id"), out var authId) &&
                Guid.TryParse(usuario.FindFirstValue("empresa_id"), out var empresaId) &&
                Enum.TryParse<RolTipo>(usuario.FindFirstValue("rol"), ignoreCase: true, out var rol))
            {
                var email = usuario.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
                var nombre = usuario.FindFirstValue("nombre_completo") ?? string.Empty;
                var sucursalIds = usuario.Claims
                    .Where(c => c.Type == "sucursal_id")
                    .Select(c => Guid.TryParse(c.Value, out var g) ? g : (Guid?)null)
                    .Where(g => g.HasValue)
                    .Select(g => g!.Value)
                    .ToList();

                return new CurrentUser(id, authId, email, nombre, rol, empresaId, sucursalIds, Activo: true);
            }

            // Fallback: JWT estándar de Supabase sin hook activo.
            // Solo 'sub' está disponible (= auth_id en la BD).
            // El handler puede resolver el resto desde la BD usando AuthId.
            var sub = usuario.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? usuario.FindFirstValue("sub");

            if (Guid.TryParse(sub, out var subId))
            {
                var emailFallback = usuario.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
                return new CurrentUser(
                    Id: Guid.Empty,
                    AuthId: subId,
                    Email: emailFallback,
                    NombreCompleto: string.Empty,
                    Rol: RolTipo.USUARIO,
                    EmpresaId: Guid.Empty,
                    SucursalIds: [],
                    Activo: true);
            }

            return null;
        }
    }
}
