namespace PideServicio.Application.Features.TiposServicio.Commands.UpdateTipoServicio;

using PideServicio.Application.Common.CQRS;

public sealed record UpdateTipoServicioCommand(
    Guid Id,
    string Nombre,
    int Orden,
    string? Descripcion
) : ICommand<Guid>;
