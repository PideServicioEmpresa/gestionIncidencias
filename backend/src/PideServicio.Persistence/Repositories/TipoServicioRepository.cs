namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

public sealed class TipoServicioRepository : ITipoServicioRepository
{
    private readonly IDbConnectionFactory _db;

    public TipoServicioRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────
    private sealed record TipoServicioRow
    {
        public Guid            Id          { get; init; }
        public Guid            EmpresaId   { get; init; }
        public string          Nombre      { get; init; } = "";
        public string?         Descripcion { get; init; }
        public int             Orden       { get; init; }
        public bool            Activo      { get; init; }
        public DateTimeOffset  CreatedAt   { get; init; }
        public DateTimeOffset? UpdatedAt   { get; init; }
        public Guid?           CreatedBy   { get; init; }
        public Guid?           UpdatedBy   { get; init; }
        public DateTimeOffset? DeletedAt   { get; init; }
        public Guid?           DeletedBy   { get; init; }
    }

    private const string SelectCols = """
        id          AS "Id",
        empresa_id  AS "EmpresaId",
        nombre      AS "Nombre",
        descripcion AS "Descripcion",
        orden       AS "Orden",
        activo      AS "Activo",
        created_at  AS "CreatedAt",
        updated_at  AS "UpdatedAt",
        created_by  AS "CreatedBy",
        updated_by  AS "UpdatedBy",
        deleted_at  AS "DeletedAt",
        deleted_by  AS "DeletedBy"
        """;

    private static TipoServicio MapearEntidad(TipoServicioRow r)
    {
        var e = EntityReconstituter.Crear<TipoServicio>();
        EntityReconstituter.Set(e, "Id",          r.Id);
        EntityReconstituter.Set(e, "EmpresaId",   r.EmpresaId);
        EntityReconstituter.Set(e, "Nombre",      r.Nombre);
        EntityReconstituter.Set(e, "Descripcion", r.Descripcion);
        EntityReconstituter.Set(e, "Orden",       r.Orden);
        EntityReconstituter.Set(e, "Activo",      r.Activo);
        EntityReconstituter.Set(e, "CreatedAt",   r.CreatedAt);
        EntityReconstituter.Set(e, "UpdatedAt",   r.UpdatedAt);
        EntityReconstituter.Set(e, "CreatedBy",   r.CreatedBy);
        EntityReconstituter.Set(e, "UpdatedBy",   r.UpdatedBy);
        EntityReconstituter.Set(e, "DeletedAt",   r.DeletedAt);
        EntityReconstituter.Set(e, "DeletedBy",   r.DeletedBy);
        return e;
    }

    // ── Consultas ─────────────────────────────────────────────────────────────
    public async Task<TipoServicio?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        string sql = $"""
            SELECT {SelectCols}
            FROM tipos_servicio
            WHERE id = @Id
              AND deleted_at IS NULL
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var row = await cn.QuerySingleOrDefaultAsync<TipoServicioRow>(sql, new { Id = id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<TipoServicio>> ListarActivosPorEmpresaAsync(
        Guid empresaId, CancellationToken ct = default)
    {
        string sql = $"""
            SELECT {SelectCols}
            FROM tipos_servicio
            WHERE empresa_id = @EmpresaId
              AND activo = true
              AND deleted_at IS NULL
            ORDER BY orden
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<TipoServicioRow>(sql, new { EmpresaId = empresaId });
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<PagedResult<TipoServicio>> ListarAsync(
        Guid empresaId, int pagina, int tamanoPagina, bool? soloActivos = null, string? busqueda = null, CancellationToken ct = default)
    {
        const string countSql = """
            SELECT COUNT(*)
            FROM tipos_servicio
            WHERE empresa_id = @EmpresaId
              AND deleted_at IS NULL
              AND (@SoloActivos::boolean IS NULL OR activo = @SoloActivos)
              AND (@Busqueda IS NULL OR nombre ILIKE '%' || @Busqueda || '%'
                                    OR descripcion ILIKE '%' || @Busqueda || '%')
            """;

        string itemsSql = $"""
            SELECT {SelectCols}
            FROM tipos_servicio
            WHERE empresa_id = @EmpresaId
              AND deleted_at IS NULL
              AND (@SoloActivos::boolean IS NULL OR activo = @SoloActivos)
              AND (@Busqueda IS NULL OR nombre ILIKE '%' || @Busqueda || '%'
                                    OR descripcion ILIKE '%' || @Busqueda || '%')
            ORDER BY orden
            LIMIT @TamanoPagina OFFSET @Offset
            """;

        var param = new
        {
            EmpresaId    = empresaId,
            SoloActivos  = soloActivos,
            Busqueda     = busqueda,
            TamanoPagina = tamanoPagina,
            Offset       = (pagina - 1) * tamanoPagina
        };

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var total = (int)await cn.ExecuteScalarAsync<long>(countSql, param);
        var rows  = await cn.QueryAsync<TipoServicioRow>(itemsSql, param);

        return new PagedResult<TipoServicio>
        {
            Items          = rows.Select(MapearEntidad).ToList().AsReadOnly(),
            Pagina         = pagina,
            TamanoPagina   = tamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM tipos_servicio
                WHERE id = @Id AND deleted_at IS NULL
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql, new { Id = id });
    }

