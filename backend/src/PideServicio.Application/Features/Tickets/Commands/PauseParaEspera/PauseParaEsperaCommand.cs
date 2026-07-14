namespace PideServicio.Application.Features.Tickets.Commands.PauseParaEspera;

using PideServicio.Application.Common.CQRS;

public sealed record PauseParaEsperaCommand(Guid TicketId) : ICommand;
