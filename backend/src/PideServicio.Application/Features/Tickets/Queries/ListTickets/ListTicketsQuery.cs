namespace PideServicio.Application.Features.Tickets.Queries.ListTickets;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Tickets.DTOs;
using PideServicio.Domain.Enums;

public sealed record ListTicketsQuery(
    Guid? SucursalId = null,
    Guid? AreaId = null,
    Guid? TecnicoId = null,
    Guid? SolicitanteId = null,
    TicketEstadoTipo? Estado = null,
    PrioridadTipo? Prioridad = null,
    DateTimeOffset? FechaDesde = null,
    DateTimeOffset? FechaHasta = null,
    string? BusquedaTexto = null,
    int Pagina = 1,
    int TamanoPagina = 20
) : IQuery<PagedResult<TicketResumenDto>>;
