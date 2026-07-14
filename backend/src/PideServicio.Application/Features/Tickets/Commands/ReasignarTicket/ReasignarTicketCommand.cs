namespace PideServicio.Application.Features.Tickets.Commands.ReasignarTicket;

using PideServicio.Application.Common.CQRS;

public sealed record ReasignarTicketCommand(
    Guid TicketId,
    Guid NuevoTecnicoId,
    string? Motivo
) : ICommand;
