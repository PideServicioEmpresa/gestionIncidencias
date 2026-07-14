namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;

/// <summary>
/// Catálogo de permisos del sistema. Formato de código: modulo.recurso.accion.
/// Se gestionan mediante seeds; no se crean ni eliminan durante el ciclo de vida normal.
/// </summary>
public sealed class Permiso : BaseEntity
{
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public string Modulo { get; private set; } = string.Empty;
    public string Recurso { get; private set; } = string.Empty;
    public string Accion { get; private set; } = string.Empty;
    public bool Activo { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private Permiso() { }
}
