namespace PideServicio.Domain.Events.Tickets;

public sealed record TicketReabiertoEvent(
    Guid TicketId,
    string Codigo,
    Guid EmpresaId,
    Guid ActorId,
    Guid MotivoRechazoId,
    string? ComentarioRechazo,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
