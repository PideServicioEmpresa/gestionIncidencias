namespace PideServicio.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SeccionNombre = "Jwt";

    public string Authority { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
}
