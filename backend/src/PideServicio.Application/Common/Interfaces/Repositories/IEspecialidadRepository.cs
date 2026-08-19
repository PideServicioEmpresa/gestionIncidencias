namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface IEspecialidadRepository
{
    Task<Especialidad?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Especialidad>> ListarActivasAsync(Guid? empresaId, CancellationToken ct = default);
    Task<PagedResult<Especialidad>> ListarAsync(Guid? empresaId, int pagina, int tamanoPagina, bool? soloActivas = null, string? busqueda = null, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(Guid? empresaId, string nombre, Guid? excludeId = null, CancellationToken ct = default);
    Task<Guid> CrearAsync(Especialidad especialidad, CancellationToken ct = default);
    Task ActualizarAsync(Especialidad especialidad, CancellationToken ct = default);
}
