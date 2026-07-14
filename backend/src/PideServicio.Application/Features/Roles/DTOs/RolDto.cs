namespace PideServicio.Application.Features.Roles.DTOs;

public sealed record RolDto(
    string Codigo,
    string Nombre,
    string? Descripcion,
    bool Activo);
