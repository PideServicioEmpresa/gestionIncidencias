namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Persistence.Helpers;

public sealed class PermisoRepository : IPermisoRepository
{
    private readonly IDbConnectionFactory _db;

    public PermisoRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ─────────────────────────────────────────────────────────────

    private sealed record PermisoRow
    {
        public Guid Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string Nombre { get; init; } = string.Empty;
        public string? Descripcion { get; init; }
        public string Modulo { get; init; } = string.Empty;
        public string Recurso { get; init; } = string.Empty;
        public string Accion { get; init; } = string.Empty;
        public bool Activo { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset UpdatedAt { get; init; }
        public Guid? CreatedBy { get; init; }
        public Guid? UpdatedBy { get; init; }
    }

    private const string SelectCols = """
        id          AS "Id",
        codigo      AS "Codigo",
        nombre      AS "Nombre",
        descripcion AS "Descripcion",
        modulo      AS "Modulo",
        recurso     AS "Recurso",
        accion      AS "Accion",
        activo      AS "Activo",
        created_at  AS "CreatedAt",
        updated_at  AS "UpdatedAt",
        created_by  AS "CreatedBy",
        updated_by  AS "UpdatedBy"
        """;

    private static Permiso MapearEntidad(PermisoRow r)
    {
        var p = EntityReconstituter.Crear<Permiso>();
        EntityReconstituter.Set(p, "Id", r.Id);
        EntityReconstituter.Set(p, "Codigo", r.Codigo);
        EntityReconstituter.Set(p, "Nombre", r.Nombre);
        EntityReconstituter.Set(p, "Descripcion", r.Descripcion);
        EntityReconstituter.Set(p, "Modulo", r.Modulo);
        EntityReconstituter.Set(p, "Recurso", r.Recurso);
        EntityReconstituter.Set(p, "Accion", r.Accion);
        EntityReconstituter.Set(p, "Activo", r.Activo);
        EntityReconstituter.Set(p, "CreatedAt", r.CreatedAt);
        EntityReconstituter.Set(p, "UpdatedAt", r.UpdatedAt);
        EntityReconstituter.Set(p, "CreatedBy", r.CreatedBy);
        EntityReconstituter.Set(p, "UpdatedBy", r.UpdatedBy);
        return p;
    }

    // ── Consultas ──────────────────────────────────────────────────────────────

    public async Task<Permiso?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectCols}
            FROM permisos
            WHERE id = @id
              AND activo = true
            """;
        var row = await cn.QuerySingleOrDefaultAsync<PermisoRow>(sql, new { id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<Permiso>> ListarTodosAsync(CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectCols}
            FROM permisos
            WHERE activo = true
            ORDER BY modulo, recurso, accion
            """;
        var rows = await cn.QueryAsync<PermisoRow>(sql);
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    /// <summary>
    /// Verifica si el usuario tiene el permiso indicado considerando su rol y empresa.
    /// Evalúa a través de la tabla role_permissions; no hay permisos por usuario individual.
    /// </summary>
    public async Task<bool> TienePermisoAsync(
        Guid usuarioId,
        string codigoPermiso,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM usuarios u
                JOIN role_permissions rp
                  ON rp.rol_codigo = u.rol_codigo
                 AND rp.activo     = true
                JOIN permisos p
                  ON p.id     = rp.permiso_id
                 AND p.activo = true
                 AND p.codigo = @codigoPermiso
                WHERE u.id          = @usuarioId
                  AND u.deleted_at  IS NULL
                  AND (rp.empresa_id IS NULL OR rp.empresa_id = u.empresa_id)
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { usuarioId, codigoPermiso });
    }

    /// <summary>
    /// Verifica si un rol tiene el permiso indicado, opcionalmente filtrando por empresa
    /// (permisos globales con empresa_id NULL siempre se incluyen).
    /// </summary>
    public async Task<bool> TienePermisoRolAsync(
        RolTipo rol,
        string codigoPermiso,
        Guid? empresaId = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM role_permissions rp
                JOIN permisos p
                  ON p.id     = rp.permiso_id
                 AND p.activo = true
                 AND p.codigo = @codigoPermiso
                WHERE rp.rol_codigo = @rolCodigo
                  AND rp.activo     = true
                  AND (@empresaId::uuid IS NULL OR rp.empresa_id IS NULL OR rp.empresa_id = @empresaId)
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new
        {
            rolCodigo = rol.ToString(),
            codigoPermiso,
            empresaId
        });
    }
}
