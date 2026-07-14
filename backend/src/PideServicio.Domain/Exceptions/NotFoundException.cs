namespace PideServicio.Domain.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string recurso, object id)
        : base($"{recurso} con id '{id}' no fue encontrado.") { }

    public NotFoundException(string mensaje) : base(mensaje) { }
}
