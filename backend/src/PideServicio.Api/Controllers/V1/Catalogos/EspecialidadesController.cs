namespace PideServicio.Api.Controllers.V1.Catalogos;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Especialidades.Commands.ActivarEspecialidad;
using PideServicio.Application.Features.Especialidades.Commands.CreateEspecialidad;
using PideServicio.Application.Features.Especialidades.Commands.DesactivarEspecialidad;
using PideServicio.Application.Features.Especialidades.Commands.UpdateEspecialidad;
using PideServicio.Application.Features.Especialidades.DTOs;
using PideServicio.Application.Features.Especialidades.Queries.GetEspecialidadById;
using PideServicio.Application.Features.Especialidades.Queries.ListEspecialidades;
using PideServicio.Contracts.Common;

/// <summary>Catálogo de especialidades (globales o por empresa).</summary>
[ApiVersion("1.0")]
[Tags("Catálogos")]
[Route("api/v{version:apiVersion}/especialidades")]
public sealed class EspecialidadesController : ApiControllerBase
{
    /// <summary>Lista especialidades con paginación, filtros y búsqueda.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<EspecialidadResumenDto>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] Guid? empresaId,
        [FromQuery] bool? soloActivas,
        [FromQuery] string? busqueda,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new ListEspecialidadesQuery(empresaId, soloActivas, busqueda, pagina, tamanoPagina), ct);
        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene una especialidad por su identificador.</summary>
    [HttpGet("{id:guid}", Name = "GetEspecialidadById")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<EspecialidadDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetEspecialidadByIdQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Crea una especialidad. Admin o SuperAdmin (global solo SuperAdmin).</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Crear([FromBody] CreateEspecialidadRequest request, CancellationToken ct)
    {
        var command = new CreateEspecialidadCommand(request.EmpresaId, request.Nombre, request.Descripcion);
        var result = await Mediator.Send(command, ct);
        return HandleCreated(result, "GetEspecialidadById", new { id = result.Valor });
    }

    /// <summary>Actualiza una especialidad. Admin o SuperAdmin (global solo SuperAdmin).</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] UpdateEspecialidadRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new UpdateEspecialidadCommand(id, request.Nombre, request.Descripcion), ct);
        return HandleResult(result);
    }

    /// <summary>Activa una especialidad. Admin o SuperAdmin (global solo SuperAdmin).</summary>
    [HttpPatch("{id:guid}/activar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new ActivarEspecialidadCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Desactiva una especialidad. Admin o SuperAdmin (global solo SuperAdmin).</summary>
    [HttpPatch("{id:guid}/desactivar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Desactivar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new DesactivarEspecialidadCommand(id), ct);
        return HandleResult(result);
    }
}

public sealed record CreateEspecialidadRequest(Guid? EmpresaId, string Nombre, string? Descripcion);
public sealed record UpdateEspecialidadRequest(string Nombre, string? Descripcion);
