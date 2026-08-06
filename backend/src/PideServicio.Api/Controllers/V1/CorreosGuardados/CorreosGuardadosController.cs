namespace PideServicio.Api.Controllers.V1.CorreosGuardados;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.CorreosGuardados.Commands.AgregarCorreoGuardado;
using PideServicio.Application.Features.CorreosGuardados.Commands.EliminarCorreoGuardado;
using PideServicio.Application.Features.CorreosGuardados.DTOs;
using PideServicio.Application.Features.CorreosGuardados.Queries.ListarCorreosGuardados;
using PideServicio.Contracts.Common;

/// <summary>Correos frecuentes guardados por el usuario para reutilizar en tickets.</summary>
[ApiVersion("1.0")]
[Tags("CorreosGuardados")]
public sealed class CorreosGuardadosController : ApiControllerBase
{
    /// <summary>Lista los correos guardados del usuario autenticado.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CorreoGuardadoDto>>), 200)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await Mediator.Send(new ListarCorreosGuardadosQuery(), ct);
        return HandleResult(result);
    }

    /// <summary>Agrega un correo a la lista guardada del usuario.</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<CorreoGuardadoDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Agregar([FromBody] AgregarCorreoRequest body, CancellationToken ct)
    {
        var result = await Mediator.Send(new AgregarCorreoGuardadoCommand(body.Correo), ct);
        return HandleCreated(result);
    }

    /// <summary>Elimina un correo guardado del usuario.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new EliminarCorreoGuardadoCommand(id), ct);
        return HandleResult(result);
    }
}

public sealed record AgregarCorreoRequest(string Correo);
