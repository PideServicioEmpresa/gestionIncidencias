namespace PideServicio.Application.Features.Configuracion.Queries.GetParametroPorClave;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Configuracion.DTOs;

public sealed record GetParametroPorClaveQuery(string Clave) : IQuery<ParametroDto>;
