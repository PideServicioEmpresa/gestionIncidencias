namespace PideServicio.Domain.Exceptions;

public sealed class UnauthorizedException : DomainException
{
    public UnauthorizedException()
        : base("No autenticado. Se requiere autenticación para acceder a este recurso.") { }
}
