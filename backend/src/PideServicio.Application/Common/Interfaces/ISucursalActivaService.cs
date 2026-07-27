namespace PideServicio.Application.Common.Interfaces;

using PideServicio.Domain.Enums;

public interface ISucursalActivaService
{
    /// <summary>
    /// Retorna la sucursal activa validada para la petición en curso.
    /// - Null para roles sin sucursal (ADMIN, SUPERADMIN).
    /// - Sucursal del header X-Sucursal-Activa si está asignada al usuario.
    /// - Sucursal principal del usuario si el header falta o no está asignado.
    /// </summary>
    Task<Guid?> ObtenerAsync(Guid usuarioId, Guid sucursalPrincipal, RolTipo rol, CancellationToken ct = default);
}
