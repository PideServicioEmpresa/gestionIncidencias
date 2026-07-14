namespace PideServicio.Domain.Events.Tickets;

using PideServicio.Domain.Enums;

public sealed record TicketCreadoEvent(
    Guid TicketId,
    string Codigo,
    Guid EmpresaId,
    Guid SucursalId,
    Guid AreaId,
    Guid SolicitanteId,
    PrioridadTipo PrioridadSolicitante,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
