namespace PideServicio.Application.Features.Dashboard.Queries.GetTicketsReporte;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Dashboard.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetTicketsReporteQueryHandler
    : IQueryHandler<GetTicketsReporteQuery, IReadOnlyList<TicketReporteItemDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IDashboardRepository _dashboardRepository;

    public GetTicketsReporteQueryHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        IDashboardRepository dashboardRepository)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _dashboardRepository = dashboardRepository;
    }

    public async Task<Result<IReadOnlyList<TicketReporteItemDto>>> Handle(
        GetTicketsReporteQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<IReadOnlyList<TicketReporteItemDto>>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);

        if (actor is null || !actor.Activo)
            return Result.NoAutorizado<IReadOnlyList<TicketReporteItemDto>>();

        Guid? empresaId;
        Guid? sucursalId;

        if (actor.Rol == RolTipo.SUPERADMIN)
        {
            empresaId = request.EmpresaId;
            sucursalId = request.SucursalId;
        }
        else if (actor.Rol is RolTipo.ADMIN or RolTipo.SUPERVISOR)
        {
            empresaId = actor.EmpresaId;
            sucursalId = request.SucursalId;
        }
        else
        {
            return Result.NoPermitido<IReadOnlyList<TicketReporteItemDto>>("No tiene permisos para exportar reportes.");
        }

        var tickets = await _dashboardRepository.ObtenerParaReporteAsync(
            empresaId, sucursalId, request.FechaDesde, request.FechaHasta, cancellationToken);

        return Result.Exito(tickets);
    }
}
