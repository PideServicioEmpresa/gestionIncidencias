namespace PideServicio.Application.Features.Auth.Commands.RecuperarContrasena;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Models;

public sealed class RecuperarContrasenaCommandHandler : ICommandHandler<RecuperarContrasenaCommand>
{
    private readonly ISupabaseAuthService _supabaseAuth;

    public RecuperarContrasenaCommandHandler(ISupabaseAuthService supabaseAuth) => _supabaseAuth = supabaseAuth;

    public async Task<Result> Handle(RecuperarContrasenaCommand request, CancellationToken ct)
    {
        try
        {
            await _supabaseAuth.RecuperarContrasenaAsync(request.Email, ct);
        }
        catch
        {
            // Silenciar error: no revelar si el correo existe o no
        }

        // Siempre retorna éxito para no revelar si el email existe
        return Result.Exito();
    }
}
