namespace PideServicio.Application.Features.Usuarios.DTOs;

public sealed record SucursalAsignacionDto(
    Guid   SucursalId,
    string SucursalNombre,
    bool   EsPrincipal,
    bool   Activo);
