namespace PideServicio.Api.Middleware;

using System.Net;
using System.Text.Json;
using PideServicio.Contracts.Common;
using PideServicio.Domain.Exceptions;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate siguiente,
    ILogger<GlobalExceptionMiddleware> logger
)
{
    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await siguiente(contexto);
        }
        catch (Exception ex)
        {
            await ManejarExcepcionAsync(contexto, ex);
        }
    }

    private async Task ManejarExcepcionAsync(HttpContext contexto, Exception excepcion)
    {
        var traceId = contexto.TraceIdentifier;
        var (codigoHttp, apiError) = MapearExcepcion(excepcion);

        // Excepciones de negocio esperadas (4xx) se registran como Warning para evitar
        // ruido en los dashboards de errores de producción.
        // Solo los errores 5xx son verdaderos errores inesperados y se registran como Error.
        if ((int)codigoHttp >= 500)
        {
            logger.LogError(excepcion,
                "Excepción no controlada [{CodigoEstado}] {TraceId}: {Mensaje}",
                (int)codigoHttp, traceId, excepcion.Message);
        }
        else
        {
            logger.LogWarning(
                "Excepción de negocio [{CodigoEstado}] {TraceId}: {TipoExcepcion} — {Mensaje}",
                (int)codigoHttp, traceId, excepcion.GetType().Name, excepcion.Message);
        }

        contexto.Response.StatusCode = (int)codigoHttp;
        contexto.Response.ContentType = "application/json";

        var respuesta = ApiResponse.Fallo(apiError, traceId);
        await contexto.Response.WriteAsync(JsonSerializer.Serialize(respuesta, OpcionesJson));
    }

    private static (HttpStatusCode, ApiError) MapearExcepcion(Exception excepcion) =>
        excepcion switch
        {
            ValidationException ex => (
                HttpStatusCode.UnprocessableEntity,
                new ApiError("ERROR_VALIDACION", "Error de validación.", ex.Errors)),

            NotFoundException ex => (
                HttpStatusCode.NotFound,
                new ApiError("NO_ENCONTRADO", ex.Message)),

            UnauthorizedException => (
                HttpStatusCode.Unauthorized,
                new ApiError("NO_AUTENTICADO", "No autenticado.")),

            ForbiddenException ex => (
                HttpStatusCode.Forbidden,
                new ApiError("SIN_PERMISOS", ex.Message)),

            _ => (
                HttpStatusCode.InternalServerError,
                new ApiError("ERROR_INTERNO", "Ocurrió un error interno. Contacte al administrador."))
        };
}
