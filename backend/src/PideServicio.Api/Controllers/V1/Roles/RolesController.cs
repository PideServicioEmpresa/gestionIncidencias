namespace PideServicio.Api.Controllers.V1.Roles;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Roles.DTOs;
using PideServicio.Application.Features.Roles.Queries.GetPermisos;
using PideServicio.Application.Features.Roles.Queries.GetPermisosPorRol;
using PideServicio.Application.Features.Roles.Queries.GetRoles;
using PideServicio.Contracts.Common;
using PideServicio.Domain.Enums;

/// <summary>Consulta de roles y permisos del sistema (solo lectura).</summary>
[ApiVersion("1.0")]
[Tags("Roles")]
public sealed class RolesController : ApiControllerBase
{
    /// <summary>Devuelve todos los roles disponibles en el sistema.</summary>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<ListResult<RolDto>>), 200)]
    public async Task<IActionResult> ObtenerRoles(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetRolesQuery(), ct);
        return HandleResult(result);
    }

    /// <summary>Devuelve todos los permisos definidos en el sistema.</summary>
    [HttpGet("permisos")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<ListResult<PermisoDto>>), 200)]
    public async Task<IActionResult> ObtenerPermisos(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetPermisosQuery(), ct);
        return HandleResult(result);
    }

    /// <summary>Devuelve los permisos efectivos de un rol, opcionalmente filtrados por empresa.</summary>
    [HttpGet("permisos/por-rol")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<ListResult<PermisoDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> ObtenerPermisosPorRol(
        [FromQuery] RolTipo rol,
        [FromQuery] Guid? empresaId,
        CancellationToken ct)
    {
        var result = await Mediator.Send(new GetPermisosPorRolQuery(rol, empresaId), ct);
        return HandleResult(result);
    }
}
