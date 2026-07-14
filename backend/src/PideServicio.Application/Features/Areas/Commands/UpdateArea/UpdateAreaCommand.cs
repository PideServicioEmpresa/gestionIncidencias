namespace PideServicio.Application.Features.Areas.Commands.UpdateArea;

using PideServicio.Application.Common.CQRS;

public sealed record UpdateAreaCommand(
    Guid Id,
    string Nombre,
    string? Descripcion,
    Guid? ResponsableId
) : ICommand<Guid>;
