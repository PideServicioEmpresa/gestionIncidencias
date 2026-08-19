namespace PideServicio.Application.Features.Especialidades.Queries.GetEspecialidadById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Especialidades.DTOs;

public sealed record GetEspecialidadByIdQuery(Guid Id) : IQuery<EspecialidadDto>;
