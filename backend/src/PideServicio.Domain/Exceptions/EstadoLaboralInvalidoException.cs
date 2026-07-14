namespace PideServicio.Domain.Exceptions;

using PideServicio.Domain.Enums;

public sealed class EstadoLaboralInvalidoException : DomainException
{
    public EstadoLaboralInvalidoException(EstadoLaboralTipo estado)
        : base($"El estado laboral '{estado}' no permite acceso al sistema.") { }
}
