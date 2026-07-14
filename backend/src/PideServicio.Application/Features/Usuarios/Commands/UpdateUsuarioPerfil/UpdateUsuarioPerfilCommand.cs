namespace PideServicio.Application.Features.Usuarios.Commands.UpdateUsuarioPerfil;

using PideServicio.Application.Common.CQRS;

/// <summary>
/// Actualiza el perfil de un usuario.
/// Para FotoUrl: si ActualizarFoto es true, la foto se actualiza al valor de FotoUrl
/// (null elimina la foto existente). Si ActualizarFoto es false, la foto no se toca.
/// Solo Admin/SuperAdmin pueden modificar AreaId; usuarios normales conservan su área actual.
/// </summary>
public sealed record UpdateUsuarioPerfilCommand(
    Guid UsuarioId,
    string Nombre,
    string Apellido,
    string? Telefono,
    Guid? AreaId,
    string? FotoUrl,
    bool ActualizarFoto = false) : ICommand;
