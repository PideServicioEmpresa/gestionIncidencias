namespace PideServicio.Application.Features.Auth.Commands.Logout;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Models;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly ISupabaseAuthService _supabaseAuth;

    public LogoutCommandHandler(ISupabaseAuthService supabaseAuth) => _supabaseAuth = supabaseAuth;

    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        try
        {
            await _supabaseAuth.LogoutAsync(request.AccessToken, ct);
            return Result.Exito();
        }
        catch (Exception ex)
        {
            return Result.Fallo($"Error al cerrar sesión: {ex.Message}");
        }
    }
}
