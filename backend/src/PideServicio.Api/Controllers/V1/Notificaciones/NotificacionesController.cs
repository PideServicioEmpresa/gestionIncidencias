namespace PideServicio.Api.Controllers.V1.Notificaciones;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Notificaciones.Commands.MarcarNotificacionLeida;
using PideServicio.Application.Features.Notificaciones.Commands.MarcarTodasLeidas;
using PideServicio.Application.Features.Notificaciones.DTOs;
using PideServicio.Application.Features.Notificaciones.Queries.GetConteoNoLeidas;
using PideServicio.Application.Features.Notificaciones.Queries.ListNotificaciones;
using PideServicio.Contracts.Common;

/// <summary>Gestión de notificaciones del usuario autenticado.</summary>
[ApiVersion("1.0")]
[Tags("Notificaciones")]
public sealed class NotificacionesController : ApiControllerBase
{
    /// <summary>Lista las notificaciones del usuario autenticado con paginación.</summary>
    /// <remarks>
    /// Si <paramref name="soloNoLeidas"/> es <c>true</c>, devuelve únicamente las no leídas.
    /// El resultado está ordenado por fecha de creación descendente.
    /// </remarks>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<NotificacionDto>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] bool? soloNoLeidas,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new ListNotificacionesQuery(soloNoLeidas, pagina, tamanoPagina), ct);

        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene el conteo de notificaciones no leídas del usuario autenticado.</summary>
    [HttpGet("conteo")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<ConteoNotificacionesDto>), 200)]
    public async Task<IActionResult> ObtenerConteo(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetConteoNoLeidasQuery(), ct);
        return HandleResult(result);
    }

    /// <summary>Marca una notificación específica como leída.</summary>
    [HttpPatch("{id:guid}/leida")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> MarcarLeida(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new MarcarNotificacionLeidaCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Marca todas las notificaciones del usuario autenticado como leídas.</summary>
    [HttpPatch("marcar-todas-leidas")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> MarcarTodasLeidas(CancellationToken ct)
    {
        var result = await Mediator.Send(new MarcarTodasLeidasCommand(), ct);
        return HandleResult(result);
    }
}
