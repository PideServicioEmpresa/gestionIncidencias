namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

public sealed class Area : SoftDeletableEntity
{
    public Guid SucursalId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public Guid? ResponsableId { get; private set; }
    public bool Activa { get; private set; } = true;

    private Area() { }

    public static Area Crear(
        Guid sucursalId,
        string nombre,
        string? descripcion = null,
        Guid? responsableId = null,
        Guid? creadoPor = null)
    {
        if (sucursalId == Guid.Empty)
            throw new ValidationException("SucursalId", "El id de sucursal es requerido.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre del área es requerido.");

        var ahora = DateTimeOffset.UtcNow;
        return new Area
        {
            Id = Guid.NewGuid(),
            SucursalId = sucursalId,
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            ResponsableId = responsableId,
            Activa = true,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor
        };
    }

    public void Actualizar(string nombre, string? descripcion, Guid? responsableId, Guid actualizadoPor)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre del área es requerido.");

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        ResponsableId = responsableId;
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
