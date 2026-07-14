namespace PideServicio.Application.Features.MotivosCancelacion.Queries.ListMotivosCancelacion;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.MotivosCancelacion.DTOs;

public sealed record ListMotivosCancelacionQuery(
    Guid? EmpresaId = null,
    bool? SoloActivos = null,
    string? Busqueda = null,
    bool SoloGlobales = false,
    int Pagina = 1,
    int TamanoPagina = 20
) : IQuery<PagedResult<MotivoCancelacionResumenDto>>;
