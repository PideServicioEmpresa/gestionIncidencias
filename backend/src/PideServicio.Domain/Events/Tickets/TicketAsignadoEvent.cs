namespace PideServicio.Domain.Events.Tickets;

public sealed record TicketAsignadoEvent(
    Guid TicketId,
    string Codigo,
    Guid EmpresaId,
    Guid TecnicoId,
    Guid AsignadorId,
    bool EsReasignacion,
    Guid? TecnicoAnteriorId,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
