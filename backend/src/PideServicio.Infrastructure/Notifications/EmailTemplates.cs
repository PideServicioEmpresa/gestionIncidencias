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

    private static string AsuntoConTitulo(string prefijo, string codigo, string? titulo) =>
        string.IsNullOrWhiteSpace(titulo)
            ? $"[{codigo}] {prefijo}"
            : $"[{codigo}] {prefijo}: {titulo.Trim()}";

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
        string codigo, string? titulo, string prioridad, string area, string solicitante)
    {
        var asunto = AsuntoConTitulo("Nueva solicitud registrada", codigo, titulo);
        var html = Wrap("Nueva solicitud", "Sistema de gestión de solicitudes e incidencias", $"""
            <span class="badge">Nueva solicitud · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
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
        string codigo, string? titulo, string tecnico, string prioridad,
        string? sucursal = null, string? area = null, string? solicitante = null)
    {
        var asunto = AsuntoConTitulo("Solicitud asignada", codigo, titulo);

        var filasOpcionales = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(sucursal))
            filasOpcionales.Append($"<tr><td>Sucursal</td><td>{EscapeHtml(sucursal)}</td></tr>");
        if (!string.IsNullOrEmpty(area))
            filasOpcionales.Append($"<tr><td>Área</td><td>{EscapeHtml(area)}</td></tr>");
        if (!string.IsNullOrEmpty(solicitante))
            filasOpcionales.Append($"<tr><td>Solicitante</td><td>{EscapeHtml(solicitante)}</td></tr>");

        var html = Wrap("Solicitud asignada", "Se te ha asignado una nueva solicitud", $"""
            <span class="badge">Asignado · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">Se te ha asignado la siguiente solicitud para su atención. Por favor revisa los detalles e inicia el proceso.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Estado</td><td>Asignado</td></tr>
              <tr><td>Prioridad</td><td>{EscapeHtml(prioridad)}</td></tr>
              <tr><td>Técnico asignado</td><td>{EscapeHtml(tecnico)}</td></tr>
              {filasOpcionales}
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketReasignado(
        string codigo, string? titulo, string tecnico, string prioridad,
        string? motivo = null, string? sucursal = null, string? area = null, string? solicitante = null)
    {
        var asunto = AsuntoConTitulo("Solicitud reasignada", codigo, titulo);

        var filasOpcionales = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(sucursal))
            filasOpcionales.Append($"<tr><td>Sucursal</td><td>{EscapeHtml(sucursal)}</td></tr>");
        if (!string.IsNullOrEmpty(area))
            filasOpcionales.Append($"<tr><td>Área</td><td>{EscapeHtml(area)}</td></tr>");
        if (!string.IsNullOrEmpty(solicitante))
            filasOpcionales.Append($"<tr><td>Solicitante</td><td>{EscapeHtml(solicitante)}</td></tr>");
        if (!string.IsNullOrEmpty(motivo))
            filasOpcionales.Append($"<tr><td>Motivo de reasignación</td><td>{EscapeHtml(motivo)}</td></tr>");

        var html = Wrap("Solicitud reasignada", "Se te ha reasignado una solicitud", $"""
            <span class="badge">Reasignado · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">Esta solicitud ha sido reasignada y queda bajo tu responsabilidad.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Estado</td><td>Asignado</td></tr>
              <tr><td>Prioridad</td><td>{EscapeHtml(prioridad)}</td></tr>
              <tr><td>Técnico asignado</td><td>{EscapeHtml(tecnico)}</td></tr>
              {filasOpcionales}
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketAsignadoSolicitante(
        string codigo, string? titulo, string tecnico, string prioridad)
    {
        var asunto = AsuntoConTitulo("Tu solicitud fue asignada", codigo, titulo);
        var html = Wrap("Solicitud en atención", "Tu solicitud ha sido asignada a un técnico", $"""
            <span class="badge">En atención · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">Tu solicitud ha sido asignada y está siendo atendida. Te notificaremos cuando el técnico complete el trabajo.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Técnico asignado</td><td>{EscapeHtml(tecnico)}</td></tr>
              <tr><td>Prioridad</td><td>{EscapeHtml(prioridad)}</td></tr>
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketPendienteValidacion(
        string codigo, string? titulo, string tecnico)
    {
        var asunto = AsuntoConTitulo("Solicitud pendiente de tu validación", codigo, titulo);
        var html = Wrap("Pendiente de validación", "Tu solicitud requiere validación", $"""
            <span class="badge">Pendiente de validación · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
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
        string codigo, string? titulo, string? valoracion)
    {
        var valoracionTexto = string.IsNullOrEmpty(valoracion) ? "Sin valoración" : valoracion;
        var asunto = AsuntoConTitulo("Solicitud cerrada", codigo, titulo);
        var html = Wrap("Solicitud cerrada", "La solicitud ha sido cerrada exitosamente", $"""
            <span class="badge">Cerrado · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">La solicitud ha sido cerrada y marcada como completada.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Valoración</td><td>{EscapeHtml(valoracionTexto)}</td></tr>
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketReabierto(
        string codigo, string? titulo, string motivo)
    {
        var asunto = AsuntoConTitulo("Solicitud rechazada / reabierta", codigo, titulo);
        var html = Wrap("Solicitud reabierta", "La validación fue rechazada", $"""
            <span class="badge">Reabierto · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">El solicitante ha rechazado la atención y la solicitud ha sido reabierta. El administrador la reasignará próximamente.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Motivo de rechazo</td><td>{EscapeHtml(motivo)}</td></tr>
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketCerradoTecnico(
        string codigo, string? titulo, string? valoracion)
    {
        var valoracionTexto = string.IsNullOrEmpty(valoracion) ? "Sin valoración" : valoracion;
        var asunto = AsuntoConTitulo("Solicitud cerrada por el solicitante", codigo, titulo);
        var html = Wrap("Solicitud cerrada", "El solicitante ha cerrado la solicitud", $"""
            <span class="badge">Cerrado · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">El solicitante ha validado y cerrado la solicitud. ¡Buen trabajo!</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Valoración recibida</td><td>{EscapeHtml(valoracionTexto)}</td></tr>
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketCancelado(
        string codigo, string? titulo, string motivo)
    {
        var asunto = AsuntoConTitulo("Solicitud cancelada", codigo, titulo);
        var html = Wrap("Solicitud cancelada", "Tu solicitud ha sido cancelada", $"""
            <span class="badge">Cancelado · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">Tu solicitud ha sido cancelada. Si necesitas asistencia, por favor registra una nueva solicitud.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Motivo de cancelación</td><td>{EscapeHtml(motivo)}</td></tr>
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketEnProceso(
        string codigo, string? titulo, string? tecnico)
    {
        var asunto = AsuntoConTitulo("Tu solicitud está siendo atendida", codigo, titulo);
        var filaTecnico = string.IsNullOrEmpty(tecnico)
            ? string.Empty
            : $"<tr><td>Técnico</td><td>{EscapeHtml(tecnico)}</td></tr>";

        var html = Wrap("Solicitud en proceso", "El técnico ya inició la atención de tu solicitud", $"""
            <span class="badge">En proceso · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">El técnico ha comenzado a trabajar en tu solicitud. Te notificaremos cuando esté lista para validación.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              {filaTecnico}
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) DesasignacionTecnico(
        string codigo, string? titulo, string? motivo)
    {
        var asunto = AsuntoConTitulo("Has sido desasignado de una solicitud", codigo, titulo);
        var filaMotivo = string.IsNullOrEmpty(motivo)
            ? string.Empty
            : $"<tr><td>Motivo</td><td>{EscapeHtml(motivo)}</td></tr>";

        var html = Wrap("Desasignado de solicitud", "Has sido removido de una solicitud", $"""
            <span class="badge">Reasignado · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">Has sido desasignado de la siguiente solicitud. Un administrador la reasignó a otro técnico.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              {filaMotivo}
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) CambioPrioridadTecnico(
        string codigo, string? titulo, string prioridadAnterior, string prioridadNueva)
    {
        var asunto = AsuntoConTitulo("Prioridad modificada", codigo, titulo);
        var html = Wrap("Prioridad actualizada", "La prioridad de tu solicitud asignada fue modificada", $"""
            <span class="badge">Prioridad cambiada · {codigo}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">Un administrador ha modificado la prioridad de esta solicitud asignada a ti.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Prioridad anterior</td><td>{EscapeHtml(prioridadAnterior)}</td></tr>
              <tr><td>Nueva prioridad</td><td><strong>{EscapeHtml(prioridadNueva)}</strong></td></tr>
            </table>
            """);
        return (asunto, html);
    }

    public static (string Asunto, string Html) TicketCreadoAdmin(
        string codigo, Guid ticketId, string? titulo, string prioridad,
        string? sucursal, string area, string solicitante, string? urlFrontend)
    {
        var asunto = AsuntoConTitulo("Nuevo ticket pendiente de asignación", codigo, titulo);

        var filasSucursal = !string.IsNullOrEmpty(sucursal)
            ? $"<tr><td>Sucursal</td><td>{EscapeHtml(sucursal)}</td></tr>"
            : string.Empty;

        var cta = string.Empty;
        if (!string.IsNullOrWhiteSpace(urlFrontend))
        {
            var enlace = $"{urlFrontend.TrimEnd('/')}/tickets/{ticketId}";
            cta = $"""<a href="{enlace}" class="btn">Ir al ticket</a>""";
        }

        var html = Wrap("Nuevo ticket pendiente", "Requiere asignación de técnico", $"""
            <span class="badge">Sin asignar · {EscapeHtml(codigo)}</span>
            <p class="title">{EscapeHtml(string.IsNullOrWhiteSpace(titulo) ? codigo : titulo.Trim())}</p>
            <p class="desc">Se ha registrado un nuevo ticket que está pendiente de asignación. Ingresa al sistema para asignarlo a un técnico.</p>
            <table class="data">
              <tr><td>Código</td><td><strong>{EscapeHtml(codigo)}</strong></td></tr>
              <tr><td>Prioridad</td><td>{EscapeHtml(prioridad)}</td></tr>
              {filasSucursal}
              <tr><td>Área</td><td>{EscapeHtml(area)}</td></tr>
              <tr><td>Solicitante</td><td>{EscapeHtml(solicitante)}</td></tr>
            </table>
            {cta}
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
