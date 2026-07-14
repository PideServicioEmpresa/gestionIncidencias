namespace PideServicio.Api.Controllers.V1.Usuarios;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Usuarios.Commands.ActivarUsuario;
using PideServicio.Application.Features.Usuarios.Commands.CambiarEstadoLaboral;
using PideServicio.Application.Features.Usuarios.Commands.CambiarRol;
using PideServicio.Application.Features.Usuarios.Commands.CreateUsuario;
using PideServicio.Application.Features.Usuarios.Commands.DesactivarUsuario;
using PideServicio.Application.Features.Usuarios.Commands.UpdateUsuarioPerfil;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Application.Features.Usuarios.Queries.GetUsuarioById;
using PideServicio.Application.Features.Usuarios.Queries.ListUsuarios;
using PideServicio.Contracts.Common;
using PideServicio.Domain.Enums;

[ApiVersion("1.0")]
[Tags("Usuarios")]
public sealed class UsuariosController : ApiControllerBase
{
    /// <summary>Lista usuarios con paginación y filtros opcionales.</summary>
    /// <remarks>
    /// Admin/SuperAdmin pueden listar. El handler aplica el filtro de empresa según el rol del actor.
    /// </remarks>
    [HttpGet]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(PagedResponse<UsuarioResumenDto>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] Guid? empresaId,
        [FromQuery] Guid? sucursalId,
        [FromQuery] RolTipo? rol,
        [FromQuery] bool? soloActivos,
        [FromQuery] string? busqueda,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken ct = default)
    {
        var query = new ListUsuariosQuery(
            empresaId,
            sucursalId,
            rol,
            soloActivos,
            busqueda,
            pagina,
            tamanoPagina);

        var result = await Mediator.Send(query, ct);
        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }

    /// <summary>Obtiene un usuario por su identificador.</summary>
    /// <remarks>
    /// SuperAdmin puede ver cualquier usuario. Admin ve su empresa. Usuario ve su propio perfil.
    /// </remarks>
    [HttpGet("{id:guid}", Name = "GetUsuarioById")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetUsuarioByIdQuery(id), ct);
        return HandleResult(result);
    }

    /// <summary>Crea un nuevo usuario en el sistema y en Supabase Auth.</summary>
    /// <remarks>
    /// Solo Admin y SuperAdmin pueden crear usuarios. El sistema crea el registro en Supabase Auth
    /// usando la contraseña proporcionada y luego registra el usuario en la base de datos.
    /// Si la inserción en BD falla, se revierte automáticamente la creación en Supabase Auth.
    /// </remarks>
    [HttpPost]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Crear([FromBody] CreateUsuarioRequest request, CancellationToken ct)
    {
        var command = new CreateUsuarioCommand(
            request.SucursalId,
            request.AreaId,
            request.Nombre,
            request.Apellido,
            request.Correo,
            request.NombreUsuario,
            request.Contrasena,
            request.Telefono,
            request.Rol);

        var result = await Mediator.Send(command, ct);
        return HandleCreated(result, "GetUsuarioById", new { id = result.Valor });
    }

    /// <summary>Actualiza el perfil de un usuario.</summary>
    /// <remarks>
    /// El propio usuario puede editar su perfil (nombre, apellido, teléfono, foto).
    /// Admin y SuperAdmin pueden también modificar el área.
    /// </remarks>
    [HttpPut("{id:guid}/perfil")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> ActualizarPerfil(
        Guid id,
        [FromBody] UpdatePerfilRequest request,
        CancellationToken ct)
    {
        var command = new UpdateUsuarioPerfilCommand(
            id,
            request.Nombre,
            request.Apellido,
            request.Telefono,
            request.AreaId,
            request.FotoUrl,
            request.ActualizarFoto);

        var result = await Mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>Cambia el rol de un usuario.</summary>
    /// <remarks>
    /// Solo Admin y SuperAdmin. Solo un SuperAdmin puede asignar el rol SUPERADMIN.
    /// Un admin no puede cambiar su propio rol.
    /// </remarks>
    [HttpPut("{id:guid}/rol")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> CambiarRol(
        Guid id,
        [FromBody] CambiarRolRequest request,
        CancellationToken ct)
    {
        var result = await Mediator.Send(new CambiarRolCommand(id, request.NuevoRol), ct);
        return HandleResult(result);
    }

    /// <summary>Cambia el estado laboral de un usuario.</summary>
    /// <remarks>
    /// Solo Admin y SuperAdmin. Permite marcar al usuario como ACTIVO, VACACIONES, LICENCIA o RETIRADO.
    /// </remarks>
    [HttpPut("{id:guid}/estado-laboral")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> CambiarEstadoLaboral(
        Guid id,
        [FromBody] CambiarEstadoLaboralRequest request,
        CancellationToken ct)
    {
        var result = await Mediator.Send(new CambiarEstadoLaboralCommand(id, request.NuevoEstado), ct);
        return HandleResult(result);
    }

    /// <summary>Activa un usuario previamente desactivado.</summary>
    /// <remarks>Solo Admin y SuperAdmin. El Admin solo puede activar usuarios de su empresa.</remarks>
    [HttpPatch("{id:guid}/activar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new ActivarUsuarioCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Desactiva un usuario activo.</summary>
    /// <remarks>Solo Admin y SuperAdmin. Un admin no puede desactivarse a sí mismo.</remarks>
    [HttpPatch("{id:guid}/desactivar")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Desactivar(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new DesactivarUsuarioCommand(id), ct);
        return HandleResult(result);
    }
}

// ---------------------------------------------------------------------------
// Request records
// ---------------------------------------------------------------------------

/// <summary>Payload para crear un nuevo usuario.</summary>
public sealed record CreateUsuarioRequest(
    Guid SucursalId,
    Guid? AreaId,
    string Nombre,
    string Apellido,
    string Correo,
    string NombreUsuario,
    string Contrasena,
    string? Telefono,
    RolTipo Rol);

/// <summary>Payload para actualizar el perfil de un usuario.</summary>
public sealed record UpdatePerfilRequest(
    string Nombre,
    string Apellido,
    string? Telefono,
    Guid? AreaId,
    string? FotoUrl,
    bool ActualizarFoto = false);

/// <summary>Payload para cambiar el rol de un usuario.</summary>
public sealed record CambiarRolRequest(RolTipo NuevoRol);

/// <summary>Payload para cambiar el estado laboral de un usuario.</summary>
public sealed record CambiarEstadoLaboralRequest(EstadoLaboralTipo NuevoEstado);
