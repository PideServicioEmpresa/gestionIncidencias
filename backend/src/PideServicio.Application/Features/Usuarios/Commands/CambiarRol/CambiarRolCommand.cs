namespace PideServicio.Application.Features.Usuarios.Commands.CambiarRol;

using PideServicio.Application.Common.CQRS;
using PideServicio.Domain.Enums;

public sealed record CambiarRolCommand(
    Guid UsuarioId,
    RolTipo NuevoRol) : ICommand;
