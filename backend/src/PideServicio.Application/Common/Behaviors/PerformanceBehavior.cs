namespace PideServicio.Application.Common.Behaviors;

using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PideServicio.Application.Common.Options;

public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
    IOptions<PerformanceOptions> opciones
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly int _umbralMs = opciones.Value.UmbralAlertaMs;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cronometro = Stopwatch.StartNew();
        var respuesta = await next();
        cronometro.Stop();

        if (cronometro.ElapsedMilliseconds > _umbralMs)
        {
            logger.LogWarning(
                "Solicitud lenta: {NombreSolicitud} tardó {TiempoMs}ms (umbral: {UmbralMs}ms) — {@Solicitud}",
                typeof(TRequest).Name,
                cronometro.ElapsedMilliseconds,
                _umbralMs,
                request);
        }

        return respuesta;
    }
}
