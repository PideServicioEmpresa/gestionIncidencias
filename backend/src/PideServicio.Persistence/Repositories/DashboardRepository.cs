namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Features.Dashboard.DTOs;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly IDbConnectionFactory _db;

    public DashboardRepository(IDbConnectionFactory db)
    {
        _db = db;
    }

    private sealed record KpiRow
    {
        public int TotalAbiertos { get; init; }
        public int TotalCerrados { get; init; }
        public int Total { get; init; }
        public int Criticos { get; init; }
        public int CerradosHoy { get; init; }
    }

    public async Task<(int TotalAbiertos, int TotalCerrados, int Total, int Criticos, int CerradosHoy)>
        ObtenerKpisAsync(Guid? empresaId, Guid? sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                COUNT(*) FILTER (WHERE estado::text NOT IN ('CERRADO', 'CANCELADO'))::int AS "TotalAbiertos",
                COUNT(*) FILTER (WHERE estado::text = 'CERRADO')::int                    AS "TotalCerrados",
                COUNT(*)::int                                                             AS "Total",
                COUNT(*) FILTER (WHERE prioridad_efectiva::text = 'CRITICA'
                                   AND estado::text NOT IN ('CERRADO', 'CANCELADO'))::int AS "Criticos",
                COUNT(*) FILTER (WHERE estado::text = 'CERRADO'
                                   AND fecha_cierre::date = CURRENT_DATE)::int            AS "CerradosHoy"
            FROM tickets
            WHERE deleted_at IS NULL
              AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
              AND (@SucursalId IS NULL OR sucursal_id = @SucursalId);
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var row = await cn.QuerySingleOrDefaultAsync<KpiRow>(sql, new { EmpresaId = empresaId, SucursalId = sucursalId });

        return row is null
            ? (0, 0, 0, 0, 0)
            : (row.TotalAbiertos, row.TotalCerrados, row.Total, row.Criticos, row.CerradosHoy);
    }

    public async Task<IReadOnlyList<ContadorEstadoDto>>
        ObtenerPorEstadoAsync(Guid? empresaId, Guid? sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT estado::text AS "Estado", COUNT(*)::int AS "Total"
            FROM tickets
            WHERE deleted_at IS NULL
              AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
              AND (@SucursalId IS NULL OR sucursal_id = @SucursalId)
            GROUP BY estado
            ORDER BY "Total" DESC;
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<ContadorEstadoDto>(sql, new { EmpresaId = empresaId, SucursalId = sucursalId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ContadorPrioridadDto>>
        ObtenerPorPrioridadAsync(Guid? empresaId, Guid? sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT prioridad_efectiva::text AS "Prioridad", COUNT(*)::int AS "Total"
            FROM tickets
            WHERE deleted_at IS NULL
              AND estado::text NOT IN ('CERRADO', 'CANCELADO')
              AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
              AND (@SucursalId IS NULL OR sucursal_id = @SucursalId)
            GROUP BY prioridad_efectiva
            ORDER BY "Total" DESC;
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<ContadorPrioridadDto>(sql, new { EmpresaId = empresaId, SucursalId = sucursalId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ContadorSucursalDto>>
        ObtenerPorSucursalAsync(Guid? empresaId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT t.sucursal_id AS "SucursalId", s.nombre AS "SucursalNombre", COUNT(*)::int AS "Total"
            FROM tickets t
            INNER JOIN sucursales s ON s.id = t.sucursal_id AND s.deleted_at IS NULL
            WHERE t.deleted_at IS NULL
              AND (@EmpresaId IS NULL OR t.empresa_id = @EmpresaId)
            GROUP BY t.sucursal_id, s.nombre
            ORDER BY "Total" DESC;
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<ContadorSucursalDto>(sql, new { EmpresaId = empresaId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ContadorAreaDto>>
        ObtenerPorAreaAsync(Guid? empresaId, Guid? sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                t.area_id     AS "AreaId",
                a.nombre      AS "AreaNombre",
                t.sucursal_id AS "SucursalId",
                COUNT(*) FILTER (WHERE t.estado::text NOT IN ('CERRADO', 'CANCELADO'))::int AS "Abiertos",
                COUNT(*) FILTER (WHERE t.estado::text = 'CERRADO')::int                    AS "Cerrados"
            FROM tickets t
            INNER JOIN areas a ON a.id = t.area_id AND a.deleted_at IS NULL
            WHERE t.deleted_at IS NULL
              AND (@EmpresaId  IS NULL OR t.empresa_id  = @EmpresaId)
              AND (@SucursalId IS NULL OR t.sucursal_id = @SucursalId)
            GROUP BY t.area_id, a.nombre, t.sucursal_id
            ORDER BY 4 + 5 DESC;
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<ContadorAreaDto>(sql, new { EmpresaId = empresaId, SucursalId = sucursalId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ContadorTipoServicioDto>>
        ObtenerPorTipoServicioAsync(Guid? empresaId, Guid? sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                t.tipo_servicio_id AS "TipoServicioId",
                ts.nombre          AS "TipoServicioNombre",
                COUNT(*)::int      AS "Total"
            FROM tickets t
            INNER JOIN tipos_servicio ts ON ts.id = t.tipo_servicio_id AND ts.deleted_at IS NULL
            WHERE t.deleted_at IS NULL
              AND (@EmpresaId  IS NULL OR t.empresa_id  = @EmpresaId)
              AND (@SucursalId IS NULL OR t.sucursal_id = @SucursalId)
            GROUP BY t.tipo_servicio_id, ts.nombre
            ORDER BY "Total" DESC;
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<ContadorTipoServicioDto>(sql, new { EmpresaId = empresaId, SucursalId = sucursalId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ContadorTecnicoDto>>
        ObtenerPorTecnicoAsync(Guid? empresaId, Guid? sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                u.id AS "TecnicoId",
                (u.nombre || ' ' || u.apellido) AS "TecnicoNombre",
                COUNT(*)::int AS "Total"
            FROM tickets t
            INNER JOIN usuarios u ON u.id = t.tecnico_id
            WHERE t.deleted_at IS NULL
              AND t.tecnico_id IS NOT NULL
              AND t.estado::text NOT IN ('CERRADO', 'CANCELADO')
              AND (@EmpresaId  IS NULL OR t.empresa_id  = @EmpresaId)
              AND (@SucursalId IS NULL OR t.sucursal_id = @SucursalId)
            GROUP BY u.id, u.nombre, u.apellido
            ORDER BY "Total" DESC
            LIMIT 10;
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<ContadorTecnicoDto>(sql, new { EmpresaId = empresaId, SucursalId = sucursalId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<PuntoTendenciaDto>>
        ObtenerTendenciaDiariaAsync(Guid? empresaId, Guid? sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            WITH dias AS (
                SELECT generate_series(CURRENT_DATE - 15, CURRENT_DATE, '1 day')::date AS d
            ),
            creados AS (
                SELECT fecha_creacion::date AS dia, COUNT(*)::int AS total
                FROM tickets
                WHERE deleted_at IS NULL
                  AND fecha_creacion::date >= CURRENT_DATE - 15
                  AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
                  AND (@SucursalId IS NULL OR sucursal_id = @SucursalId)
                GROUP BY fecha_creacion::date
            ),
            resueltos AS (
                SELECT fecha_cierre::date AS dia, COUNT(*)::int AS total
                FROM tickets
                WHERE deleted_at IS NULL
                  AND fecha_cierre IS NOT NULL
                  AND fecha_cierre::date >= CURRENT_DATE - 15
                  AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
                  AND (@SucursalId IS NULL OR sucursal_id = @SucursalId)
                GROUP BY fecha_cierre::date
            )
            SELECT
                to_char(d.d, 'DD/MM')    AS "Fecha",
                COALESCE(c.total, 0)     AS "Creados",
                COALESCE(r.total, 0)     AS "Resueltos"
            FROM dias d
            LEFT JOIN creados   c ON c.dia = d.d
            LEFT JOIN resueltos r ON r.dia = d.d
            ORDER BY d.d;
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<PuntoTendenciaDto>(sql, new { EmpresaId = empresaId, SucursalId = sucursalId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<PuntoSemanalDto>>
        ObtenerTendenciaSemanalAsync(Guid? empresaId, Guid? sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            WITH semanas AS (
                SELECT
                    date_trunc('week', CURRENT_DATE::timestamp - (n || ' weeks')::interval)::date AS inicio,
                    n AS idx
                FROM generate_series(3, 0, -1) AS n
            ),
            creados_sem AS (
                SELECT date_trunc('week', fecha_creacion)::date AS semana, COUNT(*)::int AS total
                FROM tickets
                WHERE deleted_at IS NULL
                  AND fecha_creacion >= date_trunc('week', CURRENT_TIMESTAMP) - INTERVAL '3 weeks'
                  AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
                  AND (@SucursalId IS NULL OR sucursal_id = @SucursalId)
                GROUP BY date_trunc('week', fecha_creacion)::date
            ),
            cerrados_sem AS (
                SELECT date_trunc('week', fecha_cierre)::date AS semana, COUNT(*)::int AS total
                FROM tickets
                WHERE deleted_at IS NULL
                  AND fecha_cierre IS NOT NULL
                  AND fecha_cierre >= date_trunc('week', CURRENT_TIMESTAMP) - INTERVAL '3 weeks'
                  AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
                  AND (@SucursalId IS NULL OR sucursal_id = @SucursalId)
                GROUP BY date_trunc('week', fecha_cierre)::date
            )
            SELECT
                ('Sem ' || (4 - s.idx)) AS "Semana",
                COALESCE(c.total, 0)    AS "Creados",
                COALESCE(r.total, 0)    AS "Resueltos"
            FROM semanas s
            LEFT JOIN creados_sem  c ON c.semana = s.inicio
            LEFT JOIN cerrados_sem r ON r.semana = s.inicio
            ORDER BY s.inicio;
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<PuntoSemanalDto>(sql, new { EmpresaId = empresaId, SucursalId = sucursalId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<SparklineRowDto>>
        ObtenerSparklineAsync(Guid? empresaId, Guid? sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            WITH dias AS (
                SELECT generate_series(CURRENT_DATE - 6, CURRENT_DATE, '1 day')::date AS d
            ),
            creados_dia AS (
                SELECT fecha_creacion::date AS dia, COUNT(*)::int AS total
                FROM tickets
                WHERE deleted_at IS NULL
                  AND fecha_creacion::date >= CURRENT_DATE - 6
                  AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
                  AND (@SucursalId IS NULL OR sucursal_id = @SucursalId)
                GROUP BY fecha_creacion::date
            ),
            criticos_dia AS (
                SELECT fecha_creacion::date AS dia, COUNT(*)::int AS total
                FROM tickets
                WHERE deleted_at IS NULL
                  AND fecha_creacion::date >= CURRENT_DATE - 6
                  AND prioridad_efectiva::text = 'CRITICA'
                  AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
                  AND (@SucursalId IS NULL OR sucursal_id = @SucursalId)
                GROUP BY fecha_creacion::date
            ),
            cerrados_dia AS (
                SELECT fecha_cierre::date AS dia, COUNT(*)::int AS total
                FROM tickets
                WHERE deleted_at IS NULL
                  AND fecha_cierre IS NOT NULL
                  AND fecha_cierre::date >= CURRENT_DATE - 6
                  AND (@EmpresaId  IS NULL OR empresa_id  = @EmpresaId)
                  AND (@SucursalId IS NULL OR sucursal_id = @SucursalId)
                GROUP BY fecha_cierre::date
            )
            SELECT
                COALESCE(c.total,  0) AS "Abiertos",
                COALESCE(cr.total, 0) AS "Criticos",
                COALESCE(ce.total, 0) AS "Cerrados"
            FROM dias d
            LEFT JOIN creados_dia  c  ON c.dia  = d.d
            LEFT JOIN criticos_dia cr ON cr.dia = d.d
            LEFT JOIN cerrados_dia ce ON ce.dia = d.d
            ORDER BY d.d;
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<SparklineRowDto>(sql, new { EmpresaId = empresaId, SucursalId = sucursalId });
        return rows.ToList().AsReadOnly();
    }
}
