namespace PideServicio.Application.Features.Areas.DTOs;

public sealed record AreaResumenDto(
    Guid Id,
    Guid SucursalId,
    string Nombre,
    bool Activa,
    DateTimeOffset CreatedAt);
