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
        Guid? areaId = request.AreaId;

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

        // Métodos de agregación — aplican filtro de fechas cuando se proveen (7)
        var kpisTask            = _dashboardRepository.ObtenerKpisAsync(empresaId, sucursalId, areaId, request.FechaDesde, request.FechaHasta, cancellationToken);
        var porEstadoTask       = _dashboardRepository.ObtenerPorEstadoAsync(empresaId, sucursalId, areaId, request.FechaDesde, request.FechaHasta, cancellationToken);
        var porPrioridadTask    = _dashboardRepository.ObtenerPorPrioridadAsync(empresaId, sucursalId, areaId, request.FechaDesde, request.FechaHasta, cancellationToken);
        var porSucursalTask     = _dashboardRepository.ObtenerPorSucursalAsync(empresaId, request.FechaDesde, request.FechaHasta, cancellationToken);
        var porAreaTask         = _dashboardRepository.ObtenerPorAreaAsync(empresaId, sucursalId, areaId, request.FechaDesde, request.FechaHasta, cancellationToken);
        var porTipoServicioTask = _dashboardRepository.ObtenerPorTipoServicioAsync(empresaId, sucursalId, areaId, request.FechaDesde, request.FechaHasta, cancellationToken);
        var porTecnicoTask      = _dashboardRepository.ObtenerPorTecnicoAsync(empresaId, sucursalId, areaId, request.FechaDesde, request.FechaHasta, cancellationToken);

        // Métodos de tendencia — ventana fija de tiempo, sin filtro de fechas (3)
        var tendenciaDiariaTask = _dashboardRepository.ObtenerTendenciaDiariaAsync(empresaId, sucursalId, areaId, cancellationToken);
        var tendenciaSemTask    = _dashboardRepository.ObtenerTendenciaSemanalAsync(empresaId, sucursalId, areaId, cancellationToken);
        var sparklineTask       = _dashboardRepository.ObtenerSparklineAsync(empresaId, sucursalId, areaId, cancellationToken);

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

        // Contrato explícito con el frontend: los arreglos que alimentan gráficos nunca
        // viajan como null. Con la base sin tickets todos salen vacíos, no nulos.
        static IReadOnlyList<T> Vacio<T>(IReadOnlyList<T>? lista) => lista ?? [];

        var dto = new DashboardResumenDto(
            TotalAbiertos:    totalAbiertos,
            TotalCerrados:    totalCerrados,
            Total:            total,
            Criticos:         criticos,
            CerradosHoy:      cerradosHoy,
            TasaResolucionPct: tasaResolucion,
            PorEstado:        Vacio(porEstado),
            PorPrioridad:     Vacio(porPrioridad),
            PorSucursal:      Vacio(porSucursal),
            PorArea:          Vacio(porArea),
            PorTipoServicio:  Vacio(porTipoServicio),
            PorTecnico:       Vacio(porTecnico),
            Tendencia16Dias:  Vacio(tendenciaDiaria),
            TendenciaSemanal: Vacio(tendenciaSem),
            SparkAbiertos:    Vacio(sparkAbiertos),
            SparkCriticos:    Vacio(sparkCriticos),
            SparkCerrados:    Vacio(sparkCerrados)
        );

        return Result.Exito(dto);
    }
}
