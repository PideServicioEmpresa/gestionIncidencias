namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

public sealed class Sucursal : SoftDeletableEntity
{
    public Guid EmpresaId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public string? Direccion { get; private set; }
    public Guid? ResponsableId { get; private set; }
    public bool Activa { get; private set; } = true;

    private Sucursal() { }

    public static Sucursal Crear(
        Guid empresaId,
        string nombre,
        string? descripcion = null,
        string? direccion = null,
        Guid? responsableId = null,
        Guid? creadoPor = null)
    {
        if (empresaId == Guid.Empty)
            throw new ValidationException("EmpresaId", "El id de empresa es requerido.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre de la sucursal es requerido.");

        var ahora = DateTimeOffset.UtcNow;
        return new Sucursal
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            Direccion = direccion?.Trim(),
            ResponsableId = responsableId,
            Activa = true,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor
        };
    }

    public void Actualizar(string nombre, string? descripcion, string? direccion, Guid? responsableId, Guid actualizadoPor)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre de la sucursal es requerido.");

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        Direccion = direccion?.Trim();
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

    /// <summary>
    /// La Application layer debe validar que no sea la última sucursal activa de la empresa
    /// y que no tenga tickets activos antes de llamar este método.
    /// </summary>
    public void Desactivar(Guid actualizadoPor)
    {
        Activa = false;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }
}
