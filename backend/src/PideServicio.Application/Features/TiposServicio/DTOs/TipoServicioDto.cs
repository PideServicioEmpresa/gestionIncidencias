namespace PideServicio.Application.Features.TiposServicio.DTOs;

public sealed record TipoServicioDto(
    Guid Id,
    Guid EmpresaId,
    string Nombre,
    string? Descripcion,
    int Orden,
    bool Activo,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
