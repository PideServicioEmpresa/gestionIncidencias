namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;

public interface IAuditLogRepository
{
    Task<PagedResult<AuditLog>> ListarAsync(
        Guid empresaId,
        string? tabla,
        Guid? registroId,
        Guid? usuarioId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        int pagina,
        int tamanoPagina,
        CancellationToken ct = default);
    Task<Guid> CrearAsync(AuditLog log, CancellationToken ct = default);
}
