namespace PideServicio.Application.Features.Auth.Queries.GetMe;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Auth.DTOs;

public sealed record GetMeQuery : IQuery<PerfilUsuarioDto>;
