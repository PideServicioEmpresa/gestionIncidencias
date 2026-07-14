namespace PideServicio.Application.Features.Categorias.Queries.ListCategorias;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Categorias.DTOs;

public sealed record ListCategoriasQuery(
    Guid? EmpresaId = null,
    bool? SoloActivas = null,
    string? Busqueda = null,
    bool SoloGlobales = false,
    int Pagina = 1,
    int TamanoPagina = 20
) : IQuery<PagedResult<CategoriaResumenDto>>;
