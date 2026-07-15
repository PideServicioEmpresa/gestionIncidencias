namespace PideServicio.Infrastructure.Notifications;

internal static class EmailTemplates
{
    private const string BaseStyle = """
        body{margin:0;padding:0;background:#f4f4f5;font-family:'Segoe UI',Arial,sans-serif}
        .wrap{max-width:600px;margin:32px auto;background:#fff;border-radius:8px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,.08)}
        .header{background:#2563eb;padding:24px 32px}
        .header h1{margin:0;color:#fff;font-size:20px;font-weight:700}
        .header p{margin:4px 0 0;color:#bfdbfe;font-size:13px}
        .body{padding:28px 32px}
        .badge{display:inline-block;background:#eff6ff;color:#1d4ed8;border:1px solid #bfdbfe;border-radius:20px;padding:3px 12px;font-size:12px;font-weight:600;margin-bottom:16px}
        .title{font-size:17px;font-weight:700;color:#111;margin:0 0 8px}
        .desc{font-size:14px;color:#444;margin:0 0 20px;line-height:1.6}
        table.data{width:100%;border-collapse:collapse;margin:0 0 20px;font-size:13px}
        table.data td{padding:8px 10px;border-bottom:1px solid #f1f5f9;color:#374151}
        table.data td:first-child{color:#6b7280;width:130px;font-weight:500}
        .btn{display:inline-block;background:#2563eb;color:#fff;text-decoration:none;padding:10px 24px;border-radius:6px;font-size:14px;font-weight:600;margin:4px 0 16px}
        .footer{background:#f8fafc;padding:16px 32px;font-size:11px;color:#9ca3af;border-top:1px solid #e5e7eb}
        """;

    private static string Wrap(string titulo, string subtitulo, string cuerpo) => $"""
        <!doctype html><html lang="es"><head><meta charset="utf-8">
        <style>{BaseStyle}</style></head><body>
        <div class="wrap">
          <div class="header"><h1>Pide Servicio</h1><p>{subtitulo}</p></div>
          <div class="body">{cuerpo}</div>
          <div class="footer">Este correo fue generado automáticamente por Pide Servicio. Por favor no responda a este mensaje.</div>
        </div></body></html>
        """;

    public static (string Asunto, string Html) TicketCreado(
        string codigo, string titulo, string prioridad, string area, string solicitante)
    {
        var asunto = $"[{codigo}] Nueva solicitud registrada: {titulo}";
        var html = Wrap("Nueva solicitud", "Sistema de gestión de solicitudes e incidencias", $"""
            <span class="badge">Nueva solicitud · {codigo}</span>
            <p class="title">{EscapeHtml(titulo)}</p>
            <p class="desc">Se ha registrado una nueva solicitud en el sistema y está en espera de asignación.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Prioridad</td><td>{EscapeHtml(prioridad)}</td></tr>
              <tr><td>Área</td><td>{EscapeHtml(area)}</td></tr>
              <tr><td>Solicitante</td><td>{EscapeHtml(solicitante)}</td></tr>
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketAsignado(
        string codigo, string titulo, string tecnico, string prioridad)
    {
        var asunto = $"[{codigo}] Solicitud asignada: {titulo}";
        var html = Wrap("Solicitud asignada", "Se te ha asignado una nueva solicitud", $"""
            <span class="badge">Asignado · {codigo}</span>
            <p class="title">{EscapeHtml(titulo)}</p>
            <p class="desc">Se te ha asignado la siguiente solicitud para su atención.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Técnico asignado</td><td>{EscapeHtml(tecnico)}</td></tr>
              <tr><td>Prioridad</td><td>{EscapeHtml(prioridad)}</td></tr>
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketPendienteValidacion(
        string codigo, string titulo, string tecnico)
    {
        var asunto = $"[{codigo}] Solicitud pendiente de tu validación: {titulo}";
        var html = Wrap("Pendiente de validación", "Tu solicitud requiere validación", $"""
            <span class="badge">Pendiente de validación · {codigo}</span>
            <p class="title">{EscapeHtml(titulo)}</p>
            <p class="desc">El técnico ha completado el trabajo en tu solicitud. Por favor revisa e indica si está conforme para poder cerrarla.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Atendido por</td><td>{EscapeHtml(tecnico)}</td></tr>
            </table>
            <p class="desc">Ingresa a la aplicación para validar o rechazar la atención.</p>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketCerrado(
        string codigo, string titulo, string? valoracion)
    {
        var valoracionTexto = string.IsNullOrEmpty(valoracion) ? "Sin valoración" : valoracion;
        var asunto = $"[{codigo}] Solicitud cerrada: {titulo}";
        var html = Wrap("Solicitud cerrada", "La solicitud ha sido cerrada exitosamente", $"""
            <span class="badge">Cerrado · {codigo}</span>
            <p class="title">{EscapeHtml(titulo)}</p>
            <p class="desc">La solicitud ha sido cerrada y marcada como completada.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Valoración</td><td>{EscapeHtml(valoracionTexto)}</td></tr>
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketReabierto(
        string codigo, string titulo, string motivo)
    {
        var asunto = $"[{codigo}] Solicitud rechazada / reabierta: {titulo}";
        var html = Wrap("Solicitud reabierta", "La validación fue rechazada", $"""
            <span class="badge">Reabierto · {codigo}</span>
            <p class="title">{EscapeHtml(titulo)}</p>
            <p class="desc">El solicitante ha rechazado la atención y la solicitud ha sido reabierta.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Motivo</td><td>{EscapeHtml(motivo)}</td></tr>
            </table>
            """);
        return (asunto, html);
    }

    private static string EscapeHtml(string? text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");
    }
}
