namespace PideServicio.Infrastructure.Notifications;

public sealed class NotificationOptions
{
    public const string SeccionNombre = "Notificaciones";

    public bool EmailHabilitado { get; init; } = false;
    public bool PushHabilitado { get; init; } = false;
    public string? SmtpHost { get; init; }
    public int SmtpPuerto { get; init; } = 587;
    public string? SmtpUsuario { get; init; }
}
