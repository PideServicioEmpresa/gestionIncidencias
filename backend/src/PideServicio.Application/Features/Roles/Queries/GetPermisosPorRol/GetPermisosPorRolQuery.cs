namespace PideServicio.Application.Features.Roles.Queries.GetPermisosPorRol;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;
using PideServicio.Domain.Enums;

public sealed record GetPermisosPorRolQuery(
    RolTipo Rol,
    Guid? EmpresaId = null) : IQuery<ListResult<PermisoDto>>;
