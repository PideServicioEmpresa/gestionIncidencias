namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface IEmpresaRepository
{
    Task<Empresa?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Empresa?> ObtenerPorIdentificacionFiscalAsync(string identificacion, CancellationToken ct = default);
    Task<PagedResult<Empresa>> ListarAsync(int pagina, int tamanoPagina, bool? soloActivas = null, string? busqueda = null, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteIdentificacionFiscalAsync(string identificacion, Guid? excluirId = null, CancellationToken ct = default);
    Task<Guid> CrearAsync(Empresa empresa, CancellationToken ct = default);
    Task ActualizarAsync(Empresa empresa, CancellationToken ct = default);
}
