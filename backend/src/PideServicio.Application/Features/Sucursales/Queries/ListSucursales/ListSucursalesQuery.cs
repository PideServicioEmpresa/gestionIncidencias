namespace PideServicio.Application.Features.Sucursales.Queries.ListSucursales;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Sucursales.DTOs;

public sealed record ListSucursalesQuery(
    Guid? EmpresaId = null,
    int Pagina = 1,
    int TamanoPagina = 20,
    bool? SoloActivas = null,
    string? Busqueda = null
) : IQuery<PagedResult<SucursalResumenDto>>;
