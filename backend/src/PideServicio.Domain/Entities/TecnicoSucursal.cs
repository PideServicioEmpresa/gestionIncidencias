namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Relación M-N entre Técnico y Sucursal. Permite técnicos multi-sucursal.
/// El trigger trg_fn_tecnico_sucursales_guard_principal garantiza exactamente una sucursal principal por técnico.
/// </summary>
public sealed class TecnicoSucursal : BaseEntity
{
    public Guid TecnicoId { get; private set; }
    public Guid SucursalId { get; private set; }
    public bool EsPrincipal { get; private set; }
    public bool Activo { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private TecnicoSucursal() { }

    public static TecnicoSucursal Asignar(
        Guid tecnicoId,
        Guid sucursalId,
        bool esPrincipal = false,
        Guid? asignadoPor = null)
    {
        if (tecnicoId == Guid.Empty)
            throw new ValidationException("TecnicoId", "El id del técnico es requerido.");
        if (sucursalId == Guid.Empty)
            throw new ValidationException("SucursalId", "El id de la sucursal es requerido.");

        var ahora = DateTimeOffset.UtcNow;
        return new TecnicoSucursal
        {
            Id = Guid.NewGuid(),
            TecnicoId = tecnicoId,
            SucursalId = sucursalId,
            EsPrincipal = esPrincipal,
            Activo = true,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = asignadoPor
        };
    }

    public void MarcarComoPrincipal(Guid actualizadoPor)
    {
        EsPrincipal = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    public void DesmarcarComoPrincipal(Guid actualizadoPor)
    {
        EsPrincipal = false;
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
