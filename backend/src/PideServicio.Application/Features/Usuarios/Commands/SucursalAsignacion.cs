namespace PideServicio.Application.Features.Usuarios.Commands;

/// <summary>Record de entrada compartido entre CreateUsuario y ActualizarSucursalesUsuario.</summary>
public sealed record SucursalAsignacion(Guid SucursalId, bool EsPrincipal);
