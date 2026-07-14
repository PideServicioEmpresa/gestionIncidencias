namespace PideServicio.Domain.Events.Usuarios;

public sealed record UsuarioDesactivadoEvent(
    Guid UsuarioId,
    Guid EmpresaId,
    string NombreCompleto,
    DateTimeOffset OcurridoEn
) : IDomainEvent;
