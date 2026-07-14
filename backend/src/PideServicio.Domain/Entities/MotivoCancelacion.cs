namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Motivo de cancelación de ticket. empresa_id nullable = motivo global.
/// </summary>
public sealed class MotivoCancelacion : SoftDeletableEntity
{
    public Guid? EmpresaId { get; private set; }
    public string Texto { get; private set; } = string.Empty;
    public bool Activo { get; private set; } = true;

    public bool EsGlobal => EmpresaId is null;

    private MotivoCancelacion() { }

    public static MotivoCancelacion Crear(
        string texto,
        Guid? empresaId = null,
        Guid? creadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ValidationException("Texto", "El texto del motivo de cancelación es requerido.");
        if (texto.Trim().Length > 300)
            throw new ValidationException("Texto", "El motivo de cancelación no puede exceder 300 caracteres.");

        var ahora = DateTimeOffset.UtcNow;
        return new MotivoCancelacion
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Texto = texto.Trim(),
            Activo = true,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor
        };
    }

    public void Actualizar(string texto, Guid actualizadoPor)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ValidationException("Texto", "El texto del motivo de cancelación es requerido.");
        if (texto.Trim().Length > 300)
            throw new ValidationException("Texto", "El motivo de cancelación no puede exceder 300 caracteres.");

        Texto = texto.Trim();
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
