namespace PideServicio.Api.Controllers.V1.Tickets;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Tickets.Commands.AsignarTicket;
using PideServicio.Application.Features.Tickets.Commands.CambiarArea;
using PideServicio.Application.Features.Tickets.Commands.CambiarPrioridad;
using PideServicio.Application.Features.Tickets.Commands.CancelarTicket;
using PideServicio.Application.Features.Tickets.Commands.CerrarTicket;
using PideServicio.Application.Features.Tickets.Commands.CrearTicket;
using PideServicio.Application.Features.Tickets.Commands.IniciarProceso;
using PideServicio.Application.Features.Tickets.Commands.PauseParaEspera;
using PideServicio.Application.Features.Tickets.Commands.ReanudarDesdeEspera;
using PideServicio.Application.Features.Tickets.Commands.ReabrirTicket;
using PideServicio.Application.Features.Tickets.Commands.ReasignarTicket;
using PideServicio.Application.Features.Tickets.Commands.SubmitParaValidacion;
using PideServicio.Application.Features.Tickets.DTOs;
using PideServicio.Application.Features.Tickets.Queries.GetTicketById;
using PideServicio.Application.Features.Tickets.Queries.GetTicketHistorial;
using PideServicio.Application.Features.Tickets.Queries.ListTickets;
using PideServicio.Contracts.Common;
using PideServicio.Domain.Enums;

