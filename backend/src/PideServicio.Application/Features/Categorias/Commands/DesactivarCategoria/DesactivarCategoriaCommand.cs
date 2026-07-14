namespace PideServicio.Application.Features.Categorias.Commands.DesactivarCategoria;

using PideServicio.Application.Common.CQRS;

public sealed record DesactivarCategoriaCommand(Guid Id) : ICommand<Guid>;
