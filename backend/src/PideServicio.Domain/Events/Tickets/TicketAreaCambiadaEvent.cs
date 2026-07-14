namespace PideServicio.Domain.Events.Tickets;

public sealed record TicketAreaCambiadaEvent(
    Guid TicketId,
    string Codigo,
    Guid EmpresaId,
    Guid AreaAnteriorId,
    Guid AreaNuevaId,
    Guid ActorId,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
