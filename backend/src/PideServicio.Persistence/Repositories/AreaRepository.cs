namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

public sealed class AreaRepository : IAreaRepository
{
    private readonly IDbConnectionFactory _db;

    public AreaRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ─────────────────────────────────────────────────────────────

    private sealed record AreaRow
    {
        public Guid Id { get; init; }
        public Guid SucursalId { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string? Descripcion { get; init; }
        public Guid? ResponsableId { get; init; }
        public bool Activa { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public Guid? CreatedBy { get; init; }
        public Guid? UpdatedBy { get; init; }
        public DateTimeOffset? DeletedAt { get; init; }
        public Guid? DeletedBy { get; init; }
        public int TotalRegistros { get; init; }
    }

    private const string SelectCols = """
        id             AS "Id",
        sucursal_id    AS "SucursalId",
        nombre         AS "Nombre",
        descripcion    AS "Descripcion",
        responsable_id AS "ResponsableId",
        activa         AS "Activa",
        created_at     AS "CreatedAt",
        updated_at     AS "UpdatedAt",
        created_by     AS "CreatedBy",
        updated_by     AS "UpdatedBy",
        deleted_at     AS "DeletedAt",
        deleted_by     AS "DeletedBy"
        """;

    private static Area MapearEntidad(AreaRow r)
    {
        var a = EntityReconstituter.Crear<Area>();
        EntityReconstituter.Set(a, "Id", r.Id);
        EntityReconstituter.Set(a, "SucursalId", r.SucursalId);
        EntityReconstituter.Set(a, "Nombre", r.Nombre);
        EntityReconstituter.Set(a, "Descripcion", r.Descripcion);
        EntityReconstituter.Set(a, "ResponsableId", r.ResponsableId);
        EntityReconstituter.Set(a, "Activa", r.Activa);
        EntityReconstituter.Set(a, "CreatedAt", r.CreatedAt);
        EntityReconstituter.Set(a, "UpdatedAt", r.UpdatedAt);
        EntityReconstituter.Set(a, "CreatedBy", r.CreatedBy);
        EntityReconstituter.Set(a, "UpdatedBy", r.UpdatedBy);
        EntityReconstituter.Set(a, "DeletedAt", r.DeletedAt);
        EntityReconstituter.Set(a, "DeletedBy", r.DeletedBy);
        return a;
    }

    // ── Consultas ──────────────────────────────────────────────────────────────

    public async Task<Area?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectCols}
            FROM areas
            WHERE id = @id
              AND deleted_at IS NULL
            """;
        var row = await cn.QuerySingleOrDefaultAsync<AreaRow>(sql, new { id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<Area>> ListarPorSucursalAsync(
        Guid sucursalId,
        bool? soloActivas = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectCols}
            FROM areas
            WHERE sucursal_id = @sucursalId
              AND deleted_at IS NULL
              AND (@soloActivas::boolean IS NULL OR activa = @soloActivas)
            ORDER BY nombre
            """;
        var rows = await cn.QueryAsync<AreaRow>(sql, new { sucursalId, soloActivas });
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Area>> ListarAsync(
        Guid? sucursalId,
        Guid? empresaId,
        int pagina,
        int tamanoPagina,
        bool? soloActivas = null,
        string? busqueda = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        var offset = (pagina - 1) * tamanoPagina;

        var sql = $"""
            SELECT a.id             AS "Id",
                   a.sucursal_id    AS "SucursalId",
                   a.nombre         AS "Nombre",
                   a.descripcion    AS "Descripcion",
                   a.responsable_id AS "ResponsableId",
                   a.activa         AS "Activa",
                   a.created_at     AS "CreatedAt",
                   a.updated_at     AS "UpdatedAt",
                   a.created_by     AS "CreatedBy",
                   a.updated_by     AS "UpdatedBy",
                   a.deleted_at     AS "DeletedAt",
                   a.deleted_by     AS "DeletedBy",
                   COUNT(*) OVER()  AS "TotalRegistros"
            FROM areas a
            JOIN sucursales s ON s.id = a.sucursal_id AND s.deleted_at IS NULL
            WHERE a.deleted_at IS NULL
              AND (@sucursalId::uuid IS NULL OR a.sucursal_id = @sucursalId)
              AND (@empresaId::uuid IS NULL OR s.empresa_id = @empresaId)
              AND (@soloActivas::boolean IS NULL OR a.activa = @soloActivas)
              AND (@busqueda IS NULL OR a.nombre ILIKE '%' || @busqueda || '%'
                                    OR a.descripcion ILIKE '%' || @busqueda || '%')
            ORDER BY a.nombre
            LIMIT @tamanoPagina OFFSET @offset
            """;

        var rows = (await cn.QueryAsync<AreaRow>(sql, new { sucursalId, empresaId, soloActivas, busqueda, tamanoPagina, offset }))
                   .AsList();

        var total = rows.Count > 0 ? rows[0].TotalRegistros : 0;

        return new PagedResult<Area>
        {
            Items = rows.Select(MapearEntidad).ToList().AsReadOnly(),
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM areas WHERE id = @id AND deleted_at IS NULL
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { id });
    }

    public async Task<bool> ExisteNombreAsync(Guid sucursalId, string nombre, Guid? excludeId = null, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM areas
                WHERE sucursal_id = @sucursalId
                  AND nombre = @nombre
                  AND deleted_at IS NULL
                  AND (@excludeId::uuid IS NULL OR id <> @excludeId)
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { sucursalId, nombre, excludeId });
    }

    public async Task<bool> PerteneceASucursalAsync(Guid areaId, Guid sucursalId, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM areas
                WHERE id = @areaId
                  AND sucursal_id = @sucursalId
                  AND deleted_at IS NULL
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { areaId, sucursalId });
    }

    // ── Escritura ──────────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(Area area, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            INSERT INTO areas
                (id, sucursal_id, nombre, descripcion,
                 responsable_id, activa,
                 created_at, updated_at, created_by, updated_by)
            VALUES
                (@Id, @SucursalId, @Nombre, @Descripcion,
                 @ResponsableId, @Activa,
                 @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy)
            RETURNING id
            """;
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            area.Id,
            area.SucursalId,
            area.Nombre,
            area.Descripcion,
            area.ResponsableId,
            area.Activa,
            area.CreatedAt,
            area.UpdatedAt,
            area.CreatedBy,
            area.UpdatedBy
        });
    }

    public async Task ActualizarAsync(Area area, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            UPDATE areas
            SET nombre         = @Nombre,
                descripcion    = @Descripcion,
                responsable_id = @ResponsableId,
                activa         = @Activa,
                updated_at     = @UpdatedAt,
                updated_by     = @UpdatedBy,
                deleted_at     = @DeletedAt,
                deleted_by     = @DeletedBy
            WHERE id = @Id
            """;
        await cn.ExecuteAsync(sql, new
        {
            area.Id,
            area.Nombre,
            area.Descripcion,
            area.ResponsableId,
            area.Activa,
            area.UpdatedAt,
            area.UpdatedBy,
            area.DeletedAt,
            area.DeletedBy
        });
    }
}
