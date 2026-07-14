namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Categoría de ticket. empresa_id nullable = categoría global (gestionada por SuperAdmin).
/// Un ticket SIEMPRE debe tener categoría (NOT NULL en BD).
/// </summary>
public sealed class Categoria : SoftDeletableEntity
{
    public Guid? EmpresaId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool Activa { get; private set; } = true;

    public bool EsGlobal => EmpresaId is null;

    private Categoria() { }

    public static Categoria Crear(string nombre, Guid? empresaId = null, string? descripcion = null, Guid? creadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre de la categoría es requerido.");

        var ahora = DateTimeOffset.UtcNow;
        return new Categoria
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            Activa = true,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor
        };
    }

    public void Actualizar(string nombre, string? descripcion, Guid actualizadoPor)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre de la categoría es requerido.");

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    public void Activar(Guid actualizadoPor)
    {
        Activa = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    public void Desactivar(Guid actualizadoPor)
    {
        Activa = false;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }
}
