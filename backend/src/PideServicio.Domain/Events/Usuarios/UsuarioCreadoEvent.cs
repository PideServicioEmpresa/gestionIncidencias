namespace PideServicio.Domain.Events.Usuarios;

using PideServicio.Domain.Enums;

public sealed record UsuarioCreadoEvent(
    Guid UsuarioId,
    Guid EmpresaId,
    string Correo,
    RolTipo Rol,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
