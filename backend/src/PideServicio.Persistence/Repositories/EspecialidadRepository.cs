namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

public sealed class EspecialidadRepository : IEspecialidadRepository
{
    private readonly IDbConnectionFactory _db;

    public EspecialidadRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────
    private sealed record EspecialidadRow
    {
        public Guid            Id          { get; init; }
        public Guid?           EmpresaId   { get; init; }
        public string          Nombre      { get; init; } = "";
        public string?         Descripcion { get; init; }
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
        activo      AS "Activo",
        created_at  AS "CreatedAt",
        updated_at  AS "UpdatedAt",
        created_by  AS "CreatedBy",
        updated_by  AS "UpdatedBy",
        deleted_at  AS "DeletedAt",
        deleted_by  AS "DeletedBy"
        """;

    private static Especialidad MapearEntidad(EspecialidadRow r)
    {
        var e = EntityReconstituter.Crear<Especialidad>();
        EntityReconstituter.Set(e, "Id",          r.Id);
        EntityReconstituter.Set(e, "EmpresaId",   r.EmpresaId);
        EntityReconstituter.Set(e, "Nombre",      r.Nombre);
        EntityReconstituter.Set(e, "Descripcion", r.Descripcion);
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
    public async Task<Especialidad?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        string sql = $"""
            SELECT {SelectCols}
            FROM especialidades
            WHERE id = @Id
              AND deleted_at IS NULL
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var row = await cn.QuerySingleOrDefaultAsync<EspecialidadRow>(sql, new { Id = id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<Especialidad>> ListarActivasAsync(
        Guid? empresaId, CancellationToken ct = default)
    {
        string sql;
        object param;

        if (empresaId.HasValue)
        {
            sql = $"""
                SELECT {SelectCols}
                FROM especialidades
                WHERE (empresa_id = @EmpresaId OR empresa_id IS NULL)
                  AND activo = true
                  AND deleted_at IS NULL
                ORDER BY nombre
                """;
            param = new { EmpresaId = empresaId.Value };
        }
        else
        {
            sql = $"""
                SELECT {SelectCols}
                FROM especialidades
                WHERE empresa_id IS NULL
                  AND activo = true
                  AND deleted_at IS NULL
                ORDER BY nombre
                """;
            param = new { };
        }

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<EspecialidadRow>(sql, param);
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Especialidad>> ListarAsync(
        Guid? empresaId, int pagina, int tamanoPagina, bool? soloActivas = null, string? busqueda = null, CancellationToken ct = default)
    {
        // empresaId null → todas las especialidades
        // empresaId set  → las de la empresa + las globales (empresa_id IS NULL)
        const string whereFiltroEmpresa = """
            (@EmpresaId::uuid IS NULL OR empresa_id = @EmpresaId OR empresa_id IS NULL)
            """;

        string countSql = $"""
            SELECT COUNT(*)
            FROM especialidades
            WHERE deleted_at IS NULL
              AND {whereFiltroEmpresa}
              AND (@SoloActivas::boolean IS NULL OR activo = @SoloActivas)
              AND (@Busqueda IS NULL OR nombre ILIKE '%' || @Busqueda || '%'
                                    OR descripcion ILIKE '%' || @Busqueda || '%')
            """;

        string itemsSql = $"""
            SELECT {SelectCols}
            FROM especialidades
            WHERE deleted_at IS NULL
              AND {whereFiltroEmpresa}
              AND (@SoloActivas::boolean IS NULL OR activo = @SoloActivas)
              AND (@Busqueda IS NULL OR nombre ILIKE '%' || @Busqueda || '%'
                                    OR descripcion ILIKE '%' || @Busqueda || '%')
            ORDER BY nombre
            LIMIT @TamanoPagina OFFSET @Offset
            """;

        var param = new
        {
            EmpresaId    = empresaId,
            SoloActivas  = soloActivas,
            Busqueda     = busqueda,
            TamanoPagina = tamanoPagina,
            Offset       = (pagina - 1) * tamanoPagina
        };

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var total = (int)await cn.ExecuteScalarAsync<long>(countSql, param);
        var rows  = await cn.QueryAsync<EspecialidadRow>(itemsSql, param);

        return new PagedResult<Especialidad>
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
                SELECT 1 FROM especialidades
                WHERE id = @Id AND deleted_at IS NULL
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql, new { Id = id });
    }

    public async Task<bool> ExisteNombreAsync(
        Guid? empresaId, string nombre, Guid? excludeId = null, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM especialidades
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
    public async Task<Guid> CrearAsync(Especialidad especialidad, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO especialidades
                (id, empresa_id, nombre, descripcion, activo,
                 created_at, updated_at, created_by)
            VALUES
                (@Id, @EmpresaId, @Nombre, @Descripcion, @Activo,
                 @CreatedAt, @UpdatedAt, @CreatedBy)
            RETURNING id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            especialidad.Id,
            especialidad.EmpresaId,
            especialidad.Nombre,
            especialidad.Descripcion,
            especialidad.Activo,
            especialidad.CreatedAt,
            especialidad.UpdatedAt,
            especialidad.CreatedBy
        });
    }

    public async Task ActualizarAsync(Especialidad especialidad, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE especialidades
            SET nombre      = @Nombre,
                descripcion = @Descripcion,
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
            especialidad.Id,
            especialidad.Nombre,
            especialidad.Descripcion,
            especialidad.Activo,
            especialidad.UpdatedAt,
            especialidad.UpdatedBy,
            especialidad.DeletedAt,
            especialidad.DeletedBy
        });
    }
}
