namespace PideServicio.Application.Features.Areas.DTOs;

public sealed record AreaDto(
    Guid Id,
    Guid SucursalId,
    string Nombre,
    string? Descripcion,
    Guid? ResponsableId,
    bool Activa,
    DateTimeOffset CreatedAt);
