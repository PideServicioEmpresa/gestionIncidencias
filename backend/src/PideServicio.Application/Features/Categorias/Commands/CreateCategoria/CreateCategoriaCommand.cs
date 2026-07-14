namespace PideServicio.Application.Features.Categorias.Commands.CreateCategoria;

using PideServicio.Application.Common.CQRS;

public sealed record CreateCategoriaCommand(
    string Nombre,
    string? Descripcion,
    Guid? EmpresaId
) : ICommand<Guid>;
