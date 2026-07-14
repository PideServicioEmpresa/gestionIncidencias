namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

public sealed class CategoriaRepository : ICategoriaRepository
{
    private readonly IDbConnectionFactory _db;

    public CategoriaRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────
    private sealed record CategoriaRow
    {
        public Guid            Id          { get; init; }
        public Guid?           EmpresaId   { get; init; }
        public string          Nombre      { get; init; } = "";
        public string?         Descripcion { get; init; }
        public bool            Activa      { get; init; }
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
        activa      AS "Activa",
        created_at  AS "CreatedAt",
        updated_at  AS "UpdatedAt",
        created_by  AS "CreatedBy",
        updated_by  AS "UpdatedBy",
        deleted_at  AS "DeletedAt",
        deleted_by  AS "DeletedBy"
        """;

    private static Categoria MapearEntidad(CategoriaRow r)
    {
        var e = EntityReconstituter.Crear<Categoria>();
        EntityReconstituter.Set(e, "Id",          r.Id);
        EntityReconstituter.Set(e, "EmpresaId",   r.EmpresaId);
        EntityReconstituter.Set(e, "Nombre",      r.Nombre);
        EntityReconstituter.Set(e, "Descripcion", r.Descripcion);
        EntityReconstituter.Set(e, "Activa",      r.Activa);
        EntityReconstituter.Set(e, "CreatedAt",   r.CreatedAt);
        EntityReconstituter.Set(e, "UpdatedAt",   r.UpdatedAt);
        EntityReconstituter.Set(e, "CreatedBy",   r.CreatedBy);
        EntityReconstituter.Set(e, "UpdatedBy",   r.UpdatedBy);
        EntityReconstituter.Set(e, "DeletedAt",   r.DeletedAt);
        EntityReconstituter.Set(e, "DeletedBy",   r.DeletedBy);
        return e;
    }

    // ── Consultas ─────────────────────────────────────────────────────────────
    public async Task<Categoria?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        string sql = $"""
            SELECT {SelectCols}
            FROM categorias
            WHERE id = @Id
              AND deleted_at IS NULL
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var row = await cn.QuerySingleOrDefaultAsync<CategoriaRow>(sql, new { Id = id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<Categoria>> ListarActivasAsync(
        Guid? empresaId = null, CancellationToken ct = default)
    {
        string sql;
        object param;

        if (empresaId.HasValue)
        {
            sql = $"""
                SELECT {SelectCols}
                FROM categorias
                WHERE (empresa_id = @EmpresaId OR empresa_id IS NULL)
                  AND activa = true
                  AND deleted_at IS NULL
                ORDER BY nombre
                """;
            param = new { EmpresaId = empresaId.Value };
        }
        else
        {
            sql = $"""
                SELECT {SelectCols}
                FROM categorias
                WHERE empresa_id IS NULL
                  AND activa = true
                  AND deleted_at IS NULL
                ORDER BY nombre
                """;
            param = new { };
        }

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<CategoriaRow>(sql, param);
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Categoria>> ListarAsync(
        Guid? empresaId, int pagina, int tamanoPagina, bool? soloActivas = null, string? busqueda = null, bool soloGlobales = false, CancellationToken ct = default)
    {
        // soloGlobales=true → solo empresa_id IS NULL
        // empresaId set     → empresa_id IS NULL OR empresa_id = X
        // empresaId null AND NOT soloGlobales → todo
        const string whereFiltroEmpresa = """
            (
              (@SoloGlobales = true AND empresa_id IS NULL)
              OR (@SoloGlobales = false AND @EmpresaId::uuid IS NULL)
              OR (@SoloGlobales = false AND @EmpresaId::uuid IS NOT NULL AND (empresa_id IS NULL OR empresa_id = @EmpresaId))
            )
            """;

        string countSql = $"""
            SELECT COUNT(*)
            FROM categorias
            WHERE deleted_at IS NULL
              AND {whereFiltroEmpresa}
              AND (@SoloActivas::boolean IS NULL OR activa = @SoloActivas)
              AND (@Busqueda IS NULL OR nombre ILIKE '%' || @Busqueda || '%'
                                    OR descripcion ILIKE '%' || @Busqueda || '%')
            """;

        string itemsSql = $"""
            SELECT {SelectCols}
            FROM categorias
            WHERE deleted_at IS NULL
              AND {whereFiltroEmpresa}
              AND (@SoloActivas::boolean IS NULL OR activa = @SoloActivas)
              AND (@Busqueda IS NULL OR nombre ILIKE '%' || @Busqueda || '%'
                                    OR descripcion ILIKE '%' || @Busqueda || '%')
            ORDER BY nombre
            LIMIT @TamanoPagina OFFSET @Offset
            """;

        var param = new
        {
            EmpresaId    = empresaId,
            SoloGlobales = soloGlobales,
            SoloActivas  = soloActivas,
            Busqueda     = busqueda,
            TamanoPagina = tamanoPagina,
            Offset       = (pagina - 1) * tamanoPagina
        };

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var total = (int)await cn.ExecuteScalarAsync<long>(countSql, param);
        var rows  = await cn.QueryAsync<CategoriaRow>(itemsSql, param);

        return new PagedResult<Categoria>
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
                SELECT 1 FROM categorias
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
                SELECT 1 FROM categorias
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
    public async Task<Guid> CrearAsync(Categoria categoria, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO categorias
                (id, empresa_id, nombre, descripcion, activa,
                 created_at, updated_at, created_by)
            VALUES
                (@Id, @EmpresaId, @Nombre, @Descripcion, @Activa,
                 @CreatedAt, @UpdatedAt, @CreatedBy)
            RETURNING id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            categoria.Id,
            categoria.EmpresaId,
            categoria.Nombre,
            categoria.Descripcion,
            categoria.Activa,
            categoria.CreatedAt,
            categoria.UpdatedAt,
            categoria.CreatedBy
        });
    }

    public async Task ActualizarAsync(Categoria categoria, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE categorias
            SET nombre      = @Nombre,
                descripcion = @Descripcion,
                activa      = @Activa,
                updated_at  = @UpdatedAt,
                updated_by  = @UpdatedBy,
                deleted_at  = @DeletedAt,
                deleted_by  = @DeletedBy
            WHERE id = @Id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        await cn.ExecuteAsync(sql, new
        {
            categoria.Id,
            categoria.Nombre,
            categoria.Descripcion,
            categoria.Activa,
            categoria.UpdatedAt,
            categoria.UpdatedBy,
            categoria.DeletedAt,
            categoria.DeletedBy
        });
    }
}
