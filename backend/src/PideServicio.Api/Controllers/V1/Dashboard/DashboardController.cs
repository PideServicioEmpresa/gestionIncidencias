namespace PideServicio.Api.Controllers.V1.Dashboard;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Dashboard.DTOs;
using PideServicio.Application.Features.Dashboard.Queries.GetDashboardResumen;
using PideServicio.Application.Features.Dashboard.Queries.GetTicketsReporte;

[ApiVersion("1.0")]
[Tags("Dashboard")]
public sealed class DashboardController : ApiControllerBase
{
    /// <summary>Retorna el resumen analítico del dashboard con KPIs, tendencias y distribuciones.</summary>
    [HttpGet("resumen")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(DashboardResumenDto), 200)]
    public async Task<IActionResult> Resumen(
        [FromQuery] Guid? empresaId,
        [FromQuery] Guid? sucursalId,
        [FromQuery] Guid? areaId,
        [FromQuery] string? desde,
        [FromQuery] string? hasta,
        CancellationToken ct)
        => HandleResult(await Mediator.Send(new GetDashboardResumenQuery(empresaId, sucursalId, areaId, desde, hasta), ct));

    /// <summary>Retorna hasta 1 000 tickets individuales con los filtros aplicados, para exportación a Excel.</summary>
    [HttpGet("tickets-reporte")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(IReadOnlyList<TicketReporteItemDto>), 200)]
    public async Task<IActionResult> TicketsReporte(
        [FromQuery] Guid? empresaId,
        [FromQuery] Guid? sucursalId,
        [FromQuery] string? desde,
        [FromQuery] string? hasta,
        CancellationToken ct)
        => HandleResult(await Mediator.Send(new GetTicketsReporteQuery(empresaId, sucursalId, desde, hasta), ct));
}
