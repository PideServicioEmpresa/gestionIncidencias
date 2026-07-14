namespace PideServicio.Application.Features.Usuarios.Commands.ActivarUsuario;

using PideServicio.Application.Common.CQRS;

public sealed record ActivarUsuarioCommand(Guid UsuarioId) : ICommand;
