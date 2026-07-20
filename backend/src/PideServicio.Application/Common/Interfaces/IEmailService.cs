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

    /// <summary>
    /// Notifica a los administradores y superadministradores que hay un ticket nuevo
    /// pendiente de asignación. Cada correo se envía de forma independiente.
    /// </summary>
    Task NotificarNuevoTicketAAdminsAsync(
        IReadOnlyList<string> correosAdmins,
        string codigo,
        Guid ticketId,
        string? titulo,
        string prioridad,
        string? sucursal,
        string area,
        string solicitante,
        CancellationToken cancellationToken = default);

    Task NotificarTicketCreadoAsync(
        string correoSolicitante,
        string codigo,
        string? titulo,
        string prioridad,
        string area,
        string solicitante,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifica al técnico que se le asignó un ticket.
    /// Los parámetros opcionales enriquecen el template con contexto adicional.
    /// </summary>
    Task NotificarTicketAsignadoAsync(
        string correoTecnico,
        string codigo,
        string? titulo,
        string tecnico,
        string prioridad,
        string? sucursal = null,
        string? area = null,
        string? solicitante = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifica al técnico que se le reasignó un ticket (previamente asignado a otro).
    /// </summary>
    Task NotificarTicketReasignadoAsync(
        string correoTecnico,
        string codigo,
        string? titulo,
        string tecnico,
        string prioridad,
        string? motivo = null,
        string? sucursal = null,
        string? area = null,
        string? solicitante = null,
        CancellationToken cancellationToken = default);

    Task NotificarAsignacionASolicitanteAsync(
        string correoSolicitante,
        string codigo,
        string? titulo,
        string tecnico,
        string prioridad,
        CancellationToken cancellationToken = default);

    Task NotificarTicketPendienteValidacionAsync(
        string correoSolicitante,
        string codigo,
        string? titulo,
        string tecnico,
        CancellationToken cancellationToken = default);

    /// <summary>Notifica al solicitante que su ticket fue cerrado/validado.</summary>
    Task NotificarTicketCerradoAsync(
        string correoSolicitante,
        string codigo,
        string? titulo,
        string? valoracion,
        CancellationToken cancellationToken = default);

    /// <summary>Notifica al técnico que el ticket fue cerrado por el solicitante.</summary>
    Task NotificarTicketCerradoTecnicoAsync(
        string correoTecnico,
        string codigo,
        string? titulo,
        string? valoracion,
        CancellationToken cancellationToken = default);

    Task NotificarTicketReabiertoAsync(
        string correoTecnico,
        string codigo,
        string? titulo,
        string motivo,
        CancellationToken cancellationToken = default);

    /// <summary>Notifica al solicitante (y copia) que su ticket fue cancelado.</summary>
    Task NotificarTicketCanceladoAsync(
        string correoSolicitante,
        string codigo,
        string? titulo,
        string motivo,
        CancellationToken cancellationToken = default);

    /// <summary>Notifica al solicitante que el técnico inició la atención de su ticket.</summary>
    Task NotificarTicketEnProcesoAsync(
        string correoSolicitante,
        string codigo,
        string? titulo,
        string? tecnico = null,
        CancellationToken cancellationToken = default);

    /// <summary>Notifica al técnico anterior que fue desasignado debido a una reasignación.</summary>
    Task NotificarDesasignacionTecnicoAsync(
        string correoTecnico,
        string codigo,
        string? titulo,
        string? motivo = null,
        CancellationToken cancellationToken = default);

    /// <summary>Notifica al técnico asignado que la prioridad de su ticket fue modificada.</summary>
    Task NotificarCambioPrioridadTecnicoAsync(
        string correoTecnico,
        string codigo,
        string? titulo,
        string prioridadAnterior,
        string prioridadNueva,
        CancellationToken cancellationToken = default);
}
