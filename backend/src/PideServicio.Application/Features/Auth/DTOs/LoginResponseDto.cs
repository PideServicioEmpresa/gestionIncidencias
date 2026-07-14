namespace PideServicio.Application.Features.Auth.DTOs;

public sealed record LoginResponseDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TipoToken,
    PerfilUsuarioDto Perfil);
