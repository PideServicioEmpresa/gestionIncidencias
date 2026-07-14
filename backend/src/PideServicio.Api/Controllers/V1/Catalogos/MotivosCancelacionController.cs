namespace PideServicio.Api.Controllers.V1.Catalogos;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.MotivosCancelacion.Commands.ActivarMotivoCancelacion;
using PideServicio.Application.Features.MotivosCancelacion.Commands.CreateMotivoCancelacion;
using PideServicio.Application.Features.MotivosCancelacion.Commands.DesactivarMotivoCancelacion;
using PideServicio.Application.Features.MotivosCancelacion.Commands.UpdateMotivoCancelacion;
using PideServicio.Application.Features.MotivosCancelacion.DTOs;
using PideServicio.Application.Features.MotivosCancelacion.Queries.GetMotivoCancelacionById;
using PideServicio.Application.Features.MotivosCancelacion.Queries.ListMotivosCancelacion;
using PideServicio.Contracts.Common;

/// <summary>Catálogo de motivos de cancelación globales y por empresa.</summary>
[ApiVersion("1.0")]
[Tags("Catálogos")]
[Route("api/v{version:apiVersion}/motivos-cancelacion")]
public sealed class MotivosCancelacionController : ApiControllerBase
{
    /// <summary>Lista motivos de cancelación con paginación, filtros y búsqueda.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<MotivoCancelacionResumenDto>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] Guid? empresaId,
        [FromQuery] bool? soloActivos,
        [FromQuery] string? busqueda,
        [FromQuery] bool soloGlobales = false,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new ListMotivosCancelacionQuery(empresaId, soloActivos, busqueda, soloGlobales, pagina, tamanoPagina), ct);
        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene un motivo de cancelación por su identificador.</summary>
    [HttpGet("{id:guid}", Name = "GetMotivoCancelacionById")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<MotivoCancelacionDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetMotivoCancelacionByIdQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Crea un motivo de cancelación. Admin o SuperAdmin.</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Crear([FromBody] CreateMotivoCancelacionRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new CreateMotivoCancelacionCommand(request.Texto, request.EmpresaId), ct);
        return HandleCreated(result, "GetMotivoCancelacionById", new { id = result.Valor });
    }

    /// <summary>Actualiza un motivo de cancelación. Admin o SuperAdmin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] UpdateMotivoCancelacionRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new UpdateMotivoCancelacionCommand(id, request.Texto), ct);
        return HandleResult(result);
    }

    /// <summary>Activa un motivo de cancelación. Admin o SuperAdmin.</summary>
    [HttpPatch("{id:guid}/activar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new ActivarMotivoCancelacionCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Desactiva un motivo de cancelación. Admin o SuperAdmin.</summary>
    [HttpPatch("{id:guid}/desactivar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Desactivar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new DesactivarMotivoCancelacionCommand(id), ct);
        return HandleResult(result);
    }
}

public sealed record CreateMotivoCancelacionRequest(string Texto, Guid? EmpresaId);
public sealed record UpdateMotivoCancelacionRequest(string Texto);
