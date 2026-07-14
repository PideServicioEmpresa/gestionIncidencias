namespace PideServicio.Application.Features.MotivosCancelacion.DTOs;

public sealed record MotivoCancelacionResumenDto(
    Guid Id,
    Guid? EmpresaId,
    string Texto,
    bool Activo,
    bool EsGlobal);
