namespace PideServicio.Application.Features.Tickets.Commands.ActualizarTicket;

using PideServicio.Application.Common.CQRS;

public sealed record ActualizarTicketCommand(
    Guid TicketId,
    string? NuevoTitulo,
    Guid? NuevoTipoServicioId
) : ICommand;
