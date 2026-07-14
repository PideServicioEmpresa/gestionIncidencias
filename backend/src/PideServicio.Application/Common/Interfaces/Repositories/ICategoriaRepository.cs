namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface ICategoriaRepository
{
    Task<Categoria?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Categoria>> ListarActivasAsync(Guid? empresaId = null, CancellationToken ct = default);
    Task<PagedResult<Categoria>> ListarAsync(Guid? empresaId, int pagina, int tamanoPagina, bool? soloActivas = null, string? busqueda = null, bool soloGlobales = false, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(Guid? empresaId, string nombre, Guid? excludeId = null, CancellationToken ct = default);
    Task<Guid> CrearAsync(Categoria categoria, CancellationToken ct = default);
    Task ActualizarAsync(Categoria categoria, CancellationToken ct = default);
}
