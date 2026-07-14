namespace PideServicio.Application.Features.Tickets.Commands.SubmitParaValidacion;

using PideServicio.Application.Common.CQRS;

public sealed record SubmitParaValidacionCommand(Guid TicketId) : ICommand;
