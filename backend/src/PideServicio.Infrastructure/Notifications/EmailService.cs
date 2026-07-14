namespace PideServicio.Infrastructure.Notifications;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PideServicio.Application.Common.Interfaces;

/// <summary>
/// Implementa IEmailService. Stub preparatorio: la lógica SMTP real se implementa
/// en FASE 6.5+ con MailKit o SendGrid una vez configurado el servidor.
/// Si EmailHabilitado = false, el envío se omite silenciosamente (no interrumpe el flujo).
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly NotificationOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<NotificationOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task EnviarAsync(
        string destinatario,
        string asunto,
        string cuerpoHtml,
        string? cuerpoTexto = null,
        CancellationToken ct = default)
    {
        if (!_options.EmailHabilitado)
        {
            _logger.LogDebug(
                "Email deshabilitado. Se omite envío a {Destinatario}: {Asunto}",
                destinatario, asunto);
            return;
        }

        // TODO FASE 6.5+: Implementar con MailKit/SendGrid cuando se configure el servidor SMTP.
        _logger.LogWarning(
            "Envío de email pendiente de implementar. Destinatario: {Destinatario}, Asunto: {Asunto}",
            destinatario, asunto);

        await Task.CompletedTask;
    }

    public async Task EnviarAVariosAsync(
        IReadOnlyList<string> destinatarios,
        string asunto,
        string cuerpoHtml,
        string? cuerpoTexto = null,
        CancellationToken ct = default)
    {
        foreach (var dest in destinatarios)
            await EnviarAsync(dest, asunto, cuerpoHtml, cuerpoTexto, ct);
    }
}
