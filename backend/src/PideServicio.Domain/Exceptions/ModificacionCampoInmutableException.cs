namespace PideServicio.Domain.Exceptions;

public sealed class ModificacionCampoInmutableException : DomainException
{
    public ModificacionCampoInmutableException(string campo)
        : base($"El campo '{campo}' es inmutable y no puede modificarse después de la creación.") { }
}
