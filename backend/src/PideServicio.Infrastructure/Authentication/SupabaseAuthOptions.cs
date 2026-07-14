namespace PideServicio.Infrastructure.Authentication;

public sealed class SupabaseAuthOptions
{
    public const string SeccionNombre = "Supabase";

    public string Url { get; init; } = string.Empty;
    public string AnonKey { get; init; } = string.Empty;
    public string ServiceRoleKey { get; init; } = string.Empty;
    public string JwtSecret { get; init; } = string.Empty;
}
