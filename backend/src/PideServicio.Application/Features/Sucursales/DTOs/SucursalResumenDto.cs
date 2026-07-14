namespace PideServicio.Application.Features.Sucursales.DTOs;

public sealed record SucursalResumenDto(
    Guid Id,
    Guid EmpresaId,
    string Nombre,
    bool Activa,
    DateTimeOffset CreatedAt);
