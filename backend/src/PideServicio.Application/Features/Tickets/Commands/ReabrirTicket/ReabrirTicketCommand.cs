namespace PideServicio.Application.Features.Tickets.Commands.ReabrirTicket;

using PideServicio.Application.Common.CQRS;

public sealed record ReabrirTicketCommand(
    Guid TicketId,
    Guid MotivoRechazoId,
    string? ComentarioRechazo
) : ICommand;
