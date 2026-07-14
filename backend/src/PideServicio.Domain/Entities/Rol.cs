namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Enums;

/// <summary>
/// Catálogo de roles del sistema. Hay exactamente 6 registros (uno por valor de RolTipo).
/// Se gestionan mediante seeds; no se crean ni eliminan durante el ciclo de vida normal.
/// </summary>
public sealed class Rol : BaseEntity
{
    public RolTipo Codigo { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool Activo { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private Rol() { }
}
