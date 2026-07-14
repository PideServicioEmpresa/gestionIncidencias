namespace PideServicio.Application.Features.Tickets.Commands.AsignarTicket;

using PideServicio.Application.Common.CQRS;

public sealed record AsignarTicketCommand(
    Guid TicketId,
    Guid TecnicoId
) : ICommand;
