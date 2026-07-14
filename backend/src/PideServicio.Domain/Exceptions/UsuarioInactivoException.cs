namespace PideServicio.Domain.Exceptions;

public sealed class UsuarioInactivoException : DomainException
{
    public UsuarioInactivoException(string nombreUsuario)
        : base($"El usuario '{nombreUsuario}' está inactivo y no puede realizar esta operación.") { }

    public UsuarioInactivoException()
        : base("El usuario está inactivo y no puede realizar esta operación.") { }
}
