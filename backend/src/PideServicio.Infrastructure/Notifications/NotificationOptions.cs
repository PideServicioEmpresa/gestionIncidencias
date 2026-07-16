namespace PideServicio.Infrastructure.Notifications;

public sealed class NotificationOptions
{
    public const string SeccionNombre = "Notificaciones";

    public bool EmailHabilitado { get; init; } = false;
    public bool PushHabilitado { get; init; } = false;

    // SMTP
    public string SmtpHost { get; init; } = "smtp.gmail.com";
    public int SmtpPuerto { get; init; } = 587;
    public string SmtpUsuario { get; init; } = string.Empty;
    public string SmtpContrasena { get; init; } = string.Empty;

    // Remitente
    public string RemitenteDireccion { get; init; } = string.Empty;
    public string RemitenteNombre { get; init; } = "Pide Servicio";

    // Correo fijo que recibe copia de todos los eventos (ej: inmoveg)
    public string CorreoCopia { get; init; } = string.Empty;

    // URL base del frontend — usada en los CTAs de los correos (ej. https://app.pideservicio.com)
    public string UrlFrontend { get; init; } = string.Empty;
}
