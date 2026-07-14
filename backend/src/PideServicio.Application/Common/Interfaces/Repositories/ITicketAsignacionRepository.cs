namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Domain.Entities;

public interface ITicketAsignacionRepository
{
    Task<IReadOnlyList<TicketAsignacion>> ListarPorTicketAsync(Guid ticketId, CancellationToken ct = default);
    Task<Guid> CrearAsync(TicketAsignacion asignacion, CancellationToken ct = default);
}
