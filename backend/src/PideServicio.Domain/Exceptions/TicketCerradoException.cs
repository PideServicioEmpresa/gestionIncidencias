namespace PideServicio.Domain.Exceptions;

public sealed class TicketCerradoException : DomainException
{
    public TicketCerradoException()
        : base("No se puede modificar un ticket cerrado.") { }

    public TicketCerradoException(string accion)
        : base($"No se puede realizar '{accion}' sobre un ticket cerrado.") { }
}
