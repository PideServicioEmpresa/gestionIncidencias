namespace PideServicio.Application.Features.MotivosCancelacion.DTOs;

public sealed record MotivoCancelacionDto(
    Guid Id,
    Guid? EmpresaId,
    string Texto,
    bool Activo,
    bool EsGlobal,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
