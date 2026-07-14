namespace PideServicio.Application.Features.MotivosRechazo.DTOs;

public sealed record MotivoRechazoResumenDto(
    Guid Id,
    Guid? EmpresaId,
    string Codigo,
    string Nombre,
    bool EsOtro,
    int Orden,
    bool Activo,
    bool EsGlobal);
