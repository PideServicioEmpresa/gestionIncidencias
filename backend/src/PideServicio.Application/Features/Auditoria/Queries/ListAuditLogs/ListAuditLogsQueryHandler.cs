namespace PideServicio.Application.Features.Auditoria.Queries.ListAuditLogs;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Auditoria.DTOs;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ListAuditLogsQueryHandler
    : IQueryHandler<ListAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICurrentUserService _currentUserService;

    public ListAuditLogsQueryHandler(
        IAuditLogRepository auditLogRepository,
        ICurrentUserService currentUserService)
    {
        _auditLogRepository = auditLogRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> Handle(
        ListAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var usuario = _currentUserService.UsuarioActual;
        if (usuario is null)
            return Result.NoAutorizado<PagedResult<AuditLogDto>>();

        if (usuario.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<PagedResult<AuditLogDto>>(
                "Solo Administradores y SuperAdministradores pueden consultar los logs de auditoría.");

        try
        {
            var pagina = request.Pagina < 1 ? 1 : request.Pagina;
            var tamanoPagina = request.TamanoPagina < 1 ? 20
                : request.TamanoPagina > 100 ? 100
                : request.TamanoPagina;

            // SuperAdmin consulta sin filtro de empresa; Admin solo la suya.
            var empresaId = usuario.EsSuperAdmin ? Guid.Empty : usuario.EmpresaId;

            var resultado = await _auditLogRepository.ListarAsync(
                empresaId,
                request.Tabla,
                request.RegistroId,
                request.UsuarioId,
                request.Desde,
                request.Hasta,
                pagina,
                tamanoPagina,
                cancellationToken);

            var dtos = new PagedResult<AuditLogDto>
            {
                Items = resultado.Items.Adapt<IReadOnlyList<AuditLogDto>>(),
                Pagina = resultado.Pagina,
                TamanoPagina = resultado.TamanoPagina,
                TotalRegistros = resultado.TotalRegistros
            };

            return Result.Exito(dtos);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<PagedResult<AuditLogDto>>(ex.Message);
        }
    }
}
