namespace PideServicio.Application.Features.Categorias.DTOs;

public sealed record CategoriaResumenDto(
    Guid Id,
    Guid? EmpresaId,
    string Nombre,
    string? Descripcion,
    bool Activa,
    bool EsGlobal);
