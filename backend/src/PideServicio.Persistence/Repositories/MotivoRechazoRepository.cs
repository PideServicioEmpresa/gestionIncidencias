namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

public sealed class MotivoRechazoRepository : IMotivoRechazoRepository
{
    private readonly IDbConnectionFactory _db;

    public MotivoRechazoRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────
    private sealed record MotivoRechazoRow
    {
        public Guid            Id          { get; init; }
        public Guid?           EmpresaId   { get; init; }
        public string          Codigo      { get; init; } = "";
        public string          Nombre      { get; init; } = "";
        public string?         Descripcion { get; init; }
        public bool            EsOtro      { get; init; }
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
        codigo      AS "Codigo",
        nombre      AS "Nombre",
        descripcion AS "Descripcion",
        es_otro     AS "EsOtro",
        orden       AS "Orden",
        activo      AS "Activo",
        created_at  AS "CreatedAt",
        updated_at  AS "UpdatedAt",
        created_by  AS "CreatedBy",
        updated_by  AS "UpdatedBy",
        deleted_at  AS "DeletedAt",
        deleted_by  AS "DeletedBy"
        """;

    private static MotivoRechazo MapearEntidad(MotivoRechazoRow r)
    {
        var e = EntityReconstituter.Crear<MotivoRechazo>();
        EntityReconstituter.Set(e, "Id",          r.Id);
        EntityReconstituter.Set(e, "EmpresaId",   r.EmpresaId);
        EntityReconstituter.Set(e, "Codigo",      r.Codigo);
        EntityReconstituter.Set(e, "Nombre",      r.Nombre);
        EntityReconstituter.Set(e, "Descripcion", r.Descripcion);
        EntityReconstituter.Set(e, "EsOtro",      r.EsOtro);
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
    public async Task<MotivoRechazo?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        string sql = $"""
            SELECT {SelectCols}
            FROM motivos_rechazo
            WHERE id = @Id
              AND deleted_at IS NULL
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var row = await cn.QuerySingleOrDefaultAsync<MotivoRechazoRow>(sql, new { Id = id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<MotivoRechazo>> ListarActivosAsync(
        Guid? empresaId = null, CancellationToken ct = default)
    {
        string sql;
        object param;

        if (empresaId.HasValue)
        {
            sql = $"""
                SELECT {SelectCols}
                FROM motivos_rechazo
                WHERE (empresa_id = @EmpresaId OR empresa_id IS NULL)
                  AND activo = true
                  AND deleted_at IS NULL
                ORDER BY orden
                """;
            param = new { EmpresaId = empresaId.Value };
        }
        else
        {
            sql = $"""
                SELECT {SelectCols}
                FROM motivos_rechazo
                WHERE empresa_id IS NULL
                  AND activo = true
                  AND deleted_at IS NULL
                ORDER BY orden
                """;
            param = new { };
        }

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<MotivoRechazoRow>(sql, param);
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<PagedResult<MotivoRechazo>> ListarAsync(
        Guid? empresaId, int pagina, int tamanoPagina, bool? soloActivos = null, string? busqueda = null, bool soloGlobales = false, CancellationToken ct = default)
    {
        const string whereFiltroEmpresa = """
            (
              (@SoloGlobales = true AND empresa_id IS NULL)
              OR (@SoloGlobales = false AND @EmpresaId::uuid IS NULL)
              OR (@SoloGlobales = false AND @EmpresaId::uuid IS NOT NULL AND (empresa_id IS NULL OR empresa_id = @EmpresaId))
            )
            """;

        string countSql = $"""
            SELECT COUNT(*)
            FROM motivos_rechazo
            WHERE deleted_at IS NULL
              AND {whereFiltroEmpresa}
              AND (@SoloActivos::boolean IS NULL OR activo = @SoloActivos)
              AND (@Busqueda IS NULL OR codigo ILIKE '%' || @Busqueda || '%'
                                    OR nombre ILIKE '%' || @Busqueda || '%'
                                    OR descripcion ILIKE '%' || @Busqueda || '%')
            """;

        string itemsSql = $"""
            SELECT {SelectCols}
            FROM motivos_rechazo
            WHERE deleted_at IS NULL
              AND {whereFiltroEmpresa}
              AND (@SoloActivos::boolean IS NULL OR activo = @SoloActivos)
              AND (@Busqueda IS NULL OR codigo ILIKE '%' || @Busqueda || '%'
                                    OR nombre ILIKE '%' || @Busqueda || '%'
                                    OR descripcion ILIKE '%' || @Busqueda || '%')
            ORDER BY orden
            LIMIT @TamanoPagina OFFSET @Offset
            """;

        var param = new
        {
            EmpresaId    = empresaId,
            SoloGlobales = soloGlobales,
            SoloActivos  = soloActivos,
            Busqueda     = busqueda,
            TamanoPagina = tamanoPagina,
            Offset       = (pagina - 1) * tamanoPagina
        };

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var total = (int)await cn.ExecuteScalarAsync<long>(countSql, param);
        var rows  = await cn.QueryAsync<MotivoRechazoRow>(itemsSql, param);

        return new PagedResult<MotivoRechazo>
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
                SELECT 1 FROM motivos_rechazo
                WHERE id = @Id AND deleted_at IS NULL
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql, new { Id = id });
    }

    public async Task<bool> EsOtroAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT COALESCE(es_otro, false)
            FROM motivos_rechazo
            WHERE id = @Id
              AND deleted_at IS NULL
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql, new { Id = id });
    }

