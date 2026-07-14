namespace PideServicio.Application.Features.Roles.Queries.GetPermisos;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;

public sealed record GetPermisosQuery : IQuery<ListResult<PermisoDto>>;
