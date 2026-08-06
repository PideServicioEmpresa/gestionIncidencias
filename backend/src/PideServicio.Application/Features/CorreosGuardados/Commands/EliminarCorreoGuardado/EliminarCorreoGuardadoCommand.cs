namespace PideServicio.Application.Features.CorreosGuardados.Commands.EliminarCorreoGuardado;

using PideServicio.Application.Common.CQRS;

public sealed record EliminarCorreoGuardadoCommand(Guid CorreoId) : ICommand;
