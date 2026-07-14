namespace PideServicio.Application.Features.Usuarios.DTOs;

public sealed record UsuarioDto(
    Guid Id,
    Guid EmpresaId,
    Guid SucursalId,
    Guid? AreaId,
    string Nombre,
    string Apellido,
    string NombreCompleto,
    string Correo,
    string NombreUsuario,
    string? Telefono,
    string Rol,
    string EstadoLaboral,
    bool Activo,
    string? FotoUrl,
    DateTimeOffset? UltimoAcceso,
    DateTimeOffset CreatedAt);
