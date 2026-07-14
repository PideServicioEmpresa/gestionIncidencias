namespace PideServicio.Application.Features.TiposServicio.DTOs;

public sealed record TipoServicioResumenDto(
    Guid Id,
    Guid EmpresaId,
    string Nombre,
    string? Descripcion,
    int Orden,
    bool Activo,
    DateTimeOffset CreatedAt);
