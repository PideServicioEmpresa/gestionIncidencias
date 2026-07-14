namespace PideServicio.Application.Features.Sucursales.Commands.ActivarSucursal;

using PideServicio.Application.Common.CQRS;

public sealed record ActivarSucursalCommand(Guid Id) : ICommand<Guid>;
