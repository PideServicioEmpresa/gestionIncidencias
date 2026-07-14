namespace PideServicio.Application.Features.Auth.Commands.CambiarContrasena;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Models;

public sealed class CambiarContrasenaCommandHandler : ICommandHandler<CambiarContrasenaCommand>
{
    private readonly ISupabaseAuthService _supabaseAuth;

    public CambiarContrasenaCommandHandler(ISupabaseAuthService supabaseAuth) => _supabaseAuth = supabaseAuth;

    public async Task<Result> Handle(CambiarContrasenaCommand request, CancellationToken ct)
    {
        try
        {
            await _supabaseAuth.CambiarContrasenaAsync(request.AccessToken, request.NuevaContrasena, ct);
            return Result.Exito();
        }
        catch (InvalidOperationException ex)
        {
            return Result.NoAutorizado(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Fallo($"Error al cambiar contraseña: {ex.Message}");
        }
    }
}
