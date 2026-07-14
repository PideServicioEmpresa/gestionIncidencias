namespace PideServicio.Application.Features.Dashboard.Queries.GetDashboardResumen;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Dashboard.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetDashboardResumenQueryHandler
    : IQueryHandler<GetDashboardResumenQuery, DashboardResumenDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IDashboardRepository _dashboardRepository;

    public GetDashboardResumenQueryHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        IDashboardRepository dashboardRepository)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _dashboardRepository = dashboardRepository;
    }

    public async Task<Result<DashboardResumenDto>> Handle(
        GetDashboardResumenQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<DashboardResumenDto>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);

        if (actor is null || !actor.Activo)
            return Result.NoAutorizado<DashboardResumenDto>();

        Guid? empresaId;
        Guid? sucursalId;

        if (actor.Rol == RolTipo.SUPERADMIN)
        {
            // SuperAdmin puede ver todas las empresas o filtrar por una específica
            empresaId = request.EmpresaId;
            sucursalId = request.SucursalId;
        }
        else if (actor.Rol is RolTipo.ADMIN or RolTipo.SUPERVISOR)
        {
            // Admin y Supervisor están restringidos a su propia empresa
            empresaId = actor.EmpresaId;
            sucursalId = request.SucursalId;
        }
        else
        {
            return Result.NoPermitido<DashboardResumenDto>("No tiene permisos para acceder al dashboard.");
        }

        var kpisTask            = _dashboardRepository.ObtenerKpisAsync(empresaId, sucursalId, cancellationToken);
        var porEstadoTask       = _dashboardRepository.ObtenerPorEstadoAsync(empresaId, sucursalId, cancellationToken);
        var porPrioridadTask    = _dashboardRepository.ObtenerPorPrioridadAsync(empresaId, sucursalId, cancellationToken);
        var porSucursalTask     = _dashboardRepository.ObtenerPorSucursalAsync(empresaId, cancellationToken);
        var porAreaTask         = _dashboardRepository.ObtenerPorAreaAsync(empresaId, sucursalId, cancellationToken);
        var porTipoServicioTask = _dashboardRepository.ObtenerPorTipoServicioAsync(empresaId, sucursalId, cancellationToken);
        var porTecnicoTask      = _dashboardRepository.ObtenerPorTecnicoAsync(empresaId, sucursalId, cancellationToken);
        var tendenciaDiariaTask = _dashboardRepository.ObtenerTendenciaDiariaAsync(empresaId, sucursalId, cancellationToken);
        var tendenciaSemTask    = _dashboardRepository.ObtenerTendenciaSemanalAsync(empresaId, sucursalId, cancellationToken);
        var sparklineTask       = _dashboardRepository.ObtenerSparklineAsync(empresaId, sucursalId, cancellationToken);

        await Task.WhenAll(
            kpisTask,
            porEstadoTask,
            porPrioridadTask,
            porSucursalTask,
            porAreaTask,
            porTipoServicioTask,
            porTecnicoTask,
            tendenciaDiariaTask,
            tendenciaSemTask,
            sparklineTask);

        var (totalAbiertos, totalCerrados, total, criticos, cerradosHoy) = await kpisTask;
        var porEstado       = await porEstadoTask;
        var porPrioridad    = await porPrioridadTask;
        var porSucursal     = await porSucursalTask;
        var porArea         = await porAreaTask;
        var porTipoServicio = await porTipoServicioTask;
        var porTecnico      = await porTecnicoTask;
        var tendenciaDiaria = await tendenciaDiariaTask;
        var tendenciaSem    = await tendenciaSemTask;
        var sparkline       = await sparklineTask;

        var tasaResolucion = total > 0
            ? (int)Math.Round((double)totalCerrados / total * 100)
            : 0;

        var sparkAbiertos = sparkline.Select(r => r.Abiertos).ToList();
        var sparkCriticos = sparkline.Select(r => r.Criticos).ToList();
        var sparkCerrados = sparkline.Select(r => r.Cerrados).ToList();

        var dto = new DashboardResumenDto(
            TotalAbiertos:    totalAbiertos,
            TotalCerrados:    totalCerrados,
            Total:            total,
            Criticos:         criticos,
            CerradosHoy:      cerradosHoy,
            TasaResolucionPct: tasaResolucion,
            PorEstado:        porEstado,
            PorPrioridad:     porPrioridad,
            PorSucursal:      porSucursal,
            PorArea:          porArea,
            PorTipoServicio:  porTipoServicio,
            PorTecnico:       porTecnico,
            Tendencia16Dias:  tendenciaDiaria,
            TendenciaSemanal: tendenciaSem,
            SparkAbiertos:    sparkAbiertos,
            SparkCriticos:    sparkCriticos,
            SparkCerrados:    sparkCerrados
        );

        return Result.Exito(dto);
    }
}
