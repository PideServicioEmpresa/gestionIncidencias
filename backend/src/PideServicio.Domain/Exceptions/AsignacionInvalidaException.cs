namespace PideServicio.Domain.Exceptions;

public sealed class AsignacionInvalidaException : DomainException
{
    public AsignacionInvalidaException(string motivo)
        : base($"Asignación inválida: {motivo}") { }
}
