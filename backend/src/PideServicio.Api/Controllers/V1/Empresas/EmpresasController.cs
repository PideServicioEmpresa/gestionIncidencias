namespace PideServicio.Api.Controllers.V1.Empresas;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Empresas.Commands.ActivarEmpresa;
using PideServicio.Application.Features.Empresas.Commands.CreateEmpresa;
using PideServicio.Application.Features.Empresas.Commands.DesactivarEmpresa;
using PideServicio.Application.Features.Empresas.Commands.UpdateEmpresa;
using PideServicio.Application.Features.Empresas.DTOs;
using PideServicio.Application.Features.Empresas.Queries.GetEmpresaById;
using PideServicio.Application.Features.Empresas.Queries.ListEmpresas;
using PideServicio.Contracts.Common;

/// <summary>Gestión de empresas del sistema.</summary>
[ApiVersion("1.0")]
[Tags("Empresas")]
public sealed class EmpresasController : ApiControllerBase
{
    /// <summary>Lista empresas con paginación, filtro de estado y búsqueda por texto.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<EmpresaResumenDto>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] bool? soloActivas,
        [FromQuery] string? busqueda,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new ListEmpresasQuery(pagina, tamanoPagina, soloActivas, busqueda), ct);
        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene una empresa por su identificador.</summary>
    [HttpGet("{id:guid}", Name = "GetEmpresaById")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<EmpresaDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetEmpresaByIdQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Crea una nueva empresa. Solo SuperAdmin.</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Crear([FromBody] CreateEmpresaRequest request, CancellationToken ct)
    {
        var command = new CreateEmpresaCommand(
            request.NombreComercial,
            request.RazonSocial,
            request.IdentificacionFiscal,
            request.ZonaHoraria,
            request.LogoUrl,
            request.ColorPrimario,
            request.ColorSecundario);
        var result = await Mediator.Send(command, ct);
        return HandleCreated(result, "GetEmpresaById", new { id = result.Valor });
    }

    /// <summary>Actualiza los datos de una empresa. Solo SuperAdmin.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] UpdateEmpresaRequest request,
        CancellationToken ct)
    {
        var command = new UpdateEmpresaCommand(
            id,
            request.NombreComercial,
            request.RazonSocial,
            request.ZonaHoraria,
            request.LogoUrl,
            request.ColorPrimario,
            request.ColorSecundario);
        var result = await Mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>Activa una empresa. Solo SuperAdmin.</summary>
    [HttpPatch("{id:guid}/activar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new ActivarEmpresaCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Desactiva una empresa. Solo SuperAdmin.</summary>
    [HttpPatch("{id:guid}/desactivar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Desactivar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new DesactivarEmpresaCommand(id), ct);
        return HandleResult(result);
    }
}

// ---------------------------------------------------------------------------
// Request records (contratos de entrada del controlador)
// ---------------------------------------------------------------------------

public sealed record CreateEmpresaRequest(
    string NombreComercial,
    string RazonSocial,
    string IdentificacionFiscal,
    string ZonaHoraria,
    string? LogoUrl,
    string? ColorPrimario,
    string? ColorSecundario);

public sealed record UpdateEmpresaRequest(
    string NombreComercial,
    string RazonSocial,
    string ZonaHoraria,
    string? LogoUrl,
    string? ColorPrimario,
    string? ColorSecundario);
