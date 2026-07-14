namespace PideServicio.Application.Features.Roles.DTOs;

public sealed record PermisoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string? Descripcion);
