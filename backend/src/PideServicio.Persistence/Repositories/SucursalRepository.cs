namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

public sealed class SucursalRepository : ISucursalRepository
{
    private readonly IDbConnectionFactory _db;

    public SucursalRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ─────────────────────────────────────────────────────────────

    private sealed record SucursalRow
    {
        public Guid Id { get; init; }
        public Guid EmpresaId { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string? Descripcion { get; init; }
        public string? Direccion { get; init; }
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
        empresa_id     AS "EmpresaId",
        nombre         AS "Nombre",
        descripcion    AS "Descripcion",
        direccion      AS "Direccion",
        responsable_id AS "ResponsableId",
        activa         AS "Activa",
        created_at     AS "CreatedAt",
        updated_at     AS "UpdatedAt",
        created_by     AS "CreatedBy",
        updated_by     AS "UpdatedBy",
        deleted_at     AS "DeletedAt",
        deleted_by     AS "DeletedBy"
        """;

    private static Sucursal MapearEntidad(SucursalRow r)
    {
        var s = EntityReconstituter.Crear<Sucursal>();
        EntityReconstituter.Set(s, "Id", r.Id);
        EntityReconstituter.Set(s, "EmpresaId", r.EmpresaId);
        EntityReconstituter.Set(s, "Nombre", r.Nombre);
        EntityReconstituter.Set(s, "Descripcion", r.Descripcion);
        EntityReconstituter.Set(s, "Direccion", r.Direccion);
        EntityReconstituter.Set(s, "ResponsableId", r.ResponsableId);
        EntityReconstituter.Set(s, "Activa", r.Activa);
        EntityReconstituter.Set(s, "CreatedAt", r.CreatedAt);
        EntityReconstituter.Set(s, "UpdatedAt", r.UpdatedAt);
        EntityReconstituter.Set(s, "CreatedBy", r.CreatedBy);
        EntityReconstituter.Set(s, "UpdatedBy", r.UpdatedBy);
        EntityReconstituter.Set(s, "DeletedAt", r.DeletedAt);
        EntityReconstituter.Set(s, "DeletedBy", r.DeletedBy);
        return s;
    }

    // ── Consultas ──────────────────────────────────────────────────────────────

    public async Task<Sucursal?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectCols}
            FROM sucursales
            WHERE id = @id
              AND deleted_at IS NULL
            """;
        var row = await cn.QuerySingleOrDefaultAsync<SucursalRow>(sql, new { id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<Sucursal>> ListarPorEmpresaAsync(
        Guid empresaId,
        bool? soloActivas = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectCols}
            FROM sucursales
            WHERE empresa_id = @empresaId
              AND deleted_at IS NULL
              AND (@soloActivas::boolean IS NULL OR activa = @soloActivas)
            ORDER BY nombre
            """;
        var rows = await cn.QueryAsync<SucursalRow>(sql, new { empresaId, soloActivas });
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<PagedResult<Sucursal>> ListarAsync(
        Guid? empresaId,
        int pagina,
        int tamanoPagina,
        bool? soloActivas = null,
        string? busqueda = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        var offset = (pagina - 1) * tamanoPagina;

        const string sql = $"""
            SELECT {SelectCols},
                   COUNT(*) OVER() AS "TotalRegistros"
            FROM sucursales
            WHERE deleted_at IS NULL
              AND (@empresaId::uuid IS NULL OR empresa_id = @empresaId)
              AND (@soloActivas::boolean IS NULL OR activa = @soloActivas)
              AND (@busqueda IS NULL OR nombre ILIKE '%' || @busqueda || '%'
                                    OR descripcion ILIKE '%' || @busqueda || '%')
            ORDER BY nombre
            LIMIT @tamanoPagina OFFSET @offset
            """;

        var rows = (await cn.QueryAsync<SucursalRow>(sql, new { empresaId, soloActivas, busqueda, tamanoPagina, offset }))
                   .AsList();

        var total = rows.Count > 0 ? rows[0].TotalRegistros : 0;

        return new PagedResult<Sucursal>
        {
            Items = rows.Select(MapearEntidad).ToList().AsReadOnly(),
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<int> ContarActivasPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT COUNT(*)::int FROM sucursales
            WHERE empresa_id = @empresaId
              AND activa = true
              AND deleted_at IS NULL
            """;
        return await cn.ExecuteScalarAsync<int>(sql, new { empresaId });
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM sucursales WHERE id = @id AND deleted_at IS NULL
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { id });
    }

    public async Task<bool> ExisteNombreAsync(Guid empresaId, string nombre, Guid? excludeId = null, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM sucursales
                WHERE empresa_id = @empresaId
                  AND nombre = @nombre
                  AND deleted_at IS NULL
                  AND (@excludeId::uuid IS NULL OR id <> @excludeId)
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { empresaId, nombre, excludeId });
    }

    public async Task<bool> PerteneceAEmpresaAsync(Guid sucursalId, Guid empresaId, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM sucursales
                WHERE id = @sucursalId
                  AND empresa_id = @empresaId
                  AND deleted_at IS NULL
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { sucursalId, empresaId });
    }

    // ── Escritura ──────────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(Sucursal sucursal, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            INSERT INTO sucursales
                (id, empresa_id, nombre, descripcion, direccion,
                 responsable_id, activa,
                 created_at, updated_at, created_by, updated_by)
            VALUES
                (@Id, @EmpresaId, @Nombre, @Descripcion, @Direccion,
                 @ResponsableId, @Activa,
                 @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy)
            RETURNING id
            """;
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            sucursal.Id,
            sucursal.EmpresaId,
            sucursal.Nombre,
            sucursal.Descripcion,
            sucursal.Direccion,
            sucursal.ResponsableId,
            sucursal.Activa,
            sucursal.CreatedAt,
            sucursal.UpdatedAt,
            sucursal.CreatedBy,
            sucursal.UpdatedBy
        });
    }

    public async Task ActualizarAsync(Sucursal sucursal, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            UPDATE sucursales
            SET nombre         = @Nombre,
                descripcion    = @Descripcion,
                direccion      = @Direccion,
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
            sucursal.Id,
            sucursal.Nombre,
            sucursal.Descripcion,
            sucursal.Direccion,
            sucursal.ResponsableId,
            sucursal.Activa,
            sucursal.UpdatedAt,
            sucursal.UpdatedBy,
            sucursal.DeletedAt,
            sucursal.DeletedBy
        });
    }
}
