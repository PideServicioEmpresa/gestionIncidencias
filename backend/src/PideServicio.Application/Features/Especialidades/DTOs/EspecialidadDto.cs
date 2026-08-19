namespace PideServicio.Application.Features.Especialidades.DTOs;

public sealed record EspecialidadDto(
    Guid Id,
    Guid? EmpresaId,
    string Nombre,
    string? Descripcion,
    bool Activo,
    bool EsGlobal,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