[ApiVersion("1.0")]
[Tags("Tickets")]
public sealed class TicketsController : ApiControllerBase
{
    /// <summary>Lista tickets con paginación y filtros opcionales.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<TicketResumenDto>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] Guid? sucursalId,
        [FromQuery] Guid? areaId,
        [FromQuery] Guid? tecnicoId,
        [FromQuery] Guid? solicitanteId,
        [FromQuery] TicketEstadoTipo? estado,
        [FromQuery] PrioridadTipo? prioridad,
        [FromQuery] DateTimeOffset? fechaDesde,
        [FromQuery] DateTimeOffset? fechaHasta,
        [FromQuery] string? busqueda,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new ListTicketsQuery(sucursalId, areaId, tecnicoId, solicitanteId,
                estado, prioridad, fechaDesde, fechaHasta, busqueda, pagina, tamanoPagina), ct);
        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene el detalle completo de un ticket por su identificador.</summary>
    [HttpGet("{id:guid}", Name = "GetTicketById")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<TicketDetalleDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetTicketByIdQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Obtiene el historial de transiciones de estado de un ticket.</summary>
    [HttpGet("{id:guid}/historial")]
    [Authorize(Policy = "Autenticado")]
    public async Task<IActionResult> ObtenerHistorial(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetTicketHistorialQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Crea un nuevo ticket. Cualquier usuario autenticado puede abrir un ticket.</summary>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Crear([FromBody] CrearTicketRequest request, CancellationToken ct)
    {
        var command = new CrearTicketCommand(
            request.Titulo,
            request.Descripcion,
            request.SucursalId,
            request.AreaNombre,
            request.TipoServicioId,
            request.CategoriaId,
            request.Prioridad,
            request.Ubicacion);

        var result = await Mediator.Send(command, ct);
        return HandleCreated(result, "GetTicketById", new { id = result.Valor });
    }

    /// <summary>Asigna un ticket a un técnico. Requiere rol Supervisor o superior.</summary>
    [HttpPost("{id:guid}/asignar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Asignar(Guid id, [FromBody] AsignarTicketRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new AsignarTicketCommand(id, request.TecnicoId), ct);
        return HandleResult(result);
    }

    /// <summary>Reasigna un ticket a otro técnico con motivo opcional. Requiere rol Admin o superior.</summary>
    [HttpPost("{id:guid}/reasignar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Reasignar(Guid id, [FromBody] ReasignarTicketRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new ReasignarTicketCommand(id, request.NuevoTecnicoId, request.Motivo), ct);
        return HandleResult(result);
    }

    /// <summary>Inicia el proceso de un ticket asignado. Solo el técnico asignado.</summary>
    [HttpPatch("{id:guid}/iniciar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> IniciarProceso(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new IniciarProcesoCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Pausa el ticket dejándolo en espera. Solo el técnico asignado.</summary>
    [HttpPatch("{id:guid}/pausar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> PauseParaEspera(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new PauseParaEsperaCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Reanuda un ticket que estaba en espera. Solo el técnico asignado.</summary>
    [HttpPatch("{id:guid}/reanudar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> ReanudarDesdeEspera(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new ReanudarDesdeEsperaCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Envía el ticket para validación del solicitante. Solo el técnico asignado.</summary>
    [HttpPatch("{id:guid}/submit-validacion")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> SubmitParaValidacion(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new SubmitParaValidacionCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Cierra un ticket validado. El solicitante puede cerrarlo con valoración opcional.</summary>
    [HttpPatch("{id:guid}/cerrar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Cerrar(Guid id, [FromBody] CerrarTicketRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new CerrarTicketCommand(id, request.Valoracion), ct);
        return HandleResult(result);
    }

    /// <summary>Rechaza la resolución y reabre el ticket con motivo obligatorio.</summary>
    [HttpPatch("{id:guid}/reabrir")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Reabrir(Guid id, [FromBody] ReabrirTicketRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(
            new ReabrirTicketCommand(id, request.MotivoRechazoId, request.ComentarioRechazo), ct);
        return HandleResult(result);
    }

    /// <summary>Cancela un ticket. Requiere rol Admin o superior.</summary>
    [HttpPatch("{id:guid}/cancelar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarTicketRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new CancelarTicketCommand(id, request.MotivoCancelacionId), ct);
        return HandleResult(result);
    }

    /// <summary>Cambia la prioridad de un ticket. Requiere rol Admin o superior.</summary>
    [HttpPatch("{id:guid}/prioridad")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> CambiarPrioridad(Guid id, [FromBody] CambiarPrioridadRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new CambiarPrioridadCommand(id, request.NuevaPrioridad), ct);
        return HandleResult(result);
    }

    /// <summary>Cambia el área de un ticket. Restringido a Sin Asignar salvo Admin con auditoría.</summary>
    [HttpPatch("{id:guid}/area")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> CambiarArea(Guid id, [FromBody] CambiarAreaRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new CambiarAreaCommand(id, request.NuevaAreaId), ct);
        return HandleResult(result);
    }
}

// ---------------------------------------------------------------------------
// Request records
// ---------------------------------------------------------------------------

/// <summary>Payload para crear un nuevo ticket.</summary>
public sealed record CrearTicketRequest(
    string Titulo,
    string Descripcion,
    Guid SucursalId,
    string AreaNombre,
    Guid TipoServicioId,
    Guid CategoriaId,
    PrioridadTipo Prioridad,
    string? Ubicacion);

/// <summary>Payload para asignar un ticket a un técnico.</summary>
public sealed record AsignarTicketRequest(Guid TecnicoId);

/// <summary>Payload para reasignar un ticket indicando el nuevo técnico y un motivo opcional.</summary>
public sealed record ReasignarTicketRequest(Guid NuevoTecnicoId, string? Motivo);

/// <summary>Payload para cerrar un ticket con valoración opcional (1-5).</summary>
public sealed record CerrarTicketRequest(byte? Valoracion);

/// <summary>Payload para reabrir un ticket rechazado.</summary>
public sealed record ReabrirTicketRequest(Guid MotivoRechazoId, string? ComentarioRechazo);

/// <summary>Payload para cancelar un ticket con motivo de catálogo.</summary>
public sealed record CancelarTicketRequest(Guid MotivoCancelacionId);

/// <summary>Payload para cambiar la prioridad de un ticket.</summary>
public sealed record CambiarPrioridadRequest(PrioridadTipo NuevaPrioridad);

/// <summary>Payload para reasignar el área de un ticket.</summary>
public sealed record CambiarAreaRequest(Guid NuevaAreaId);
