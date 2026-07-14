namespace PideServicio.Application.Features.Categorias.DTOs;

public sealed record CategoriaDto(
    Guid Id,
    Guid? EmpresaId,
    string Nombre,
    string? Descripcion,
    bool Activa,
    bool EsGlobal,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
