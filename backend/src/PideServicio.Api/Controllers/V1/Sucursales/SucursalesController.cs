namespace PideServicio.Api.Controllers.V1.Sucursales;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Sucursales.Commands.ActivarSucursal;
using PideServicio.Application.Features.Sucursales.Commands.CreateSucursal;
using PideServicio.Application.Features.Sucursales.Commands.DesactivarSucursal;
using PideServicio.Application.Features.Sucursales.Commands.UpdateSucursal;
using PideServicio.Application.Features.Sucursales.DTOs;
using PideServicio.Application.Features.Sucursales.Queries.GetSucursalById;
using PideServicio.Application.Features.Sucursales.Queries.ListSucursales;
using PideServicio.Contracts.Common;

/// <summary>Gestión de sucursales por empresa.</summary>
[ApiVersion("1.0")]
[Tags("Sucursales")]
public sealed class SucursalesController : ApiControllerBase
{
    /// <summary>Lista sucursales con paginación, filtros y búsqueda por texto.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<SucursalResumenDto>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] Guid? empresaId,
        [FromQuery] bool? soloActivas,
        [FromQuery] string? busqueda,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new ListSucursalesQuery(empresaId, pagina, tamanoPagina, soloActivas, busqueda), ct);
        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene una sucursal por su identificador.</summary>
    [HttpGet("{id:guid}", Name = "GetSucursalById")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<SucursalDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetSucursalByIdQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Crea una nueva sucursal dentro de una empresa. Admin o SuperAdmin.</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Crear([FromBody] CreateSucursalRequest request, CancellationToken ct)
    {
        var command = new CreateSucursalCommand(
            request.EmpresaId,
            request.Nombre,
            request.Descripcion,
            request.Direccion,
            request.ResponsableId);
        var result = await Mediator.Send(command, ct);
        return HandleCreated(result, "GetSucursalById", new { id = result.Valor });
    }

    /// <summary>Actualiza los datos de una sucursal. Admin o SuperAdmin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] UpdateSucursalRequest request,
        CancellationToken ct)
    {
        var command = new UpdateSucursalCommand(
            id,
            request.Nombre,
            request.Descripcion,
            request.Direccion,
            request.ResponsableId);
        var result = await Mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>Activa una sucursal. Admin o SuperAdmin.</summary>
    [HttpPatch("{id:guid}/activar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new ActivarSucursalCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Desactiva una sucursal. Admin o SuperAdmin. No se puede desactivar la última activa.</summary>
    [HttpPatch("{id:guid}/desactivar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Desactivar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new DesactivarSucursalCommand(id), ct);
        return HandleResult(result);
    }
}

// ---------------------------------------------------------------------------
// Request records (contratos de entrada del controlador)
// ---------------------------------------------------------------------------

public sealed record CreateSucursalRequest(
    Guid EmpresaId,
    string Nombre,
    string? Descripcion,
    string? Direccion,
    Guid? ResponsableId);

public sealed record UpdateSucursalRequest(
    string Nombre,
    string? Descripcion,
    string? Direccion,
    Guid? ResponsableId);
