namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Features.Dashboard.DTOs;

public interface IDashboardRepository
{
    Task<(int TotalAbiertos, int TotalCerrados, int Total, int Criticos, int CerradosHoy)>
        ObtenerKpisAsync(Guid? empresaId, Guid? sucursalId, Guid? areaId, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null, CancellationToken ct = default);

    Task<IReadOnlyList<ContadorEstadoDto>>
        ObtenerPorEstadoAsync(Guid? empresaId, Guid? sucursalId, Guid? areaId, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null, CancellationToken ct = default);

    Task<IReadOnlyList<ContadorPrioridadDto>>
        ObtenerPorPrioridadAsync(Guid? empresaId, Guid? sucursalId, Guid? areaId, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null, CancellationToken ct = default);

    Task<IReadOnlyList<ContadorSucursalDto>>
        ObtenerPorSucursalAsync(Guid? empresaId, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null, CancellationToken ct = default);

    Task<IReadOnlyList<ContadorAreaDto>>
        ObtenerPorAreaAsync(Guid? empresaId, Guid? sucursalId, Guid? areaId, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null, CancellationToken ct = default);

    Task<IReadOnlyList<ContadorTipoServicioDto>>
        ObtenerPorTipoServicioAsync(Guid? empresaId, Guid? sucursalId, Guid? areaId, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null, CancellationToken ct = default);

    Task<IReadOnlyList<ContadorTecnicoDto>>
        ObtenerPorTecnicoAsync(Guid? empresaId, Guid? sucursalId, Guid? areaId, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null, CancellationToken ct = default);

    Task<IReadOnlyList<PuntoTendenciaDto>>
        ObtenerTendenciaDiariaAsync(Guid? empresaId, Guid? sucursalId, Guid? areaId, CancellationToken ct = default);

    Task<IReadOnlyList<PuntoSemanalDto>>
        ObtenerTendenciaSemanalAsync(Guid? empresaId, Guid? sucursalId, Guid? areaId, CancellationToken ct = default);

    Task<IReadOnlyList<SparklineRowDto>>
        ObtenerSparklineAsync(Guid? empresaId, Guid? sucursalId, Guid? areaId, CancellationToken ct = default);
}
