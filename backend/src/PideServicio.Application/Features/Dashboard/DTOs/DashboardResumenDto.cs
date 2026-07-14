namespace PideServicio.Application.Features.Dashboard.DTOs;

public sealed record ContadorEstadoDto(string Estado, int Total);
public sealed record ContadorPrioridadDto(string Prioridad, int Total);
public sealed record ContadorSucursalDto(Guid SucursalId, string SucursalNombre, int Total);
public sealed record ContadorAreaDto(Guid AreaId, string AreaNombre, Guid SucursalId, int Abiertos, int Cerrados);
public sealed record ContadorTipoServicioDto(Guid TipoServicioId, string TipoServicioNombre, int Total);
public sealed record ContadorTecnicoDto(Guid TecnicoId, string TecnicoNombre, int Total);
public sealed record PuntoTendenciaDto(string Fecha, int Creados, int Resueltos);
public sealed record PuntoSemanalDto(string Semana, int Creados, int Resueltos);
public sealed record SparklineRowDto(int Abiertos, int Criticos, int Cerrados);

public sealed record DashboardResumenDto(
    int TotalAbiertos,
    int TotalCerrados,
    int Total,
    int Criticos,
    int CerradosHoy,
    int TasaResolucionPct,
    IReadOnlyList<ContadorEstadoDto> PorEstado,
    IReadOnlyList<ContadorPrioridadDto> PorPrioridad,
    IReadOnlyList<ContadorSucursalDto> PorSucursal,
    IReadOnlyList<ContadorAreaDto> PorArea,
    IReadOnlyList<ContadorTipoServicioDto> PorTipoServicio,
    IReadOnlyList<ContadorTecnicoDto> PorTecnico,
    IReadOnlyList<PuntoTendenciaDto> Tendencia16Dias,
    IReadOnlyList<PuntoSemanalDto> TendenciaSemanal,
    IReadOnlyList<int> SparkAbiertos,
    IReadOnlyList<int> SparkCriticos,
    IReadOnlyList<int> SparkCerrados
);
