namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Domain.Entities;

public interface ITicketHistorialRepository
{
    Task<IReadOnlyList<TicketHistorialEntrada>> ListarPorTicketAsync(Guid ticketId, CancellationToken ct = default);
    Task<Guid> CrearAsync(TicketHistorialEntrada entrada, CancellationToken ct = default);
}
