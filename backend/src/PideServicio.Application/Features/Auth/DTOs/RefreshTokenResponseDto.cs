namespace PideServicio.Application.Features.Auth.DTOs;

public sealed record RefreshTokenResponseDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TipoToken);
