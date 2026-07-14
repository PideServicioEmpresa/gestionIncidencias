namespace PideServicio.Application.Features.Tickets.Commands.CancelarTicket;

using PideServicio.Application.Common.CQRS;

public sealed record CancelarTicketCommand(
    Guid TicketId,
    Guid MotivoCancelacionId
) : ICommand;
