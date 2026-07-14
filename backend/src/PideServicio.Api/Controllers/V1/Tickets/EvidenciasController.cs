namespace PideServicio.Api.Controllers.V1.Tickets;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Evidencias.Commands.EliminarEvidencia;
using PideServicio.Application.Features.Evidencias.Commands.SubirEvidencia;
using PideServicio.Application.Features.Evidencias.DTOs;
using PideServicio.Application.Features.Evidencias.Queries.ListEvidencias;
using PideServicio.Contracts.Common;
using PideServicio.Domain.Enums;

[ApiVersion("1.0")]
[Tags("Tickets")]
[Route("api/v{version:apiVersion}/tickets/{ticketId:guid}/evidencias")]
public sealed class EvidenciasController : ApiControllerBase
{
    /// <summary>Lista las evidencias adjuntas a un ticket, con filtro opcional por tipo.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EvidenciaDto>>), 200)]
    public async Task<IActionResult> Listar(
        Guid ticketId,
        [FromQuery] EvidenciaTipo? tipo,
        CancellationToken ct)
    {
        var result = await Mediator.Send(new ListEvidenciasQuery(ticketId, tipo), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Sube una evidencia (imagen, video, documento) al ticket.
    /// Acepta multipart/form-data. Validación de MIME, extensión y tamaño se realiza en la capa de Application.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<SubirEvidenciaResponse>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Subir(
        Guid ticketId,
        [FromForm] SubirEvidenciaRequest request,
        CancellationToken ct)
    {
        var archivo = request.Archivo;
        if (archivo is null || archivo.Length == 0)
            return BadRequest(ApiResponse.Fallo(
                new ApiError("ARCHIVO_REQUERIDO", "El archivo es requerido."), TraceId));

        await using var stream = archivo.OpenReadStream();

        var command = new SubirEvidenciaCommand(
            ticketId,
            request.Tipo,
            archivo.FileName,
            archivo.ContentType,
            archivo.Length,
            stream);

        var result = await Mediator.Send(command, ct);
        return HandleCreated(result);
    }

    /// <summary>Elimina lógicamente una evidencia. Solo el autor o Admin puede eliminarla.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Eliminar(Guid ticketId, Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new EliminarEvidenciaCommand(id), ct);
        return HandleResult(result);
    }
}

// ---------------------------------------------------------------------------
// Request records
// ---------------------------------------------------------------------------

/// <summary>Payload multipart para subir una evidencia a un ticket.</summary>
public sealed record SubirEvidenciaRequest(IFormFile? Archivo, EvidenciaTipo Tipo);
