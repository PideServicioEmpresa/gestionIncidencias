namespace PideServicio.Application.Features.Tickets.Commands.CerrarTicket;

using PideServicio.Application.Common.CQRS;

public sealed record CerrarTicketCommand(
    Guid TicketId,
    byte? Valoracion
) : ICommand;
