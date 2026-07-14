namespace PideServicio.Application.Features.Tickets.Commands.CambiarArea;

using PideServicio.Application.Common.CQRS;

public sealed record CambiarAreaCommand(
    Guid TicketId,
    Guid NuevaAreaId
) : ICommand;
