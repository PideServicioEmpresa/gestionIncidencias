namespace PideServicio.Application.Features.Especialidades.Commands.DesactivarEspecialidad;

using PideServicio.Application.Common.CQRS;

public sealed record DesactivarEspecialidadCommand(Guid Id) : ICommand<Guid>;
