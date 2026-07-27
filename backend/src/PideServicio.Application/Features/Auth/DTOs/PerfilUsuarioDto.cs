namespace PideServicio.Application.Features.Auth.DTOs;

using PideServicio.Application.Features.Usuarios.DTOs;

public sealed record PerfilUsuarioDto(
    Guid Id,
    Guid AuthId,
    Guid EmpresaId,
    Guid SucursalId,
    Guid? AreaId,
    string Nombre,
    string Apellido,
    string NombreCompleto,
    string Correo,
    string NombreUsuario,
    string? Telefono,
    string? FotoUrl,
    string Rol,
    string EstadoLaboral,
    bool Activo,
    DateTimeOffset? UltimoAcceso,
    PermisosDto Permisos,
    IReadOnlyList<SucursalAsignacionDto> Sucursales);

public sealed record PermisosDto(
    bool EsSuperAdmin,
    bool EsAdmin,
    bool EsSupervisor,
    bool EsTecnico,
    bool EsTrabajador,
    bool EsUsuario,
    bool TieneAccesoAdministrativo,
    bool PuedeManejarTickets);
