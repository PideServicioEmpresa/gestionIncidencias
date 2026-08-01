namespace PideServicio.Application.Features.Dashboard.Queries.GetTicketsReporte;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Dashboard.DTOs;

public sealed record GetTicketsReporteQuery(
    Guid? EmpresaId,
    Guid? SucursalId,
    string? FechaDesde,
    string? FechaHasta) : IQuery<IReadOnlyList<TicketReporteItemDto>>;
