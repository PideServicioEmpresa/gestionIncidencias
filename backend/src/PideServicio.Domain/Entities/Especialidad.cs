namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Especialidad de técnico. empresa_id nullable = especialidad global (gestionada por SuperAdmin).
/// </summary>
public sealed class Especialidad : SoftDeletableEntity
{
    public Guid? EmpresaId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool Activo { get; private set; } = true;

    public bool EsGlobal => EmpresaId is null;

    private Especialidad() { }

    public static Especialidad Crear(Guid? empresaId, string nombre, string? descripcion = null, Guid? creadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre de la especialidad es requerido.");

        var ahora = DateTimeOffset.UtcNow;
        return new Especialidad
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            Activo = true,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor
        };
    }

    public void Actualizar(string nombre, string? descripcion, Guid actualizadoPor)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre de la especialidad es requerido.");

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    public void Activar(Guid actualizadoPor)
    {
        Activo = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    public void Desactivar(Guid actualizadoPor)
    {
        Activo = false;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }
}
