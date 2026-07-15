namespace PideServicio.Application.Common.Interfaces;

public interface IEmailService
{
    Task EnviarAsync(
        string destinatario,
        string asunto,
        string cuerpoHtml,
        string? cuerpoTexto = null,
        CancellationToken ct = default);

    Task EnviarAVariosAsync(
        IReadOnlyList<string> destinatarios,
        string asunto,
        string cuerpoHtml,
        string? cuerpoTexto = null,
        CancellationToken ct = default);

    // Métodos de conveniencia para eventos de tickets
    Task NotificarTicketCreadoAsync(
        string correoSolicitante,
        string codigo,
        string titulo,
        string prioridad,
        string area,
        string solicitante,
        CancellationToken cancellationToken = default);

    Task NotificarTicketAsignadoAsync(
        string correoTecnico,
        string codigo,
        string titulo,
        string tecnico,
        string prioridad,
        CancellationToken cancellationToken = default);

    Task NotificarTicketPendienteValidacionAsync(
        string correoSolicitante,
        string codigo,
        string titulo,
        string tecnico,
        CancellationToken cancellationToken = default);

    Task NotificarTicketCerradoAsync(
        string correoSolicitante,
        string codigo,
        string titulo,
        string? valoracion,
        CancellationToken cancellationToken = default);

    Task NotificarTicketReabiertoAsync(
        string correoTecnico,
        string codigo,
        string titulo,
        string motivo,
        CancellationToken cancellationToken = default);
}
