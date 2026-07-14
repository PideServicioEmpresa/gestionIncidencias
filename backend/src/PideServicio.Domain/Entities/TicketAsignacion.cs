namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Registro inmutable de cada asignación/reasignación de un ticket.
/// Append-only: nunca se modifica ni elimina.
/// </summary>
public sealed class TicketAsignacion : BaseEntity
{
    public Guid TicketId { get; private set; }
    public Guid TecnicoId { get; private set; }
    public Guid AsignadorId { get; private set; }
    public bool EsReasignacion { get; private set; }
    public Guid? TecnicoAnteriorId { get; private set; }
    public string? MotivoReasignacion { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }

    private TicketAsignacion() { }

    public static TicketAsignacion Registrar(
        Guid ticketId,
        Guid tecnicoId,
        Guid asignadorId,
        bool esReasignacion = false,
        Guid? tecnicoAnteriorId = null,
        string? motivoReasignacion = null)
    {
        if (esReasignacion && !tecnicoAnteriorId.HasValue)
            throw new AsignacionInvalidaException("Una reasignación requiere especificar el técnico anterior.");
        if (!esReasignacion && tecnicoAnteriorId.HasValue)
            throw new AsignacionInvalidaException("Solo se registra técnico anterior en reasignaciones.");

        return new TicketAsignacion
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            TecnicoId = tecnicoId,
            AsignadorId = asignadorId,
            EsReasignacion = esReasignacion,
            TecnicoAnteriorId = tecnicoAnteriorId,
            MotivoReasignacion = motivoReasignacion?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = asignadorId
        };
    }
}
