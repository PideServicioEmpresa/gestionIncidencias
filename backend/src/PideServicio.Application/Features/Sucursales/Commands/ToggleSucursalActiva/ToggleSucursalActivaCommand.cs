namespace PideServicio.Application.Features.Sucursales.Commands.ToggleSucursalActiva;

using PideServicio.Application.Common.CQRS;

public sealed record ToggleSucursalActivaCommand(Guid Id) : ICommand<Guid>;
