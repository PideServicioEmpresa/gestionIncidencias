namespace PideServicio.Application.Common.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using PideServicio.Application.Common.Models;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var nombre = typeof(TRequest).Name;
        logger.LogInformation("Iniciando {NombreSolicitud}: {@Solicitud}", nombre, request);

        var respuesta = await next();

        // Si la respuesta es un Result fallido, registrar el error para trazabilidad.
        // Los handlers capturan excepciones de dominio y las convierten en Result.Fallo;
        // sin este log, esas fallas de negocio quedarían sin registro.
        if (respuesta is Result { EsFallido: true } resultado)
        {
            logger.LogWarning(
                "Solicitud {NombreSolicitud} finalizó con error [{Tipo}]: {Error}",
                nombre, resultado.Tipo, resultado.Error);
        }
        else
        {
            logger.LogInformation("Completado {NombreSolicitud}", nombre);
        }

        return respuesta;
    }
}