    public async Task<bool> ExisteNombreAsync(
        Guid empresaId, string nombre, Guid? excludeId = null, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM tipos_servicio
                WHERE empresa_id = @EmpresaId
                  AND nombre = @Nombre
                  AND deleted_at IS NULL
                  AND (@ExcludeId::uuid IS NULL OR id <> @ExcludeId)
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql,
            new { EmpresaId = empresaId, Nombre = nombre, ExcludeId = excludeId });
    }

    public async Task<bool> ExisteOrdenAsync(
        Guid empresaId, int orden, Guid? excludeId = null, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM tipos_servicio
                WHERE empresa_id = @EmpresaId
                  AND orden = @Orden
                  AND deleted_at IS NULL
                  AND (@ExcludeId::uuid IS NULL OR id <> @ExcludeId)
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql,
            new { EmpresaId = empresaId, Orden = orden, ExcludeId = excludeId });
    }

    public async Task<bool> PerteneceAEmpresaAsync(
        Guid tipoServicioId, Guid empresaId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM tipos_servicio
                WHERE id = @Id
                  AND empresa_id = @EmpresaId
                  AND deleted_at IS NULL
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql,
            new { Id = tipoServicioId, EmpresaId = empresaId });
    }

    // ── Escritura ─────────────────────────────────────────────────────────────
    public async Task<Guid> CrearAsync(TipoServicio tipoServicio, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO tipos_servicio
                (id, empresa_id, nombre, descripcion, orden, activo,
                 created_at, updated_at, created_by)
            VALUES
                (@Id, @EmpresaId, @Nombre, @Descripcion, @Orden, @Activo,
                 @CreatedAt, @UpdatedAt, @CreatedBy)
            RETURNING id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            tipoServicio.Id,
            tipoServicio.EmpresaId,
            tipoServicio.Nombre,
            tipoServicio.Descripcion,
            tipoServicio.Orden,
            tipoServicio.Activo,
            tipoServicio.CreatedAt,
            tipoServicio.UpdatedAt,
            tipoServicio.CreatedBy
        });
    }

    public async Task ActualizarAsync(TipoServicio tipoServicio, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE tipos_servicio
            SET nombre      = @Nombre,
                descripcion = @Descripcion,
                orden       = @Orden,
                activo      = @Activo,
                updated_at  = @UpdatedAt,
                updated_by  = @UpdatedBy,
                deleted_at  = @DeletedAt,
                deleted_by  = @DeletedBy
            WHERE id = @Id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        await cn.ExecuteAsync(sql, new
        {
            tipoServicio.Id,
            tipoServicio.Nombre,
            tipoServicio.Descripcion,
            tipoServicio.Orden,
            tipoServicio.Activo,
            tipoServicio.UpdatedAt,
            tipoServicio.UpdatedBy,
            tipoServicio.DeletedAt,
            tipoServicio.DeletedBy
        });
    }
}
