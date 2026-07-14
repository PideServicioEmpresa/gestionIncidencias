namespace PideServicio.Api.Controllers.V1.Areas;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Areas.Commands.ActivarArea;
using PideServicio.Application.Features.Areas.Commands.CreateArea;
using PideServicio.Application.Features.Areas.Commands.DesactivarArea;
using PideServicio.Application.Features.Areas.Commands.UpdateArea;
using PideServicio.Application.Features.Areas.DTOs;
using PideServicio.Application.Features.Areas.Queries.GetAreaById;
using PideServicio.Application.Features.Areas.Queries.ListAreas;
using PideServicio.Contracts.Common;

/// <summary>Gestión de áreas por sucursal.</summary>
[ApiVersion("1.0")]
[Tags("Áreas")]
public sealed class AreasController : ApiControllerBase
{
    /// <summary>Lista áreas con paginación, filtros y búsqueda por texto.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<AreaResumenDto>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] Guid? sucursalId,
        [FromQuery] Guid? empresaId,
        [FromQuery] bool? soloActivas,
        [FromQuery] string? busqueda,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new ListAreasQuery(sucursalId, empresaId, pagina, tamanoPagina, soloActivas, busqueda), ct);
        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene un área por su identificador.</summary>
    [HttpGet("{id:guid}", Name = "GetAreaById")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<AreaDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetAreaByIdQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Crea un área nueva dentro de una sucursal. Admin o SuperAdmin.</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Crear([FromBody] CreateAreaRequest request, CancellationToken ct)
    {
        var command = new CreateAreaCommand(
            request.SucursalId,
            request.Nombre,
            request.Descripcion,
            request.ResponsableId);
        var result = await Mediator.Send(command, ct);
        return HandleCreated(result, "GetAreaById", new { id = result.Valor });
    }

    /// <summary>Actualiza los datos de un área. Admin o SuperAdmin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] UpdateAreaRequest request,
        CancellationToken ct)
    {
        var command = new UpdateAreaCommand(id, request.Nombre, request.Descripcion, request.ResponsableId);
        var result = await Mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>Activa un área. Admin o SuperAdmin.</summary>
    [HttpPatch("{id:guid}/activar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new ActivarAreaCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Desactiva un área. Admin o SuperAdmin.</summary>
    [HttpPatch("{id:guid}/desactivar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Desactivar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new DesactivarAreaCommand(id), ct);
        return HandleResult(result);
    }
}

// ---------------------------------------------------------------------------
// Request records (contratos de entrada del controlador)
// ---------------------------------------------------------------------------

public sealed record CreateAreaRequest(
    Guid SucursalId,
    string Nombre,
    string? Descripcion,
    Guid? ResponsableId);

public sealed record UpdateAreaRequest(
    string Nombre,
    string? Descripcion,
    Guid? ResponsableId);
