namespace PideServicio.Domain.Events.Tickets;

using PideServicio.Domain.Enums;

public sealed record TicketCanceladoEvent(
    Guid TicketId,
    string Codigo,
    Guid EmpresaId,
    Guid ActorId,
    Guid MotivoCancelacionId,
    TicketEstadoTipo EstadoAnterior,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
