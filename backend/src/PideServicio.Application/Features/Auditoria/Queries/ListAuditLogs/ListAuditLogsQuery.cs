namespace PideServicio.Application.Features.Auditoria.Queries.ListAuditLogs;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Auditoria.DTOs;

public sealed record ListAuditLogsQuery(
    string? Tabla,
    Guid? RegistroId,
    Guid? UsuarioId,
    DateTimeOffset? Desde,
    DateTimeOffset? Hasta,
    int Pagina,
    int TamanoPagina) : IQuery<PagedResult<AuditLogDto>>;
