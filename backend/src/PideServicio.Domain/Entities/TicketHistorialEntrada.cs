namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Enums;

/// <summary>
/// Entrada inmutable en el historial de un ticket (tabla ticket_historial).
/// Append-only: nunca se modifica ni elimina. El actor puede ser null en eventos de sistema.
/// Los campos MotivoRechazoId/ComentarioRechazo mapean las columnas rejection_reason_id/rejection_comment de la BD.
/// </summary>
public sealed class TicketHistorialEntrada : BaseEntity
{
    public Guid TicketId { get; private set; }
    public Guid? ActorId { get; private set; }
    public TipoEventoHistorialTipo TipoEvento { get; private set; }
    public TicketEstadoTipo? EstadoAnterior { get; private set; }
    public TicketEstadoTipo? EstadoNuevo { get; private set; }
    public string? ComentarioTexto { get; private set; }
    public Guid? MotivoRechazoId { get; private set; }
    public string? ComentarioRechazo { get; private set; }
    public string? Metadata { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    private TicketHistorialEntrada() { }

    public static TicketHistorialEntrada Crear(
        Guid ticketId,
        TipoEventoHistorialTipo tipoEvento,
        Guid? actorId = null,
        TicketEstadoTipo? estadoAnterior = null,
        TicketEstadoTipo? estadoNuevo = null,
        string? comentarioTexto = null,
        Guid? motivoRechazoId = null,
        string? comentarioRechazo = null,
        string? metadata = null)
    {
        return new TicketHistorialEntrada
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            ActorId = actorId,
            TipoEvento = tipoEvento,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = estadoNuevo,
            ComentarioTexto = comentarioTexto?.Trim(),
            MotivoRechazoId = motivoRechazoId,
            ComentarioRechazo = comentarioRechazo?.Trim(),
            Metadata = metadata,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = actorId
        };
    }
}
