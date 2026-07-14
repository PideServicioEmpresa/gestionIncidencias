namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Domain.Entities;

public interface ITecnicoSucursalRepository
{
    Task<TecnicoSucursal?> ObtenerAsync(Guid tecnicoId, Guid sucursalId, CancellationToken ct = default);
    Task<IReadOnlyList<TecnicoSucursal>> ListarPorTecnicoAsync(Guid tecnicoId, bool? soloActivos = null, CancellationToken ct = default);
    Task<IReadOnlyList<TecnicoSucursal>> ListarPorSucursalAsync(Guid sucursalId, bool? soloActivos = null, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid tecnicoId, Guid sucursalId, CancellationToken ct = default);
    Task<Guid> CrearAsync(TecnicoSucursal tecnicoSucursal, CancellationToken ct = default);
    Task ActualizarAsync(TecnicoSucursal tecnicoSucursal, CancellationToken ct = default);
}
