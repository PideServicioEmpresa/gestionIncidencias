namespace PideServicio.Api.Controllers.V1.Auditoria;

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PideServicio.Api.Controllers.Common;
using PideServicio.Application.Features.Auditoria.DTOs;
using PideServicio.Application.Features.Auditoria.Queries.ListAuditLogs;
using PideServicio.Contracts.Common;

/// <summary>Consulta del log de auditoría del sistema. Exclusivo para Administradores y SuperAdministradores.</summary>
[ApiVersion("1.0")]
[Tags("Auditoría")]
public sealed class AuditoriaController : ApiControllerBase
{
    /// <summary>Lista el log de auditoría con filtros opcionales.</summary>
    /// <remarks>
    /// Permite filtrar por tabla, registro específico, usuario que realizó el cambio y rango de fechas.
    /// Todos los filtros son opcionales y acumulables.
    /// Resultado ordenado por fecha de creación descendente.
    /// </remarks>
    [HttpGet]
    [Authorize(Policy = "AdminOSuperior")]
    [ProducesResponseType(typeof(PagedResponse<AuditLogDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 403)]
    public async Task<IActionResult> Listar(
        [FromQuery] string? tabla,
        [FromQuery] Guid? registroId,
        [FromQuery] Guid? usuarioId,
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 50,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new ListAuditLogsQuery(tabla, registroId, usuarioId, desde, hasta, pagina, tamanoPagina), ct);

        if (result.EsFallido) return HandleResult(result);
        return OkPaged(result.Valor!);
    }
}
