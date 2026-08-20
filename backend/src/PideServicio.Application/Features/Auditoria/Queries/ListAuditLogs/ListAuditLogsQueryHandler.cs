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
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;

    public ListAuditLogsQueryHandler(
        IAuditLogRepository auditLogRepository,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService)
    {
        _auditLogRepository = auditLogRepository;
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> Handle(
        ListAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<PagedResult<AuditLogDto>>();

        // El rol ya lo validó la política AdminOSuperior contra la BD. Se recarga el
        // usuario porque el ÁMBITO de los datos depende de su rol y empresa reales.
        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<PagedResult<AuditLogDto>>();

        try
        {
            var pagina = request.Pagina < 1 ? 1 : request.Pagina;
            var tamanoPagina = request.TamanoPagina < 1 ? 20
                : request.TamanoPagina > 100 ? 100
                : request.TamanoPagina;

            // SuperAdmin consulta sin filtro de empresa; Admin solo la suya.
            var empresaId = actorDb.Rol == RolTipo.SUPERADMIN ? Guid.Empty : actorDb.EmpresaId;

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
