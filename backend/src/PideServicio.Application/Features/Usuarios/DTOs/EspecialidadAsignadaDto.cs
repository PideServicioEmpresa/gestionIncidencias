namespace PideServicio.Application.Features.Usuarios.DTOs;

public sealed record EspecialidadAsignadaDto(
    Guid   EspecialidadId,
    string EspecialidadNombre,
    bool   Activo);
