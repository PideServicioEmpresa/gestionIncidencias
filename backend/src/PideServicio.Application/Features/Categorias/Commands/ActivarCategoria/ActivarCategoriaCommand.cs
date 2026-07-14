namespace PideServicio.Application.Features.Categorias.Commands.ActivarCategoria;

using PideServicio.Application.Common.CQRS;

public sealed record ActivarCategoriaCommand(Guid Id) : ICommand<Guid>;
