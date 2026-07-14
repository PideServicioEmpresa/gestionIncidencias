namespace PideServicio.Application.Features.Areas.Commands.CreateArea;

using PideServicio.Application.Common.CQRS;

public sealed record CreateAreaCommand(
    Guid SucursalId,
    string Nombre,
    string? Descripcion,
    Guid? ResponsableId
) : ICommand<Guid>;