    public async Task<bool> ExisteCodigoAsync(
        Guid? empresaId, string codigo, Guid? excludeId = null, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM motivos_rechazo
                WHERE codigo = @Codigo
                  AND deleted_at IS NULL
                  AND (@ExcludeId::uuid IS NULL OR id <> @ExcludeId)
                  AND (
                      (@EmpresaId::uuid IS NULL AND empresa_id IS NULL)
                      OR (@EmpresaId::uuid IS NOT NULL AND empresa_id = @EmpresaId)
                  )
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql,
            new { EmpresaId = empresaId, Codigo = codigo.Trim().ToUpperInvariant(), ExcludeId = excludeId });
    }

    public async Task<bool> ExisteNombreAsync(
        Guid? empresaId, string nombre, Guid? excludeId = null, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM motivos_rechazo
                WHERE nombre = @Nombre
                  AND deleted_at IS NULL
                  AND (@ExcludeId::uuid IS NULL OR id <> @ExcludeId)
                  AND (
                      (@EmpresaId::uuid IS NULL AND empresa_id IS NULL)
                      OR (@EmpresaId::uuid IS NOT NULL AND empresa_id = @EmpresaId)
                  )
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql,
            new { EmpresaId = empresaId, Nombre = nombre, ExcludeId = excludeId });
    }

    // ── Escritura ─────────────────────────────────────────────────────────────
    public async Task<Guid> CrearAsync(MotivoRechazo motivo, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO motivos_rechazo
                (id, empresa_id, codigo, nombre, descripcion, es_otro, orden, activo,
                 created_at, updated_at, created_by)
            VALUES
                (@Id, @EmpresaId, @Codigo, @Nombre, @Descripcion, @EsOtro, @Orden, @Activo,
                 @CreatedAt, @UpdatedAt, @CreatedBy)
            RETURNING id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            motivo.Id,
            motivo.EmpresaId,
            motivo.Codigo,
            motivo.Nombre,
            motivo.Descripcion,
            motivo.EsOtro,
            motivo.Orden,
            motivo.Activo,
            motivo.CreatedAt,
            motivo.UpdatedAt,
            motivo.CreatedBy
        });
    }

    public async Task ActualizarAsync(MotivoRechazo motivo, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE motivos_rechazo
            SET codigo      = @Codigo,
                nombre      = @Nombre,
                descripcion = @Descripcion,
                es_otro     = @EsOtro,
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
            motivo.Id,
            motivo.Codigo,
            motivo.Nombre,
            motivo.Descripcion,
            motivo.EsOtro,
            motivo.Orden,
            motivo.Activo,
            motivo.UpdatedAt,
            motivo.UpdatedBy,
            motivo.DeletedAt,
            motivo.DeletedBy
        });
    }
}
