namespace PideServicio.Api.Controllers.V1.Catalogos;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.MotivosRechazo.Commands.ActivarMotivoRechazo;
using PideServicio.Application.Features.MotivosRechazo.Commands.CreateMotivoRechazo;
using PideServicio.Application.Features.MotivosRechazo.Commands.DesactivarMotivoRechazo;
using PideServicio.Application.Features.MotivosRechazo.Commands.UpdateMotivoRechazo;
using PideServicio.Application.Features.MotivosRechazo.DTOs;
using PideServicio.Application.Features.MotivosRechazo.Queries.GetMotivoRechazoById;
using PideServicio.Application.Features.MotivosRechazo.Queries.ListMotivosRechazo;
using PideServicio.Contracts.Common;

/// <summary>Catálogo de motivos de rechazo globales y por empresa.</summary>
[ApiVersion("1.0")]
[Tags("Catálogos")]
[Route("api/v{version:apiVersion}/motivos-rechazo")]
public sealed class MotivosRechazoController : ApiControllerBase
{
    /// <summary>Lista motivos de rechazo con paginación, filtros y búsqueda.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<MotivoRechazoResumenDto>), 200)]
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
            new ListMotivosRechazoQuery(empresaId, soloActivos, busqueda, soloGlobales, pagina, tamanoPagina), ct);
        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene un motivo de rechazo por su identificador.</summary>
    [HttpGet("{id:guid}", Name = "GetMotivoRechazoById")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<MotivoRechazoDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetMotivoRechazoByIdQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Crea un motivo de rechazo. Admin o SuperAdmin.</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Crear([FromBody] CreateMotivoRechazoRequest request, CancellationToken ct)
    {
        var command = new CreateMotivoRechazoCommand(
            request.Codigo, request.Nombre, request.Orden, request.EsOtro, request.Descripcion, request.EmpresaId);
        var result = await Mediator.Send(command, ct);
        return HandleCreated(result, "GetMotivoRechazoById", new { id = result.Valor });
    }

    /// <summary>Actualiza un motivo de rechazo. Admin o SuperAdmin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] UpdateMotivoRechazoRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new UpdateMotivoRechazoCommand(id, request.Nombre, request.Codigo, request.Orden, request.Descripcion), ct);
        return HandleResult(result);
    }

    /// <summary>Activa un motivo de rechazo. Admin o SuperAdmin.</summary>
    [HttpPatch("{id:guid}/activar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new ActivarMotivoRechazoCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Desactiva un motivo de rechazo. Admin o SuperAdmin.</summary>
    [HttpPatch("{id:guid}/desactivar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Desactivar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new DesactivarMotivoRechazoCommand(id), ct);
        return HandleResult(result);
    }
}

public sealed record CreateMotivoRechazoRequest(
    string Codigo, string Nombre, int Orden, bool EsOtro, string? Descripcion, Guid? EmpresaId);

public sealed record UpdateMotivoRechazoRequest(
    string Nombre, string Codigo, int Orden, string? Descripcion);
