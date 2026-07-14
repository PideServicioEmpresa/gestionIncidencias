namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface IMotivoCancelacionRepository
{
    Task<MotivoCancelacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MotivoCancelacion>> ListarActivosAsync(Guid? empresaId = null, CancellationToken ct = default);
    Task<PagedResult<MotivoCancelacion>> ListarAsync(Guid? empresaId, int pagina, int tamanoPagina, bool? soloActivos = null, string? busqueda = null, bool soloGlobales = false, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteTextoAsync(Guid? empresaId, string texto, Guid? excludeId = null, CancellationToken ct = default);
    Task<Guid> CrearAsync(MotivoCancelacion motivo, CancellationToken ct = default);
    Task ActualizarAsync(MotivoCancelacion motivo, CancellationToken ct = default);
}
