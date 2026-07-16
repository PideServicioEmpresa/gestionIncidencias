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

    public Task NotificarNuevoTicketAAdminsAsync(
        IReadOnlyList<string> correosAdmins,
        string codigo,
        Guid ticketId,
        string titulo,
        string prioridad,
        string? sucursal,
        string area,
        string solicitante,
        CancellationToken cancellationToken = default)
    {
        if (correosAdmins.Count == 0)
            return Task.CompletedTask;

        try
        {
            var urlFrontend = string.IsNullOrWhiteSpace(_options.UrlFrontend) ? null : _options.UrlFrontend;
            var (asunto, html) = EmailTemplates.TicketCreadoAdmin(
                codigo, ticketId, titulo, prioridad, sucursal, area, solicitante, urlFrontend);
            return EnviarAVariosAsync(correosAdmins, asunto, html, ct: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarNuevoTicketAAdminsAsync para ticket {Codigo}", codigo);
            return Task.CompletedTask;
        }
    }

    public Task NotificarTicketCreadoAsync(
        string correoSolicitante, string codigo, string titulo,
        string prioridad, string area, string solicitante,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketCreado(codigo, titulo, prioridad, area, solicitante);
            return EnviarConCopiaAsync(correoSolicitante, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarTicketCreadoAsync para {Correo}", correoSolicitante);
            return Task.CompletedTask;
        }
    }

    public Task NotificarTicketAsignadoAsync(
        string correoTecnico, string codigo, string titulo,
        string tecnico, string prioridad,
        string? sucursal = null, string? area = null, string? solicitante = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketAsignado(codigo, titulo, tecnico, prioridad, sucursal, area, solicitante);
            return EnviarConCopiaAsync(correoTecnico, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarTicketAsignadoAsync para {Correo}", correoTecnico);
            return Task.CompletedTask;
        }
    }

    public Task NotificarTicketReasignadoAsync(
        string correoTecnico, string codigo, string titulo,
        string tecnico, string prioridad,
        string? motivo = null, string? sucursal = null, string? area = null, string? solicitante = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketReasignado(codigo, titulo, tecnico, prioridad, motivo, sucursal, area, solicitante);
            return EnviarConCopiaAsync(correoTecnico, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarTicketReasignadoAsync para {Correo}", correoTecnico);
            return Task.CompletedTask;
        }
    }

    public Task NotificarAsignacionASolicitanteAsync(
        string correoSolicitante, string codigo, string titulo,
        string tecnico, string prioridad,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketAsignadoSolicitante(codigo, titulo, tecnico, prioridad);
            return EnviarConCopiaAsync(correoSolicitante, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarAsignacionASolicitanteAsync para {Correo}", correoSolicitante);
            return Task.CompletedTask;
        }
    }

    public Task NotificarTicketPendienteValidacionAsync(
        string correoSolicitante, string codigo, string titulo,
        string tecnico,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketPendienteValidacion(codigo, titulo, tecnico);
            return EnviarConCopiaAsync(correoSolicitante, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarTicketPendienteValidacionAsync para {Correo}", correoSolicitante);
            return Task.CompletedTask;
        }
    }

    public Task NotificarTicketCerradoAsync(
        string correoSolicitante, string codigo, string titulo,
        string? valoracion,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketCerrado(codigo, titulo, valoracion);
            return EnviarConCopiaAsync(correoSolicitante, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarTicketCerradoAsync para {Correo}", correoSolicitante);
            return Task.CompletedTask;
        }
    }

    public Task NotificarTicketCerradoTecnicoAsync(
        string correoTecnico, string codigo, string titulo,
        string? valoracion,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketCerradoTecnico(codigo, titulo, valoracion);
            return EnviarConCopiaAsync(correoTecnico, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarTicketCerradoTecnicoAsync para {Correo}", correoTecnico);
            return Task.CompletedTask;
        }
    }

    public Task NotificarTicketReabiertoAsync(
        string correoTecnico, string codigo, string titulo,
        string motivo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketReabierto(codigo, titulo, motivo);
            return EnviarConCopiaAsync(correoTecnico, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarTicketReabiertoAsync para {Correo}", correoTecnico);
            return Task.CompletedTask;
        }
    }

    public Task NotificarTicketCanceladoAsync(
        string correoSolicitante, string codigo, string titulo,
        string motivo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketCancelado(codigo, titulo, motivo);
            return EnviarConCopiaAsync(correoSolicitante, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarTicketCanceladoAsync para {Correo}", correoSolicitante);
            return Task.CompletedTask;
        }
    }

    public Task NotificarTicketEnProcesoAsync(
        string correoSolicitante, string codigo, string titulo,
        string? tecnico = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.TicketEnProceso(codigo, titulo, tecnico);
            return EnviarConCopiaAsync(correoSolicitante, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarTicketEnProcesoAsync para {Correo}", correoSolicitante);
            return Task.CompletedTask;
        }
    }

    public Task NotificarDesasignacionTecnicoAsync(
        string correoTecnico, string codigo, string titulo,
        string? motivo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.DesasignacionTecnico(codigo, titulo, motivo);
            return EnviarConCopiaAsync(correoTecnico, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarDesasignacionTecnicoAsync para {Correo}", correoTecnico);
            return Task.CompletedTask;
        }
    }

    public Task NotificarCambioPrioridadTecnicoAsync(
        string correoTecnico, string codigo, string titulo,
        string prioridadAnterior, string prioridadNueva,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (asunto, html) = EmailTemplates.CambioPrioridadTecnico(codigo, titulo, prioridadAnterior, prioridadNueva);
            return EnviarConCopiaAsync(correoTecnico, asunto, html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar email NotificarCambioPrioridadTecnicoAsync para {Correo}", correoTecnico);
            return Task.CompletedTask;
        }
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
        const int MaxIntentos = 2;

        for (int intento = 1; intento <= MaxIntentos; intento++)
        {
            using var cliente = new SmtpClient();
            cliente.Timeout = 30_000; // 30 segundos; evita cuelgues indefinidos en la conexión SMTP
            try
            {
                await cliente.ConnectAsync(_options.SmtpHost, _options.SmtpPuerto, SecureSocketOptions.StartTls, ct);
                await cliente.AuthenticateAsync(_options.SmtpUsuario, _options.SmtpContrasena, ct);
                await cliente.SendAsync(mensaje, ct);
                await cliente.DisconnectAsync(quit: true, ct);
                return;
            }
            catch (Exception ex) when (intento < MaxIntentos && !ct.IsCancellationRequested)
            {
                _logger.LogWarning(ex,
                    "Intento {Intento}/{Max} fallido al enviar email SMTP a {Destinatario}. Se reintenta en 3 s.",
                    intento, MaxIntentos, mensaje.To.FirstOrDefault()?.ToString());
                await Task.Delay(TimeSpan.FromSeconds(3), ct);
            }
        }
    }

    private static string StripHtml(string html)
    {
        return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ")
            .Replace("  ", " ").Trim();
    }
}
