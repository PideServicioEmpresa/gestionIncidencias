namespace PideServicio.Domain.Exceptions;

public sealed class ValidationException : DomainException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Se produjeron uno o más errores de validación.")
    {
        Errors = errors;
    }

    public ValidationException(string campo, string mensaje)
        : base(mensaje)
    {
        Errors = new Dictionary<string, string[]> { [campo] = [mensaje] };
    }
}
