namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

public sealed class MotivoCancelacionRepository : IMotivoCancelacionRepository
{
    private readonly IDbConnectionFactory _db;

    public MotivoCancelacionRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────
    private sealed record MotivoCancelacionRow
    {
        public Guid            Id          { get; init; }
        public Guid?           EmpresaId   { get; init; }
        public string          Texto       { get; init; } = "";
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
        texto       AS "Texto",
        activo      AS "Activo",
        created_at  AS "CreatedAt",
        updated_at  AS "UpdatedAt",
        created_by  AS "CreatedBy",
        updated_by  AS "UpdatedBy",
        deleted_at  AS "DeletedAt",
        deleted_by  AS "DeletedBy"
        """;

    private static MotivoCancelacion MapearEntidad(MotivoCancelacionRow r)
    {
        var e = EntityReconstituter.Crear<MotivoCancelacion>();
        EntityReconstituter.Set(e, "Id",        r.Id);
        EntityReconstituter.Set(e, "EmpresaId", r.EmpresaId);
        EntityReconstituter.Set(e, "Texto",     r.Texto);
        EntityReconstituter.Set(e, "Activo",    r.Activo);
        EntityReconstituter.Set(e, "CreatedAt", r.CreatedAt);
        EntityReconstituter.Set(e, "UpdatedAt", r.UpdatedAt);
        EntityReconstituter.Set(e, "CreatedBy", r.CreatedBy);
        EntityReconstituter.Set(e, "UpdatedBy", r.UpdatedBy);
        EntityReconstituter.Set(e, "DeletedAt", r.DeletedAt);
        EntityReconstituter.Set(e, "DeletedBy", r.DeletedBy);
        return e;
    }

    // ── Consultas ─────────────────────────────────────────────────────────────
    public async Task<MotivoCancelacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        string sql = $"""
            SELECT {SelectCols}
            FROM motivos_cancelacion
            WHERE id = @Id
              AND deleted_at IS NULL
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var row = await cn.QuerySingleOrDefaultAsync<MotivoCancelacionRow>(sql, new { Id = id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<MotivoCancelacion>> ListarActivosAsync(
        Guid? empresaId = null, CancellationToken ct = default)
    {
        string sql;
        object param;

        if (empresaId.HasValue)
        {
            sql = $"""
                SELECT {SelectCols}
                FROM motivos_cancelacion
                WHERE (empresa_id = @EmpresaId OR empresa_id IS NULL)
                  AND activo = true
                  AND deleted_at IS NULL
                ORDER BY texto
                """;
            param = new { EmpresaId = empresaId.Value };
        }
        else
        {
            sql = $"""
                SELECT {SelectCols}
                FROM motivos_cancelacion
                WHERE empresa_id IS NULL
                  AND activo = true
                  AND deleted_at IS NULL
                ORDER BY texto
                """;
            param = new { };
        }

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<MotivoCancelacionRow>(sql, param);
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<PagedResult<MotivoCancelacion>> ListarAsync(
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
            FROM motivos_cancelacion
            WHERE deleted_at IS NULL
              AND {whereFiltroEmpresa}
              AND (@SoloActivos::boolean IS NULL OR activo = @SoloActivos)
              AND (@Busqueda IS NULL OR texto ILIKE '%' || @Busqueda || '%')
            """;

        string itemsSql = $"""
            SELECT {SelectCols}
            FROM motivos_cancelacion
            WHERE deleted_at IS NULL
              AND {whereFiltroEmpresa}
              AND (@SoloActivos::boolean IS NULL OR activo = @SoloActivos)
              AND (@Busqueda IS NULL OR texto ILIKE '%' || @Busqueda || '%')
            ORDER BY texto
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
        var rows  = await cn.QueryAsync<MotivoCancelacionRow>(itemsSql, param);

        return new PagedResult<MotivoCancelacion>
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
                SELECT 1 FROM motivos_cancelacion
                WHERE id = @Id AND deleted_at IS NULL
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql, new { Id = id });
    }

    public async Task<bool> ExisteTextoAsync(
        Guid? empresaId, string texto, Guid? excludeId = null, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM motivos_cancelacion
                WHERE texto = @Texto
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
            new { EmpresaId = empresaId, Texto = texto, ExcludeId = excludeId });
    }

    // ── Escritura ─────────────────────────────────────────────────────────────
    public async Task<Guid> CrearAsync(MotivoCancelacion motivo, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO motivos_cancelacion
                (id, empresa_id, texto, activo,
                 created_at, updated_at, created_by)
            VALUES
                (@Id, @EmpresaId, @Texto, @Activo,
                 @CreatedAt, @UpdatedAt, @CreatedBy)
            RETURNING id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            motivo.Id,
            motivo.EmpresaId,
            motivo.Texto,
            motivo.Activo,
            motivo.CreatedAt,
            motivo.UpdatedAt,
            motivo.CreatedBy
        });
    }

    public async Task ActualizarAsync(MotivoCancelacion motivo, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE motivos_cancelacion
            SET texto      = @Texto,
                activo     = @Activo,
                updated_at = @UpdatedAt,
                updated_by = @UpdatedBy,
                deleted_at = @DeletedAt,
                deleted_by = @DeletedBy
            WHERE id = @Id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        await cn.ExecuteAsync(sql, new
        {
            motivo.Id,
            motivo.Texto,
            motivo.Activo,
            motivo.UpdatedAt,
            motivo.UpdatedBy,
            motivo.DeletedAt,
            motivo.DeletedBy
        });
    }
}
