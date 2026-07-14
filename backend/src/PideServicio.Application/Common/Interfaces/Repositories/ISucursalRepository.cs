namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface ISucursalRepository
{
    Task<Sucursal?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Sucursal>> ListarPorEmpresaAsync(Guid empresaId, bool? soloActivas = null, CancellationToken ct = default);
    Task<PagedResult<Sucursal>> ListarAsync(Guid? empresaId, int pagina, int tamanoPagina, bool? soloActivas = null, string? busqueda = null, CancellationToken ct = default);
    Task<int> ContarActivasPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(Guid empresaId, string nombre, Guid? excludeId = null, CancellationToken ct = default);
    Task<bool> PerteneceAEmpresaAsync(Guid sucursalId, Guid empresaId, CancellationToken ct = default);
    Task<Guid> CrearAsync(Sucursal sucursal, CancellationToken ct = default);
    Task ActualizarAsync(Sucursal sucursal, CancellationToken ct = default);
}
