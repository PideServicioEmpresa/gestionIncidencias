namespace PideServicio.Api.Controllers.V1.Configuracion;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Configuracion.Commands.UpdateParametro;
using PideServicio.Application.Features.Configuracion.DTOs;
using PideServicio.Application.Features.Configuracion.Queries.GetParametroPorClave;
using PideServicio.Application.Features.Configuracion.Queries.GetParametros;
using PideServicio.Contracts.Common;

/// <summary>Gestión de parámetros de configuración del sistema y por empresa.</summary>
[ApiVersion("1.0")]
[Tags("Configuración")]
public sealed class ConfiguracionController : ApiControllerBase
{
    /// <summary>Devuelve todos los parámetros de configuración aplicables.</summary>
    [HttpGet]
    [Authorize(Policy = "AdminOSuperior")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ParametroDto>>), 200)]
    public async Task<IActionResult> ObtenerParametros(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetParametrosQuery(), ct);
        return HandleResult(result);
    }

    /// <summary>Obtiene un parámetro de configuración por su clave.</summary>
    [HttpGet("{clave}")]
    [Authorize(Policy = "AdminOSuperior")]
    [ProducesResponseType(typeof(ApiResponse<ParametroDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> ObtenerPorClave(string clave, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetParametroPorClaveQuery(clave), ct);
        return HandleResult(result);
    }

    /// <summary>Actualiza el valor de un parámetro de configuración.</summary>
    [HttpPut("{clave}")]
    [Authorize(Policy = "AdminOSuperior")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Actualizar(
        string clave,
        [FromBody] UpdateParametroRequest request,
        CancellationToken ct)
    {
        var command = new UpdateParametroCommand(clave, request.NuevoValor, request.EmpresaId);
        var result = await Mediator.Send(command, ct);
        return HandleResult(result);
    }
}

// ---------------------------------------------------------------------------
// Request records (contratos de entrada del controlador)
// ---------------------------------------------------------------------------

public sealed record UpdateParametroRequest(string NuevoValor, Guid? EmpresaId);
