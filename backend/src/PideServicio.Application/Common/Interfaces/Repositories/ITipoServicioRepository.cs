namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface ITipoServicioRepository
{
    Task<TipoServicio?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TipoServicio>> ListarActivosPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<PagedResult<TipoServicio>> ListarAsync(Guid empresaId, int pagina, int tamanoPagina, bool? soloActivos = null, string? busqueda = null, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(Guid empresaId, string nombre, Guid? excludeId = null, CancellationToken ct = default);
    Task<bool> ExisteOrdenAsync(Guid empresaId, int orden, Guid? excludeId = null, CancellationToken ct = default);
    Task<bool> PerteneceAEmpresaAsync(Guid tipoServicioId, Guid empresaId, CancellationToken ct = default);
    Task<Guid> CrearAsync(TipoServicio tipoServicio, CancellationToken ct = default);
    Task ActualizarAsync(TipoServicio tipoServicio, CancellationToken ct = default);
}
