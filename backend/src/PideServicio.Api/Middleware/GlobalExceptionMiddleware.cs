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
        var (codigoHttp, apiError) = MapearExcepcion(excepcion, traceId);

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

    /// <summary>
    /// Referencia corta del traceId para que el usuario pueda reportarla. El traceId
    /// completo queda en los logs y en el campo traceId de la respuesta.
    /// </summary>
    private static string ReferenciaCorta(string traceId)
    {
        // Formato W3C: 00-{trace-id}-{span-id}-{flags}. Tomamos el inicio del trace-id real.
        var partes = traceId.Split('-');
        var significativo = partes.Length >= 2 ? partes[1] : traceId;
        return significativo.Length >= 8 ? significativo[..8] : significativo;
    }

    private static (HttpStatusCode, ApiError) MapearExcepcion(Exception excepcion, string traceId) =>
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

            // El detalle real de la excepción nunca se expone: solo la referencia
            // para que el usuario pueda reportarla y se cruce con los logs.
            _ => (
                HttpStatusCode.InternalServerError,
                new ApiError(
                    "ERROR_INTERNO",
                    $"Ocurrió un error interno. Código de referencia: {ReferenciaCorta(traceId)}"))
        };
}
