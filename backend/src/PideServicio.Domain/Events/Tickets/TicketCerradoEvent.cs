namespace PideServicio.Domain.Events.Tickets;

public sealed record TicketCerradoEvent(
    Guid TicketId,
    string Codigo,
    Guid EmpresaId,
    Guid ActorId,
    byte? Valoracion,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
