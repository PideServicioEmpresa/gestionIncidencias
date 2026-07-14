namespace PideServicio.Application.Features.Sucursales.Commands.DesactivarSucursal;

using PideServicio.Application.Common.CQRS;

public sealed record DesactivarSucursalCommand(Guid Id) : ICommand<Guid>;
