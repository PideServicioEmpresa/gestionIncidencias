namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Relación M-N entre Usuario y Sucursal. Permite usuarios multi-sucursal.
/// El trigger trg_usuario_sucursales_guard_principal garantiza exactamente una sucursal principal por usuario.
/// </summary>
public sealed class UsuarioSucursal : BaseEntity
{
    public Guid UsuarioId   { get; private set; }
    public Guid SucursalId  { get; private set; }
    public bool EsPrincipal { get; private set; }
    public bool Activo      { get; private set; } = true;

    public DateTimeOffset CreatedAt { get; private set; }
    public Guid?          CreatedBy { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid?          UpdatedBy { get; private set; }

    private UsuarioSucursal() { }

    public static UsuarioSucursal Asignar(
        Guid  usuarioId,
        Guid  sucursalId,
        bool  esPrincipal = false,
        Guid? asignadoPor = null)
    {
        if (usuarioId  == Guid.Empty) throw new ValidationException("UsuarioId",  "El id del usuario es requerido.");
        if (sucursalId == Guid.Empty) throw new ValidationException("SucursalId", "El id de la sucursal es requerido.");

        var ahora = DateTimeOffset.UtcNow;
        return new UsuarioSucursal
        {
            Id          = Guid.NewGuid(),
            UsuarioId   = usuarioId,
            SucursalId  = sucursalId,
            EsPrincipal = esPrincipal,
            Activo      = true,
            CreatedAt   = ahora,
            UpdatedAt   = ahora,
            CreatedBy   = asignadoPor
        };
    }
}
