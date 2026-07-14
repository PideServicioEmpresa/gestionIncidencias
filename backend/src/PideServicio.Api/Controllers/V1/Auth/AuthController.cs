namespace PideServicio.Api.Controllers.V1.Auth;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Auth.Commands.CambiarContrasena;
using PideServicio.Application.Features.Auth.Commands.Login;
using PideServicio.Application.Features.Auth.Commands.Logout;
using PideServicio.Application.Features.Auth.Commands.RecuperarContrasena;
using PideServicio.Application.Features.Auth.Commands.RefreshToken;
using PideServicio.Application.Features.Auth.DTOs;
using PideServicio.Application.Features.Auth.Queries.GetMe;
using PideServicio.Contracts.Common;

/// <summary>Gestión de sesión: login, logout, refresh y perfil del usuario autenticado.</summary>
[ApiVersion("1.0")]
[Tags("Autenticación")]
public sealed class AuthController : ApiControllerBase
{
    // ── Endpoints públicos (no requieren JWT) ─────────────────────────────────

    /// <summary>Inicia sesión con credenciales de email y contraseña.</summary>
    /// <remarks>
    /// Autentica contra Supabase Auth, valida que el usuario tenga perfil en la base de datos
    /// y devuelve tokens JWT (access + refresh) junto con el perfil completo del usuario.
    ///
    /// El **access_token** tiene una vida de 3600 segundos (1 hora).
    /// El **refresh_token** se usa en POST /refresh para obtener nuevos tokens sin reautenticar.
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await Mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>Renueva el access token usando un refresh token válido.</summary>
    /// <remarks>
    /// El refresh token se obtiene tras un login exitoso. No requiere JWT en el header.
    /// Devuelve nuevos access_token y refresh_token.
    /// </remarks>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken ct)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await Mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>Solicita el envío de un correo para recuperar la contraseña.</summary>
    /// <remarks>
    /// Siempre devuelve 200 OK independientemente de si el correo existe, para no
    /// revelar la existencia de cuentas. Si el correo está registrado, Supabase enviará
    /// el enlace de restablecimiento.
    /// </remarks>
    [HttpPost("recuperar-contrasena")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> RecuperarContrasena(
        [FromBody] RecuperarContrasenaRequest request,
        CancellationToken ct)
    {
        var command = new RecuperarContrasenaCommand(request.Email);
        var result = await Mediator.Send(command, ct);
        return HandleResult(result);
    }

    // ── Endpoints autenticados (requieren JWT válido) ─────────────────────────

    /// <summary>Obtiene el perfil completo del usuario autenticado desde la base de datos.</summary>
    /// <remarks>
    /// Enriquece los claims del JWT con datos frescos de la BD: nombre, foto, área, sucursal,
    /// estado laboral y flags de permisos. Usar este endpoint al iniciar la sesión en el frontend
    /// y para refrescar el estado del usuario.
    /// </remarks>
    [HttpGet("me")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse<PerfilUsuarioDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> ObtenerPerfil(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetMeQuery(), ct);
        return HandleResult(result);
    }

    /// <summary>Cierra la sesión invalidando el token en Supabase Auth.</summary>
    /// <remarks>
    /// Invalida el access token actual en el servidor de Supabase. El frontend debe
    /// eliminar los tokens almacenados localmente tras recibir el 200 OK.
    /// </remarks>
    [HttpPost("logout")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var token = ObtenerBearerToken();
        if (token is null)
            return Unauthorized(ApiResponse.Fallo(
                new ApiError("NO_AUTENTICADO", "Token no encontrado en el encabezado."), TraceId));

        var result = await Mediator.Send(new LogoutCommand(token), ct);
        return HandleResult(result);
    }

    /// <summary>Cambia la contraseña del usuario autenticado.</summary>
    /// <remarks>
    /// La nueva contraseña debe tener mínimo 8 caracteres, incluir al menos una mayúscula
    /// y un número. La confirmación debe coincidir exactamente.
    /// </remarks>
    [HttpPost("cambiar-contrasena")]
    [Authorize(Policy = "Autenticado")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    [ProducesResponseType(typeof(ApiResponse), 422)]
    public async Task<IActionResult> CambiarContrasena(
        [FromBody] CambiarContrasenaRequest request,
        CancellationToken ct)
    {
        var token = ObtenerBearerToken();
        if (token is null)
            return Unauthorized(ApiResponse.Fallo(
                new ApiError("NO_AUTENTICADO", "Token no encontrado en el encabezado."), TraceId));

        var command = new CambiarContrasenaCommand(token, request.NuevaContrasena, request.ConfirmacionContrasena);
        var result = await Mediator.Send(command, ct);
        return HandleResult(result);
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private string? ObtenerBearerToken()
    {
        var header = Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(header) && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return header["Bearer ".Length..].Trim();
        return null;
    }
}

// ── Request bodies (inline para simplicidad, no necesitan capa Application) ──

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record RecuperarContrasenaRequest(string Email);

public sealed record CambiarContrasenaRequest(string NuevaContrasena, string ConfirmacionContrasena);
