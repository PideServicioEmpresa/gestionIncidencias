namespace PideServicio.Application.Common.Behaviors;

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using DomainValidationException = PideServicio.Domain.Exceptions.ValidationException;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validadores,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validadores.Any())
            return await next();

        var nombre = typeof(TRequest).Name;
        logger.LogDebug("Validando {NombreSolicitud}", nombre);

        var contexto = new ValidationContext<TRequest>(request);
        var resultados = await Task.WhenAll(
            validadores.Select(v => v.ValidateAsync(contexto, cancellationToken)));

        var errores = resultados
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        if (errores.Count != 0)
            throw new DomainValidationException(errores);

        return await next();
    }
}
