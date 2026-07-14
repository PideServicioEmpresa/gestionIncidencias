namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface IMotivoRechazoRepository
{
    Task<MotivoRechazo?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MotivoRechazo>> ListarActivosAsync(Guid? empresaId = null, CancellationToken ct = default);
    Task<PagedResult<MotivoRechazo>> ListarAsync(Guid? empresaId, int pagina, int tamanoPagina, bool? soloActivos = null, string? busqueda = null, bool soloGlobales = false, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> EsOtroAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteCodigoAsync(Guid? empresaId, string codigo, Guid? excludeId = null, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(Guid? empresaId, string nombre, Guid? excludeId = null, CancellationToken ct = default);
    Task<Guid> CrearAsync(MotivoRechazo motivo, CancellationToken ct = default);
    Task ActualizarAsync(MotivoRechazo motivo, CancellationToken ct = default);
}
