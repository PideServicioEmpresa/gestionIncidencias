namespace PideServicio.Application.Features.Usuarios.DTOs;

public sealed record UsuarioResumenDto(
    Guid Id,
    string NombreCompleto,
    string Correo,
    string Rol,
    string EstadoLaboral,
    bool Activo,
    DateTimeOffset CreatedAt);
