namespace PideServicio.Application.Features.Especialidades.Commands.CreateEspecialidad;

using PideServicio.Application.Common.CQRS;

public sealed record CreateEspecialidadCommand(
    Guid? EmpresaId,
    string Nombre,
    string? Descripcion
) : ICommand<Guid>;
