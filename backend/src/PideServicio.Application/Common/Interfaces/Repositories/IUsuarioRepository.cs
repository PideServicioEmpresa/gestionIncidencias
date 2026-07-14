namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Usuario?> ObtenerPorAuthIdAsync(Guid authId, CancellationToken ct = default);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default);
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default);
    Task<PagedResult<Usuario>> ListarAsync(
        Guid empresaId,
        Guid? sucursalId,
        RolTipo? rol,
        bool? soloActivos,
        string? busqueda,
        int pagina,
        int tamanoPagina,
        CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> ListarTecnicosActivosPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteCorreoAsync(string correo, Guid empresaId, Guid? excluirId = null, CancellationToken ct = default);
    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, Guid? excluirId = null, CancellationToken ct = default);
    Task<Guid> CrearAsync(Usuario usuario, CancellationToken ct = default);
    Task ActualizarAsync(Usuario usuario, CancellationToken ct = default);
}
