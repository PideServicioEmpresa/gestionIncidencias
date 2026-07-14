namespace PideServicio.Application.Features.Sucursales.Commands.UpdateSucursal;

using PideServicio.Application.Common.CQRS;

public sealed record UpdateSucursalCommand(
    Guid Id,
    string Nombre,
    string? Descripcion,
    string? Direccion,
    Guid? ResponsableId
) : ICommand<Guid>;
