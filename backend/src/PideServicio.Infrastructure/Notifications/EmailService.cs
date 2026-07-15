namespace PideServicio.Infrastructure.Notifications;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PideServicio.Application.Common.Interfaces;

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
            _logger.LogDebug("Email deshabilitado. Se omite envío a {Destinatario}: {Asunto}", destinatario, asunto);
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.SmtpUsuario) || string.IsNullOrWhiteSpace(_options.SmtpContrasena))
        {
            _logger.LogWarning("Credenciales SMTP no configuradas. Se omite envío a {Destinatario}", destinatario);
            return;
        }

        try
        {
            var mensaje = ConstruirMensaje(destinatario, asunto, cuerpoHtml, cuerpoTexto);
            await EnviarMensajeAsync(mensaje, ct);

            _logger.LogInformation("Email enviado a {Destinatario}: {Asunto}", destinatario, asunto);
        }
        catch (Exception ex)
        {
            // El fallo de email NO interrumpe el flujo principal
            _logger.LogError(ex, "Error al enviar email a {Destinatario}: {Asunto}", destinatario, asunto);
        }
    }

    public async Task EnviarAVariosAsync(
        IReadOnlyList<string> destinatarios,
        string asunto,
        string cuerpoHtml,
        string? cuerpoTexto = null,
        CancellationToken ct = default)
    {
        var tareas = destinatarios
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(d => EnviarAsync(d, asunto, cuerpoHtml, cuerpoTexto, ct));

        await Task.WhenAll(tareas);
    }

    public Task NotificarTicketCreadoAsync(
        string correoSolicitante, string codigo, string titulo,
        string prioridad, string area, string solicitante,
        CancellationToken cancellationToken = default)
    {
        var (asunto, html) = EmailTemplates.TicketCreado(codigo, titulo, prioridad, area, solicitante);
        return EnviarConCopiaAsync(correoSolicitante, asunto, html, cancellationToken);
    }

    public Task NotificarTicketAsignadoAsync(
        string correoTecnico, string codigo, string titulo,
        string tecnico, string prioridad,
        CancellationToken cancellationToken = default)
    {
        var (asunto, html) = EmailTemplates.TicketAsignado(codigo, titulo, tecnico, prioridad);
        return EnviarConCopiaAsync(correoTecnico, asunto, html, cancellationToken);
    }

    public Task NotificarTicketPendienteValidacionAsync(
        string correoSolicitante, string codigo, string titulo,
        string tecnico,
        CancellationToken cancellationToken = default)
    {
        var (asunto, html) = EmailTemplates.TicketPendienteValidacion(codigo, titulo, tecnico);
        return EnviarConCopiaAsync(correoSolicitante, asunto, html, cancellationToken);
    }

    public Task NotificarTicketCerradoAsync(
        string correoSolicitante, string codigo, string titulo,
        string? valoracion,
        CancellationToken cancellationToken = default)
    {
        var (asunto, html) = EmailTemplates.TicketCerrado(codigo, titulo, valoracion);
        return EnviarConCopiaAsync(correoSolicitante, asunto, html, cancellationToken);
    }

    public Task NotificarTicketReabiertoAsync(
        string correoTecnico, string codigo, string titulo,
        string motivo,
        CancellationToken cancellationToken = default)
    {
        var (asunto, html) = EmailTemplates.TicketReabierto(codigo, titulo, motivo);
        return EnviarConCopiaAsync(correoTecnico, asunto, html, cancellationToken);
    }

    // -------------------------------------------------------------------------

    private Task EnviarConCopiaAsync(string destinatarioPrincipal, string asunto, string html, CancellationToken ct)
    {
        var destinatarios = new List<string> { destinatarioPrincipal };
        if (!string.IsNullOrWhiteSpace(_options.CorreoCopia))
            destinatarios.Add(_options.CorreoCopia);

        return EnviarAVariosAsync(destinatarios, asunto, html, ct: ct);
    }

    private MimeMessage ConstruirMensaje(string destinatario, string asunto, string cuerpoHtml, string? cuerpoTexto)
    {
        var remitente = string.IsNullOrWhiteSpace(_options.RemitenteDireccion)
            ? _options.SmtpUsuario
            : _options.RemitenteDireccion;

        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_options.RemitenteNombre, remitente));
        mensaje.To.Add(MailboxAddress.Parse(destinatario));
        mensaje.Subject = asunto;

        var builder = new BodyBuilder
        {
            HtmlBody = cuerpoHtml,
            TextBody = cuerpoTexto ?? StripHtml(cuerpoHtml),
        };
        mensaje.Body = builder.ToMessageBody();

        return mensaje;
    }

    private async Task EnviarMensajeAsync(MimeMessage mensaje, CancellationToken ct)
    {
        using var cliente = new SmtpClient();
        await cliente.ConnectAsync(_options.SmtpHost, _options.SmtpPuerto, SecureSocketOptions.StartTls, ct);
        await cliente.AuthenticateAsync(_options.SmtpUsuario, _options.SmtpContrasena, ct);
        await cliente.SendAsync(mensaje, ct);
        await cliente.DisconnectAsync(quit: true, ct);
    }

    private static string StripHtml(string html)
    {
        return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ")
            .Replace("  ", " ").Trim();
    }
}
