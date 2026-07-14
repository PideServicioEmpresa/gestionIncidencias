namespace PideServicio.Application.Features.TiposServicio.Queries.GetTipoServicioById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.TiposServicio.DTOs;

public sealed record GetTipoServicioByIdQuery(Guid Id) : IQuery<TipoServicioDto>;
