namespace PideServicio.Application.Features.MotivosRechazo.DTOs;

public sealed record MotivoRechazoDto(
    Guid Id,
    Guid? EmpresaId,
    string Codigo,
    string Nombre,
    string? Descripcion,
    bool EsOtro,
    int Orden,
    bool Activo,
    bool EsGlobal,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
