namespace PideServicio.Application.Features.Especialidades.DTOs;

public sealed record EspecialidadResumenDto(
    Guid Id,
    Guid? EmpresaId,
    string Nombre,
    string? Descripcion,
    bool Activo,
    bool EsGlobal);
