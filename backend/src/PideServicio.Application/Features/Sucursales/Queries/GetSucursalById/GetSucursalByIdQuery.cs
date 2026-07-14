namespace PideServicio.Application.Features.Sucursales.Queries.GetSucursalById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Sucursales.DTOs;

public sealed record GetSucursalByIdQuery(Guid Id) : IQuery<SucursalDto>;
