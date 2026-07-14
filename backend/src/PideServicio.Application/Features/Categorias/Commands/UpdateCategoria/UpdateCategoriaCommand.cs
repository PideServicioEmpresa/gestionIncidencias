namespace PideServicio.Application.Features.Categorias.Commands.UpdateCategoria;

using PideServicio.Application.Common.CQRS;

public sealed record UpdateCategoriaCommand(
    Guid Id,
    string Nombre,
    string? Descripcion
) : ICommand<Guid>;
