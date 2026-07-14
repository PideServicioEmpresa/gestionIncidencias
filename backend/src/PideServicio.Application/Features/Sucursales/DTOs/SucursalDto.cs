namespace PideServicio.Application.Features.Sucursales.DTOs;

public sealed record SucursalDto(
    Guid Id,
    Guid EmpresaId,
    string Nombre,
    string? Descripcion,
    string? Direccion,
    Guid? ResponsableId,
    bool Activa,
    DateTimeOffset CreatedAt);
