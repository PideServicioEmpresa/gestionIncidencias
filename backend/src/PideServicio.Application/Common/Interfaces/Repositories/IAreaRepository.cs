namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface IAreaRepository
{
    Task<Area?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Area>> ListarPorSucursalAsync(Guid sucursalId, bool? soloActivas = null, CancellationToken ct = default);
    Task<PagedResult<Area>> ListarAsync(Guid? sucursalId, Guid? empresaId, int pagina, int tamanoPagina, bool? soloActivas = null, string? busqueda = null, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(Guid sucursalId, string nombre, Guid? excludeId = null, CancellationToken ct = default);
    Task<bool> PerteneceASucursalAsync(Guid areaId, Guid sucursalId, CancellationToken ct = default);
    Task<Guid> CrearAsync(Area area, CancellationToken ct = default);
    Task ActualizarAsync(Area area, CancellationToken ct = default);
}
