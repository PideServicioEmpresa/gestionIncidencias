namespace PideServicio.Application.Features.Tickets.Commands.CambiarPrioridad;

using PideServicio.Application.Common.CQRS;
using PideServicio.Domain.Enums;

public sealed record CambiarPrioridadCommand(
    Guid TicketId,
    PrioridadTipo NuevaPrioridad
) : ICommand;
