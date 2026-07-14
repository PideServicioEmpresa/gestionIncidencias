namespace PideServicio.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate siguiente)
{
    private const string EncabezadoCorrelacionId = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext contexto)
    {
        var correlacionId = contexto.Request.Headers[EncabezadoCorrelacionId].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        contexto.TraceIdentifier = correlacionId;
        contexto.Response.Headers[EncabezadoCorrelacionId] = correlacionId;

        using (Serilog.Context.LogContext.PushProperty("TraceId", correlacionId))
        {
            await siguiente(contexto);
        }
    }
}
