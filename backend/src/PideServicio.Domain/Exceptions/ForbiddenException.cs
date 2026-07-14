namespace PideServicio.Domain.Exceptions;

public sealed class ForbiddenException : DomainException
{
    public ForbiddenException()
        : base("No autorizado. No tiene permisos para realizar esta acción.") { }

    public ForbiddenException(string mensaje) : base(mensaje) { }
}
