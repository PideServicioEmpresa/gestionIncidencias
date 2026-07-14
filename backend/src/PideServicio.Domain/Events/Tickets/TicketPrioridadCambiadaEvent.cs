namespace PideServicio.Domain.Events.Tickets;

using PideServicio.Domain.Enums;

public sealed record TicketPrioridadCambiadaEvent(
    Guid TicketId,
    string Codigo,
    Guid EmpresaId,
    PrioridadTipo PrioridadAnterior,
    PrioridadTipo PrioridadNueva,
    Guid ActorId,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
