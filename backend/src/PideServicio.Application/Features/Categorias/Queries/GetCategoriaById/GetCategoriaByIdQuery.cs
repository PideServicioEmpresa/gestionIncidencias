namespace PideServicio.Application.Features.Categorias.Queries.GetCategoriaById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Categorias.DTOs;

public sealed record GetCategoriaByIdQuery(Guid Id) : IQuery<CategoriaDto>;
