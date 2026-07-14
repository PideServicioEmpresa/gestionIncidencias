namespace PideServicio.Application.Features.Configuracion.Queries.GetParametros;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Configuracion.DTOs;

public sealed record GetParametrosQuery : IQuery<IReadOnlyList<ParametroDto>>;
