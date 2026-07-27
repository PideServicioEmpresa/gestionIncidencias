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
            // Render muestra los logs línea a línea; {Exception} queda en líneas
            // subsiguientes que se pierden en el viewer. Incluimos el tipo y el
            // inner exception en el mensaje estructurado para que aparezcan en la
            // primera línea, y escribimos también a stderr para captura garantizada.
            var innerMsg = excepcion.InnerException is { } inner
                ? $"{inner.GetType().Name}: {inner.Message}"
                : "(sin inner exception)";

            logger.LogError(excepcion,
                "Excepción no controlada [{CodigoEstado}] {TraceId} | Tipo={TipoExcepcion} | Inner={InnerExcepcion} | {Mensaje}",
                (int)codigoHttp, traceId,
                excepcion.GetType().FullName,
                innerMsg,
                excepcion.Message);

            // Garantía de último recurso: stderr siempre aparece en Render
            // independientemente del output template de Serilog.
            await Console.Error.WriteLineAsync(
                $"[500] TraceId={traceId} | {excepcion.GetType().FullName}: {excepcion.Message}{Environment.NewLine}{excepcion}");
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
