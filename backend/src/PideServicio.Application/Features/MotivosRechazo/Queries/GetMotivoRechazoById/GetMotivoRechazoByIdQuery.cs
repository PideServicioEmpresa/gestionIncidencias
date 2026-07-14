namespace PideServicio.Application.Features.MotivosRechazo.Queries.GetMotivoRechazoById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.MotivosRechazo.DTOs;

public sealed record GetMotivoRechazoByIdQuery(Guid Id) : IQuery<MotivoRechazoDto>;
