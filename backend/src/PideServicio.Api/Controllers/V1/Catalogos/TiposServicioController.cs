namespace PideServicio.Api.Controllers.V1.Catalogos;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.TiposServicio.Commands.ActivarTipoServicio;
using PideServicio.Application.Features.TiposServicio.Commands.CreateTipoServicio;
using PideServicio.Application.Features.TiposServicio.Commands.DesactivarTipoServicio;
using PideServicio.Application.Features.TiposServicio.Commands.UpdateTipoServicio;
using PideServicio.Application.Features.TiposServicio.DTOs;
using PideServicio.Application.Features.TiposServicio.Queries.GetTipoServicioById;
using PideServicio.Application.Features.TiposServicio.Queries.ListTiposServicio;
using PideServicio.Contracts.Common;

/// <summary>Catálogo de tipos de servicio por empresa.</summary>
[ApiVersion("1.0")]
[Tags("Catálogos")]
[Route("api/v{version:apiVersion}/tipos-servicio")]
public sealed class TiposServicioController : ApiControllerBase
{
    /// <summary>Lista tipos de servicio con paginación, filtros y búsqueda.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<TipoServicioResumenDto>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] Guid? empresaId,
        [FromQuery] bool? soloActivos,
        [FromQuery] string? busqueda,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new ListTiposServicioQuery(empresaId, soloActivos, busqueda, pagina, tamanoPagina), ct);
        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene un tipo de servicio por su identificador.</summary>
    [HttpGet("{id:guid}", Name = "GetTipoServicioById")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<TipoServicioDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetTipoServicioByIdQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Crea un tipo de servicio. Admin o SuperAdmin.</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Crear([FromBody] CreateTipoServicioRequest request, CancellationToken ct)
    {
        var command = new CreateTipoServicioCommand(request.EmpresaId, request.Nombre, request.Orden, request.Descripcion);
        var result = await Mediator.Send(command, ct);
        return HandleCreated(result, "GetTipoServicioById", new { id = result.Valor });
    }

    /// <summary>Actualiza un tipo de servicio. Admin o SuperAdmin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] UpdateTipoServicioRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new UpdateTipoServicioCommand(id, request.Nombre, request.Orden, request.Descripcion), ct);
        return HandleResult(result);
    }

    /// <summary>Activa un tipo de servicio. Admin o SuperAdmin.</summary>
    [HttpPatch("{id:guid}/activar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new ActivarTipoServicioCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Desactiva un tipo de servicio. Admin o SuperAdmin.</summary>
    [HttpPatch("{id:guid}/desactivar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Desactivar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new DesactivarTipoServicioCommand(id), ct);
        return HandleResult(result);
    }
}

public sealed record CreateTipoServicioRequest(Guid EmpresaId, string Nombre, int Orden, string? Descripcion);
public sealed record UpdateTipoServicioRequest(string Nombre, int Orden, string? Descripcion);
