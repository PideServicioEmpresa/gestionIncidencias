namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Persistence.Helpers;

public sealed class RolRepository : IRolRepository
{
    private readonly IDbConnectionFactory _db;

    public RolRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow rows ────────────────────────────────────────────────────────────

    private sealed record RolRow
    {
        public Guid Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string Nombre { get; init; } = string.Empty;
        public string? Descripcion { get; init; }
        public bool Activo { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset UpdatedAt { get; init; }
        public Guid? CreatedBy { get; init; }
        public Guid? UpdatedBy { get; init; }
    }

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

    private const string SelectRolCols = """
        id          AS "Id",
        codigo      AS "Codigo",
        nombre      AS "Nombre",
        descripcion AS "Descripcion",
        activo      AS "Activo",
        created_at  AS "CreatedAt",
        updated_at  AS "UpdatedAt",
        created_by  AS "CreatedBy",
        updated_by  AS "UpdatedBy"
        """;

    private const string SelectPermisoCols = """
        p.id          AS "Id",
        p.codigo      AS "Codigo",
        p.nombre      AS "Nombre",
        p.descripcion AS "Descripcion",
        p.modulo      AS "Modulo",
        p.recurso     AS "Recurso",
        p.accion      AS "Accion",
        p.activo      AS "Activo",
        p.created_at  AS "CreatedAt",
        p.updated_at  AS "UpdatedAt",
        p.created_by  AS "CreatedBy",
        p.updated_by  AS "UpdatedBy"
        """;

    private static Rol MapearRol(RolRow r)
    {
        var rol = EntityReconstituter.Crear<Rol>();
        EntityReconstituter.Set(rol, "Id", r.Id);
        EntityReconstituter.Set(rol, "Codigo", Enum.Parse<RolTipo>(r.Codigo, ignoreCase: true));
        EntityReconstituter.Set(rol, "Nombre", r.Nombre);
        EntityReconstituter.Set(rol, "Descripcion", r.Descripcion);
        EntityReconstituter.Set(rol, "Activo", r.Activo);
        EntityReconstituter.Set(rol, "CreatedAt", r.CreatedAt);
        EntityReconstituter.Set(rol, "UpdatedAt", r.UpdatedAt);
        EntityReconstituter.Set(rol, "CreatedBy", r.CreatedBy);
        EntityReconstituter.Set(rol, "UpdatedBy", r.UpdatedBy);
        return rol;
    }

    private static Permiso MapearPermiso(PermisoRow r)
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

    public async Task<Rol?> ObtenerPorCodigoAsync(RolTipo codigo, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectRolCols}
            FROM roles
            WHERE codigo = @codigo
              AND activo = true
            """;
        var row = await cn.QuerySingleOrDefaultAsync<RolRow>(sql, new { codigo = codigo.ToString() });
        return row is null ? null : MapearRol(row);
    }

    public async Task<IReadOnlyList<Rol>> ListarTodosAsync(CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectRolCols}
            FROM roles
            WHERE activo = true
            ORDER BY
                CASE codigo
                    WHEN 'SUPERADMIN' THEN 1
                    WHEN 'ADMIN'      THEN 2
                    WHEN 'SUPERVISOR' THEN 3
                    WHEN 'TECNICO'    THEN 4
                    WHEN 'TRABAJADOR' THEN 5
                    WHEN 'USUARIO'    THEN 6
                    ELSE 99
                END
            """;
        var rows = await cn.QueryAsync<RolRow>(sql);
        return rows.Select(MapearRol).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Permiso>> ListarPermisosDeRolAsync(
        RolTipo rolCodigo,
        Guid? empresaId = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT DISTINCT {SelectPermisoCols}
            FROM role_permissions rp
            JOIN permisos p ON p.id = rp.permiso_id
            WHERE rp.rol_codigo = @rolCodigo
              AND rp.activo     = true
              AND p.activo      = true
              AND (@empresaId::uuid IS NULL OR rp.empresa_id IS NULL OR rp.empresa_id = @empresaId)
            ORDER BY p.modulo, p.recurso, p.accion
            """;
        var rows = await cn.QueryAsync<PermisoRow>(sql, new { rolCodigo = rolCodigo.ToString(), empresaId });
        return rows.Select(MapearPermiso).ToList().AsReadOnly();
    }
}
