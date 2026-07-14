namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Domain.Entities;

public interface ITicketComentarioRepository
{
    Task<TicketComentario?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TicketComentario>> ListarPorTicketAsync(Guid ticketId, bool incluirInternos = false, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CrearAsync(TicketComentario comentario, CancellationToken ct = default);
    Task ActualizarAsync(TicketComentario comentario, CancellationToken ct = default);
}
