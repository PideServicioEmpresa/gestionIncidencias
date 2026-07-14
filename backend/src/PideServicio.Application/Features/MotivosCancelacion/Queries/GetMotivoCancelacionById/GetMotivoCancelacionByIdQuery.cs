namespace PideServicio.Application.Features.MotivosCancelacion.Queries.GetMotivoCancelacionById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.MotivosCancelacion.DTOs;

public sealed record GetMotivoCancelacionByIdQuery(Guid Id) : IQuery<MotivoCancelacionDto>;
