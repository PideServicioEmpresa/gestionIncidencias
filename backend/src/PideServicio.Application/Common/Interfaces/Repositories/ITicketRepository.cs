namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;

public sealed record TicketConsultaParams(
    Guid? EmpresaId = null,
    Guid? SucursalId = null,
    Guid? AreaId = null,
    Guid? TecnicoId = null,
    Guid? SolicitanteId = null,
    TicketEstadoTipo? Estado = null,
    PrioridadTipo? Prioridad = null,
    DateTimeOffset? FechaDesde = null,
    DateTimeOffset? FechaHasta = null,
    string? BusquedaTexto = null
);

public interface ITicketRepository
{
    Task<Ticket?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Ticket?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default);
    Task<string> GenerarCodigoAsync(CancellationToken ct = default);
    Task<PagedResult<Ticket>> ListarAsync(TicketConsultaParams filtros, int pagina, int tamanoPagina, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteCodigoAsync(string codigo, CancellationToken ct = default);
    Task<Guid> CrearAsync(Ticket ticket, CancellationToken ct = default);
    Task ActualizarAsync(Ticket ticket, CancellationToken ct = default);
}
