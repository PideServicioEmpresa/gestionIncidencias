namespace PideServicio.Application.Features.Roles.Queries.GetRoles;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;

public sealed record GetRolesQuery : IQuery<ListResult<RolDto>>;
