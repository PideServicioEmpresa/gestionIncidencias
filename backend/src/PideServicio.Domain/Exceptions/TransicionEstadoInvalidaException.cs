namespace PideServicio.Domain.Exceptions;

using PideServicio.Domain.Enums;

public sealed class TransicionEstadoInvalidaException : DomainException
{
    public TransicionEstadoInvalidaException(TicketEstadoTipo desde, TicketEstadoTipo hacia)
        : base($"No se puede transicionar el ticket de '{desde}' a '{hacia}'.") { }
}
