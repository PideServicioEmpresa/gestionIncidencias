namespace PideServicio.Application.Features.Empresas.Queries.ListEmpresas;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Empresas.DTOs;

public sealed record ListEmpresasQuery(
    int Pagina = 1,
    int TamanoPagina = 20,
    bool? SoloActivas = null,
    string? Busqueda = null
) : IQuery<PagedResult<EmpresaResumenDto>>;
