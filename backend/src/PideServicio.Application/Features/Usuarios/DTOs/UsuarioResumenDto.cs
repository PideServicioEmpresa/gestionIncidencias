namespace PideServicio.Application.Features.Usuarios.DTOs;

public sealed record UsuarioResumenDto(
    Guid Id,
    string NombreCompleto,
    string Correo,
    string Rol,
    string EstadoLaboral,
    bool Activo,
    DateTimeOffset CreatedAt)
{
    /// <summary>
    /// Nombres de las especialidades activas del usuario. Informativo: se muestra al
    /// asignar tickets. Lista vacía si no tiene ninguna, nunca null.
    /// </summary>
    public IReadOnlyList<string> Especialidades { get; init; } = [];
}
