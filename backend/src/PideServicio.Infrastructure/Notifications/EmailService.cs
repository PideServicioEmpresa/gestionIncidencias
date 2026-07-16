namespace PideServicio.Infrastructure.Notifications;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PideServicio.Application.Common.Interfaces;

public sealed class EmailService : IEmailService
{
    private readonly NotificationOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<NotificationOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger<EmailService> logger)
    {
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient("brevo");
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

        if (string.IsNullOrWhiteSpace(_options.BrevoApiKey))
        {
            _logger.LogWarning("Brevo API key no configurada (Notificaciones__BrevoApiKey). Se omite envío a {Destinatario}", destinatario);
            return;
        }

        try
        {
            await EnviarViaBrevoAsync(destinatario, asunto, cuerpoHtml, cuerpoTexto, ct);
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

    private async Task EnviarViaBrevoAsync(
        string destinatario, string asunto, string htmlContent, string? cuerpoTexto, CancellationToken ct)
    {
        // textContent: nunca vacío para evitar filtros de spam y rechazos de Brevo
        var textoPlano = !string.IsNullOrWhiteSpace(cuerpoTexto)
            ? cuerpoTexto
            : StripHtml(htmlContent);
        if (string.IsNullOrWhiteSpace(textoPlano))
            textoPlano = asunto;

        var remitente = string.IsNullOrWhiteSpace(_options.RemitenteDireccion)
            ? _options.SmtpUsuario  // fallback a usuario SMTP por compatibilidad histórica
            : _options.RemitenteDireccion;

        var payload = new
        {
            sender = new { name = _options.RemitenteNombre, email = remitente },
            to = new[] { new { email = destinatario } },
            subject = asunto,
            htmlContent = htmlContent,
            textContent = textoPlano,
        };

        var json = JsonSerializer.Serialize(payload);
        using var request = new HttpRequestMessage(HttpMethod.Post, "smtp/email");
        // api-key se agrega por request (no en AddHttpClient) para leer desde IOptions en tiempo de ejecución
        request.Headers.Add("api-key", _options.BrevoApiKey);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var cuerpoError = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError(
                "Brevo API respondió {StatusCode} al enviar a {Destinatario} — asunto: {Asunto}. Detalle Brevo: {BrevoError}",
                (int)response.StatusCode, destinatario, asunto, cuerpoError);
            // Lanzar para que EnviarAsync registre también el contexto del llamador
            response.EnsureSuccessStatusCode();
        }
    }

    private static string StripHtml(string html)
        => System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ")
            .Replace("  ", " ").Trim();
}
