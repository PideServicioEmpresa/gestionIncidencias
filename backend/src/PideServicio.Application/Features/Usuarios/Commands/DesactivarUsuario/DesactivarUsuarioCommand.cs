namespace PideServicio.Application.Features.Usuarios.Commands.DesactivarUsuario;

using PideServicio.Application.Common.CQRS;

public sealed record DesactivarUsuarioCommand(Guid UsuarioId) : ICommand;
