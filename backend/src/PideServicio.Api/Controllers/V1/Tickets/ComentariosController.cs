namespace PideServicio.Api.Controllers.V1.Tickets;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Comentarios.Commands.CrearComentario;
using PideServicio.Application.Features.Comentarios.Commands.EditarComentario;
using PideServicio.Application.Features.Comentarios.Commands.EliminarComentario;
using PideServicio.Application.Features.Comentarios.DTOs;
using PideServicio.Application.Features.Comentarios.Queries.ListComentarios;
using PideServicio.Contracts.Common;

[ApiVersion("1.0")]
[Tags("Tickets")]
[Route("api/v{version:apiVersion}/tickets/{ticketId:guid}/comentarios")]
public sealed class ComentariosController : ApiControllerBase
{
    /// <summary>Lista los comentarios de un ticket. Los comentarios internos solo los ven roles con permiso.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ComentarioDto>>), 200)]
    public async Task<IActionResult> Listar(Guid ticketId, CancellationToken ct)
    {
        var result = await Mediator.Send(new ListComentariosQuery(ticketId), ct);
        return HandleResult(result);
    }

    /// <summary>Agrega un comentario a un ticket. Un comentario interno solo es visible para el equipo.</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Crear(
        Guid ticketId,
        [FromBody] CrearComentarioRequest request,
        CancellationToken ct)
    {
        var result = await Mediator.Send(
            new CrearComentarioCommand(ticketId, request.Cuerpo, request.EsInterno), ct);
        return HandleCreated(result);
    }

    /// <summary>Edita el cuerpo de un comentario existente. Solo el autor puede editar el suyo.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Editar(
        Guid ticketId,
        Guid id,
        [FromBody] EditarComentarioRequest request,
        CancellationToken ct)
    {
        var result = await Mediator.Send(new EditarComentarioCommand(id, request.NuevoCuerpo), ct);
        return HandleResult(result);
    }

    /// <summary>Elimina lógicamente un comentario. Solo el autor o Admin puede eliminarlo.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Eliminar(Guid ticketId, Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new EliminarComentarioCommand(id), ct);
        return HandleResult(result);
    }
}

// ---------------------------------------------------------------------------
// Request records
// ---------------------------------------------------------------------------

/// <summary>Payload para crear un comentario en un ticket.</summary>
public sealed record CrearComentarioRequest(string Cuerpo, bool EsInterno = false);

/// <summary>Payload para editar el cuerpo de un comentario.</summary>
public sealed record EditarComentarioRequest(string NuevoCuerpo);
