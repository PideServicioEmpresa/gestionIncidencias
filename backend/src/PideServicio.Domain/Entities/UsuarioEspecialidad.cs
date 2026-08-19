namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Relación M-N entre Usuario y Especialidad. Permite usuarios con varias especialidades.
/// Sin jerarquía: ninguna especialidad es "principal".
/// </summary>
public sealed class UsuarioEspecialidad : BaseEntity
{
    public Guid UsuarioId      { get; private set; }
    public Guid EspecialidadId { get; private set; }
    public bool Activo         { get; private set; } = true;

    public DateTimeOffset CreatedAt { get; private set; }
    public Guid?          CreatedBy { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid?          UpdatedBy { get; private set; }

    private UsuarioEspecialidad() { }

    public static UsuarioEspecialidad Asignar(
        Guid  usuarioId,
        Guid  especialidadId,
        Guid? asignadoPor = null)
    {
        if (usuarioId      == Guid.Empty) throw new ValidationException("UsuarioId",      "El id del usuario es requerido.");
        if (especialidadId == Guid.Empty) throw new ValidationException("EspecialidadId", "El id de la especialidad es requerido.");

        var ahora = DateTimeOffset.UtcNow;
        return new UsuarioEspecialidad
        {
            Id             = Guid.NewGuid(),
            UsuarioId      = usuarioId,
            EspecialidadId = especialidadId,
            Activo         = true,
            CreatedAt      = ahora,
            UpdatedAt      = ahora,
            CreatedBy      = asignadoPor
        };
    }
}
