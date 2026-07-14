namespace PideServicio.Api.Controllers.Common;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Application.Common.Models;
using PideServicio.Contracts.Common;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected string TraceId => HttpContext.TraceIdentifier;

    /// <summary>Maneja un <see cref="Result"/> sin valor de retorno (comandos void-like).</summary>
    protected IActionResult HandleResult(Result result)
    {
        if (result.EsExitoso) return Ok(ApiResponse.Ok(TraceId));
        return MapFailure(result);
    }

    /// <summary>Maneja un <see cref="Result{T}"/> con valor de retorno.</summary>
    protected IActionResult HandleResult<T>(Result<T> result, int successStatusCode = 200)
    {
        if (result.EsExitoso)
            return StatusCode(successStatusCode, ApiResponse<T>.Ok(result.Valor!, TraceId));
        return MapFailure(result);
    }

    /// <summary>
    /// Maneja un <see cref="Result{T}"/> para creación (HTTP 201).
    /// Si se proporcionan <paramref name="routeName"/> y <paramref name="routeValues"/>,
    /// devuelve un <c>201 CreatedAtRoute</c>; en caso contrario, un <c>201 StatusCode</c>.
    /// </summary>
    protected IActionResult HandleCreated<T>(Result<T> result, string? routeName = null, object? routeValues = null)
    {
        if (result.EsFallido) return MapFailure(result);

        if (routeName is not null)
            return CreatedAtRoute(routeName, routeValues, ApiResponse<T>.Ok(result.Valor!, TraceId));

        return StatusCode(201, ApiResponse<T>.Ok(result.Valor!, TraceId));
    }

    /// <summary>
    /// Devuelve un <c>200 OK</c> envolviendo el resultado paginado en
    /// <see cref="ApiResponse{T}"/> donde T es <see cref="PagedResponse{TItem}"/>.
    /// Esto mantiene el contrato uniforme <c>{ exitoso, datos, traceId }</c>
    /// en todos los endpoints, incluidos los paginados.
    /// </summary>
    protected IActionResult OkPaged<T>(PagedResult<T> paged)
    {
        var response = new PagedResponse<T>
        {
            Items          = paged.Items,
            Pagina         = paged.Pagina,
            TamanoPagina   = paged.TamanoPagina,
            TotalRegistros = paged.TotalRegistros,
        };
        return Ok(ApiResponse<PagedResponse<T>>.Ok(response, TraceId));
    }

    // ---------------------------------------------------------------------------
    // Mapeo de fallo Result → IActionResult con código HTTP semántico
    // ---------------------------------------------------------------------------
    private IActionResult MapFailure(Result result)
    {
        var traceId = TraceId;

        return result.Tipo switch
        {
            ResultTipo.NoEncontrado =>
                NotFound(ApiResponse.Fallo(
                    new ApiError("NO_ENCONTRADO", result.Error), traceId)),

            ResultTipo.ErrorValidacion =>
                UnprocessableEntity(ApiResponse.Fallo(
                    new ApiError("ERROR_VALIDACION", result.Error, result.Errores), traceId)),

            ResultTipo.NoAutorizado =>
                Unauthorized(ApiResponse.Fallo(
                    new ApiError("NO_AUTENTICADO", result.Error), traceId)),

            ResultTipo.NoPermitido =>
                StatusCode(403, ApiResponse.Fallo(
                    new ApiError("SIN_PERMISOS", result.Error), traceId)),

            ResultTipo.ErrorNegocio =>
                Conflict(ApiResponse.Fallo(
                    new ApiError("ERROR_NEGOCIO", result.Error), traceId)),

            _ =>
                StatusCode(500, ApiResponse.Fallo(
                    new ApiError("ERROR_INTERNO", result.Error), traceId))
        };
    }
}
