namespace PideServicio.Application.Features.Areas.Queries.GetAreaById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Areas.DTOs;

public sealed record GetAreaByIdQuery(Guid Id) : IQuery<AreaDto>;
