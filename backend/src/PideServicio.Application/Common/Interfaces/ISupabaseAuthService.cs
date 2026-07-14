namespace PideServicio.Application.Common.Interfaces;

public interface ISupabaseAuthService
{
    Task<SupabaseAuthResult> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<SupabaseAuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task LogoutAsync(string accessToken, CancellationToken ct = default);
    Task CambiarContrasenaAsync(string accessToken, string nuevaContrasena, CancellationToken ct = default);
    Task RecuperarContrasenaAsync(string email, CancellationToken ct = default);

    // Admin API — requiere ServiceRoleKey
    Task<Guid> CrearUsuarioEnAuthAsync(string email, string contrasena, CancellationToken ct = default);
    Task EliminarUsuarioDeAuthAsync(Guid authId, CancellationToken ct = default);
}

public sealed record SupabaseAuthResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    Guid SupabaseUserId,
    string Email);
