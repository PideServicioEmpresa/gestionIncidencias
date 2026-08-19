namespace PideServicio.Application.Features.Especialidades.Commands.UpdateEspecialidad;

using PideServicio.Application.Common.CQRS;

public sealed record UpdateEspecialidadCommand(
    Guid Id,
    string Nombre,
    string? Descripcion
) : ICommand<Guid>;
