namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface INotificacionRepository
{
    Task<Notificacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Notificacion>> ListarPorUsuarioAsync(Guid usuarioId, bool? soloNoLeidas, int pagina, int tamanoPagina, CancellationToken ct = default);
    Task<int> ContarNoLeidasAsync(Guid usuarioId, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CrearAsync(Notificacion notificacion, CancellationToken ct = default);
    Task ActualizarAsync(Notificacion notificacion, CancellationToken ct = default);
    Task MarcarTodasLeidasAsync(Guid usuarioId, CancellationToken ct = default);
}
