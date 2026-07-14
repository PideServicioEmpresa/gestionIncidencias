namespace PideServicio.Application.Features.TiposServicio.Queries.ListTiposServicio;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.TiposServicio.DTOs;

public sealed record ListTiposServicioQuery(
    Guid? EmpresaId = null,
    bool? SoloActivos = null,
    string? Busqueda = null,
    int Pagina = 1,
    int TamanoPagina = 20
) : IQuery<PagedResult<TipoServicioResumenDto>>;
