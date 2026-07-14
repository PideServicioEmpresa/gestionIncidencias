namespace PideServicio.Application.Features.Sucursales.Commands.CreateSucursal;

using PideServicio.Application.Common.CQRS;

public sealed record CreateSucursalCommand(
    Guid EmpresaId,
    string Nombre,
    string? Descripcion,
    string? Direccion,
    Guid? ResponsableId
) : ICommand<Guid>;
