namespace PideServicio.Application.Features.Tickets.Commands.ReanudarDesdeEspera;

using PideServicio.Application.Common.CQRS;

public sealed record ReanudarDesdeEsperaCommand(Guid TicketId) : ICommand;
