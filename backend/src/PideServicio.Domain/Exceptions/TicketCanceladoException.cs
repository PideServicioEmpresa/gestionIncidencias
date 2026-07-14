namespace PideServicio.Domain.Exceptions;

public sealed class TicketCanceladoException : DomainException
{
    public TicketCanceladoException()
        : base("No se puede modificar un ticket cancelado.") { }

    public TicketCanceladoException(string accion)
        : base($"No se puede realizar '{accion}' sobre un ticket cancelado.") { }
}
