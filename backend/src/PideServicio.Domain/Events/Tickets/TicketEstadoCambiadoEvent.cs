namespace PideServicio.Domain.Events.Tickets;

using PideServicio.Domain.Enums;

public sealed record TicketEstadoCambiadoEvent(
    Guid TicketId,
    string Codigo,
    Guid EmpresaId,
    TicketEstadoTipo EstadoAnterior,
    TicketEstadoTipo EstadoNuevo,
    Guid ActorId,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
