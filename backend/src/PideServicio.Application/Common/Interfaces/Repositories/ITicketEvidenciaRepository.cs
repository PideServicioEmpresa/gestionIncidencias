namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;

public interface ITicketEvidenciaRepository
{
    Task<TicketEvidencia?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TicketEvidencia>> ListarPorTicketAsync(Guid ticketId, EvidenciaTipo? tipo = null, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CrearAsync(TicketEvidencia evidencia, CancellationToken ct = default);
    Task ActualizarAsync(TicketEvidencia evidencia, CancellationToken ct = default);
}
