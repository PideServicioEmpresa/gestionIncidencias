namespace PideServicio.Api.Authorization;

using Microsoft.AspNetCore.Authorization;
using PideServicio.Domain.Enums;

/// <summary>
/// Exige que el usuario tenga uno de los roles indicados, resolviéndolo desde la BASE
/// DE DATOS y no desde el claim "rol" del token.
///
/// Motivo: el claim solo existe si el hook custom_access_token_hook de Supabase llegó a
/// enriquecer el token. Cuando no ocurre (token emitido antes del hook, usuario recién
/// creado, cambio de rol posterior), RequireClaim rechaza con 403 a usuarios que sí
/// tienen el rol en la base. La BD es la única fuente de verdad del rol.
/// </summary>
public sealed class RolMinimoRequirement : IAuthorizationRequirement
{
    public RolMinimoRequirement(params RolTipo[] rolesPermitidos)
        => RolesPermitidos = rolesPermitidos;

    public IReadOnlyCollection<RolTipo> RolesPermitidos { get; }
}
