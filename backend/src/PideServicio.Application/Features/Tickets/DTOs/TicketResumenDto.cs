namespace PideServicio.Application.Features.Tickets.DTOs;

public sealed record TicketResumenDto(
    Guid Id,
    string Codigo,
    string Titulo,
    string Estado,
    string PrioridadEfectiva,
    Guid EmpresaId,
    Guid SucursalId,
    string? SucursalNombre,
    Guid AreaId,
    string? AreaNombre,
    Guid TipoServicioId,
    string? Tipo,
    string? TipoServicioNombre,
    Guid? CategoriaId,
    Guid SolicitanteId,
    string? SolicitanteNombre,
    Guid? TecnicoId,
    string? AsignadoANombre,
    DateTimeOffset FechaCreacion,
    DateTimeOffset? FechaLimiteResolucion);
