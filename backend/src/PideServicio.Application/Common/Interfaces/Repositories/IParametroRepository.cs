namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Domain.Entities;

public interface IParametroRepository
{
    Task<Parametro?> ObtenerPorClaveAsync(string clave, Guid? empresaId = null, CancellationToken ct = default);
    Task<IReadOnlyList<Parametro>> ListarPorEmpresaAsync(Guid? empresaId, CancellationToken ct = default);
    Task<bool> ExisteClaveAsync(string clave, Guid? empresaId = null, CancellationToken ct = default);
    Task ActualizarAsync(Parametro parametro, CancellationToken ct = default);

    /// <summary>
    /// Inserta el parámetro si no existe; actualiza valor, updated_at y updated_by si ya existe.
    /// La búsqueda del conflicto se basa en (clave, empresa_id), respetando NULLs.
    /// </summary>
    Task UpsertAsync(Parametro parametro, CancellationToken ct = default);
}
