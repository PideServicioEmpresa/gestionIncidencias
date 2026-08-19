namespace PideServicio.Application.Features.Especialidades.Queries.ListEspecialidades;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Especialidades.DTOs;

public sealed record ListEspecialidadesQuery(
    Guid? EmpresaId = null,
    bool? SoloActivas = null,
    string? Busqueda = null,
    int Pagina = 1,
    int TamanoPagina = 20
) : IQuery<PagedResult<EspecialidadResumenDto>>;
