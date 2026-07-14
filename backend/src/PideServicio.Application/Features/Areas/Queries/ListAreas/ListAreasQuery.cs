namespace PideServicio.Application.Features.Areas.Queries.ListAreas;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Areas.DTOs;

public sealed record ListAreasQuery(
    Guid? SucursalId = null,
    Guid? EmpresaId = null,
    int Pagina = 1,
    int TamanoPagina = 20,
    bool? SoloActivas = null,
    string? Busqueda = null
) : IQuery<PagedResult<AreaResumenDto>>;
