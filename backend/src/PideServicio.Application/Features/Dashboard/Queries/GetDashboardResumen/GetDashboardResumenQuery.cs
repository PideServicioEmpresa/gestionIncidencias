namespace PideServicio.Application.Features.Dashboard.Queries.GetDashboardResumen;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Dashboard.DTOs;

public sealed record GetDashboardResumenQuery(Guid? EmpresaId, Guid? SucursalId) : IQuery<DashboardResumenDto>;
