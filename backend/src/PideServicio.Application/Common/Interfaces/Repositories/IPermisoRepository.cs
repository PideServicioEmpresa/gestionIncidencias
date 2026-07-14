namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;

public interface IPermisoRepository
{
    Task<Permiso?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Permiso>> ListarTodosAsync(CancellationToken ct = default);
    Task<bool> TienePermisoAsync(Guid usuarioId, string codigoPermiso, CancellationToken ct = default);
    Task<bool> TienePermisoRolAsync(RolTipo rol, string codigoPermiso, Guid? empresaId = null, CancellationToken ct = default);
}
