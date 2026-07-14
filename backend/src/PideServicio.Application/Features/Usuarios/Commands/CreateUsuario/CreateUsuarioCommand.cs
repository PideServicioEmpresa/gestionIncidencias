namespace PideServicio.Application.Features.Usuarios.Commands.CreateUsuario;

using PideServicio.Application.Common.CQRS;
using PideServicio.Domain.Enums;

public sealed record CreateUsuarioCommand(
    Guid SucursalId,
    Guid? AreaId,
    string Nombre,
    string Apellido,
    string Correo,
    string NombreUsuario,
    string Contrasena,
    string? Telefono,
    RolTipo Rol) : ICommand<Guid>;
