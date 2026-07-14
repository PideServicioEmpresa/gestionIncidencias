namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Enums;

/// <summary>
/// Asignación de un permiso a un rol. empresa_id nullable = aplica globalmente.
/// Tabla append-only en la práctica: los permisos se activan/desactivan, no se modifican.
/// </summary>
public sealed class RolePermission : BaseEntity
{
    public RolTipo RolCodigo { get; private set; }
    public Guid PermisoId { get; private set; }
    public Guid? EmpresaId { get; private set; }
    public bool Activo { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    private RolePermission() { }

    public static RolePermission Asignar(RolTipo rolCodigo, Guid permisoId, Guid? empresaId, Guid? asignadoPor = null)
    {
        return new RolePermission
        {
            Id = Guid.NewGuid(),
            RolCodigo = rolCodigo,
            PermisoId = permisoId,
            EmpresaId = empresaId,
            Activo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = asignadoPor
        };
    }
}
