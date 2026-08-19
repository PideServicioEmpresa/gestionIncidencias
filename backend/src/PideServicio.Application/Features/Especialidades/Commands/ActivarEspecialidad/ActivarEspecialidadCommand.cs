namespace PideServicio.Application.Features.Especialidades.Commands.ActivarEspecialidad;

using PideServicio.Application.Common.CQRS;

public sealed record ActivarEspecialidadCommand(Guid Id) : ICommand<Guid>;
