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
    DateTimeOffset CreatedAt)
{
    // Mapster no mapea esta propiedad; el handler la rellena manualmente tras Adapt<>.
    public IReadOnlyList<SucursalAsignacionDto> Sucursales { get; init; } = [];

    // Mapster no mapea esta propiedad; el handler la rellena manualmente tras Adapt<>.
    // Siempre es una lista (vacía si el usuario no tiene especialidades), nunca null.
    public IReadOnlyList<EspecialidadAsignadaDto> Especialidades { get; init; } = [];
}
