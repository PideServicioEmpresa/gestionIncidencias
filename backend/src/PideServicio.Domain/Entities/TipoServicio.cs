namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Tipo de servicio por empresa. Es inmutable en el ticket una vez creado.
/// </summary>
public sealed class TipoServicio : SoftDeletableEntity
{
    public Guid EmpresaId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public int Orden { get; private set; }
    public bool Activo { get; private set; } = true;

    private TipoServicio() { }

    public static TipoServicio Crear(Guid empresaId, string nombre, int orden, string? descripcion = null, Guid? creadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre del tipo de servicio es requerido.");
        if (orden <= 0)
            throw new ValidationException("Orden", "El orden debe ser mayor que cero.");

        var ahora = DateTimeOffset.UtcNow;
        return new TipoServicio
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            Orden = orden,
            Activo = true,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor
        };
    }

    public void Actualizar(string nombre, int orden, string? descripcion, Guid actualizadoPor)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre del tipo de servicio es requerido.");
        if (orden <= 0)
            throw new ValidationException("Orden", "El orden debe ser mayor que cero.");

        Nombre = nombre.Trim();
        Orden = orden;
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
