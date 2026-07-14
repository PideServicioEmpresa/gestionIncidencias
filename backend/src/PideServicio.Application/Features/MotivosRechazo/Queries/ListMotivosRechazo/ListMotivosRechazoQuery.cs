namespace PideServicio.Application.Features.MotivosRechazo.Queries.ListMotivosRechazo;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.MotivosRechazo.DTOs;

public sealed record ListMotivosRechazoQuery(
    Guid? EmpresaId = null,
    bool? SoloActivos = null,
    string? Busqueda = null,
    bool SoloGlobales = false,
    int Pagina = 1,
    int TamanoPagina = 20
) : IQuery<PagedResult<MotivoRechazoResumenDto>>;
