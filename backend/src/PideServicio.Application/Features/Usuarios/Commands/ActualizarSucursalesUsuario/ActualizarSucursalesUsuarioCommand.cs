namespace PideServicio.Application.Features.Usuarios.Commands.ActualizarSucursalesUsuario;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Usuarios.Commands;

public sealed record ActualizarSucursalesUsuarioCommand(
    Guid UsuarioId,
    IReadOnlyList<SucursalAsignacion> Sucursales) : ICommand;
