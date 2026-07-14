namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Motivo de rechazo cuando el solicitante reabre un ticket.
/// Si es_otro = true, el comentario es obligatorio en ticket_historial.
/// </summary>
public sealed class MotivoRechazo : SoftDeletableEntity
{
    public Guid? EmpresaId { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool EsOtro { get; private set; }
    public int Orden { get; private set; }
    public bool Activo { get; private set; } = true;

    public bool EsGlobal => EmpresaId is null;

    private MotivoRechazo() { }

    public static MotivoRechazo Crear(
        string codigo,
        string nombre,
        int orden,
        bool esOtro = false,
        Guid? empresaId = null,
        string? descripcion = null,
        Guid? creadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ValidationException("Codigo", "El código del motivo de rechazo es requerido.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre del motivo de rechazo es requerido.");
        if (orden <= 0)
            throw new ValidationException("Orden", "El orden debe ser mayor que cero.");

        var ahora = DateTimeOffset.UtcNow;
        return new MotivoRechazo
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Codigo = codigo.Trim().ToUpperInvariant(),
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            EsOtro = esOtro,
            Orden = orden,
            Activo = true,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor
        };
    }

    public void Actualizar(string nombre, string codigo, int orden, string? descripcion, Guid actualizadoPor)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre del motivo de rechazo es requerido.");
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ValidationException("Codigo", "El código del motivo de rechazo es requerido.");
        if (orden <= 0)
            throw new ValidationException("Orden", "El orden debe ser mayor que cero.");

        Nombre = nombre.Trim();
        Codigo = codigo.Trim().ToUpperInvariant();
        Descripcion = descripcion?.Trim();
        Orden = orden;
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
