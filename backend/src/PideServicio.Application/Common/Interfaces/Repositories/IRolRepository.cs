namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;

public interface IRolRepository
{
    Task<Rol?> ObtenerPorCodigoAsync(RolTipo codigo, CancellationToken ct = default);
    Task<IReadOnlyList<Rol>> ListarTodosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Permiso>> ListarPermisosDeRolAsync(RolTipo rolCodigo, Guid? empresaId = null, CancellationToken ct = default);
}
