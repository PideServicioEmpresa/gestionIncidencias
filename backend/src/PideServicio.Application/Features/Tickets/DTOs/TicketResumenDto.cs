namespace PideServicio.Application.Features.Tickets.DTOs;

public sealed record TicketResumenDto(
    Guid Id,
    string Codigo,
    string Titulo,
    string Estado,
    string PrioridadEfectiva,
    Guid EmpresaId,
    Guid SucursalId,
    Guid AreaId,
    Guid TipoServicioId,
    Guid SolicitanteId,
    Guid? TecnicoId,
    DateTimeOffset FechaCreacion,
    DateTimeOffset? FechaLimiteResolucion);
