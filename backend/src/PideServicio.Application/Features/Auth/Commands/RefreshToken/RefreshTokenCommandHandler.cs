namespace PideServicio.Application.Features.Auth.Commands.RefreshToken;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Auth.DTOs;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResponseDto>
{
    private readonly ISupabaseAuthService _supabaseAuth;

    public RefreshTokenCommandHandler(ISupabaseAuthService supabaseAuth) => _supabaseAuth = supabaseAuth;

    public async Task<Result<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        SupabaseAuthResult result;
        try
        {
            result = await _supabaseAuth.RefreshAsync(request.RefreshToken, ct);
        }
        catch (InvalidOperationException ex)
        {
            return Result.NoAutorizado<RefreshTokenResponseDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Fallo<RefreshTokenResponseDto>($"Error al renovar el token: {ex.Message}");
        }

        return Result.Exito(new RefreshTokenResponseDto(
            AccessToken: result.AccessToken,
            RefreshToken: result.RefreshToken,
            ExpiresIn: result.ExpiresIn,
            TipoToken: "bearer"));
    }
}
