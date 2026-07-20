namespace PideServicio.Application.Features.Tickets.Commands.CambiarSucursal;

using PideServicio.Application.Common.CQRS;

public sealed record CambiarSucursalCommand(
    Guid TicketId,
    Guid NuevaSucursalId
) : ICommand;
