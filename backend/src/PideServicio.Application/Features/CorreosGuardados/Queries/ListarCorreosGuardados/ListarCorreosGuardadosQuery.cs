namespace PideServicio.Application.Features.CorreosGuardados.Queries.ListarCorreosGuardados;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.CorreosGuardados.DTOs;

public sealed record ListarCorreosGuardadosQuery : IQuery<IReadOnlyList<CorreoGuardadoDto>>;
