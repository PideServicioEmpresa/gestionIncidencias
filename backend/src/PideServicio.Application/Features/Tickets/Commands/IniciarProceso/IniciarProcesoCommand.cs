namespace PideServicio.Application.Features.Tickets.Commands.IniciarProceso;

using PideServicio.Application.Common.CQRS;

public sealed record IniciarProcesoCommand(Guid TicketId) : ICommand;
