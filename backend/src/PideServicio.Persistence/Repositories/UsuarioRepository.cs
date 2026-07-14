namespace PideServicio.Persistence.Repositories;

using System.Text;
using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.ValueObjects;
using PideServicio.Persistence.Helpers;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _db;

    public UsuarioRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ──────────────────────────────────────────────────────────

    private sealed record UsuarioRow
    {
        public Guid Id { get; init; }
        public Guid AuthId { get; init; }
        public Guid EmpresaId { get; init; }
        public Guid SucursalId { get; init; }
        public Guid? AreaId { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string Apellido { get; init; } = string.Empty;
        public string Correo { get; init; } = string.Empty;
        public string NombreUsuario { get; init; } = string.Empty;
        public string? Telefono { get; init; }
        public string Rol { get; init; } = string.Empty;
        public string EstadoLaboral { get; init; } = string.Empty;
        public bool Activo { get; init; }
        public string? FotoUrl { get; init; }
        public DateTimeOffset? UltimoAcceso { get; init; }
        public int Version { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public Guid? CreatedBy { get; init; }
        public Guid? UpdatedBy { get; init; }
        public DateTimeOffset? DeletedAt { get; init; }
        public Guid? DeletedBy { get; init; }
        public int TotalRegistros { get; init; }
    }

    private const string SelectCols = """
        id               AS "Id",
        auth_id          AS "AuthId",
        empresa_id       AS "EmpresaId",
        sucursal_id      AS "SucursalId",
        area_id          AS "AreaId",
        nombre           AS "Nombre",
        apellido         AS "Apellido",
        correo           AS "Correo",
        nombre_usuario   AS "NombreUsuario",
        telefono         AS "Telefono",
        rol::text        AS "Rol",
        estado_laboral::text AS "EstadoLaboral",
        activo           AS "Activo",
        foto_url         AS "FotoUrl",
        ultimo_acceso    AS "UltimoAcceso",
        version          AS "Version",
        created_at       AS "CreatedAt",
        updated_at       AS "UpdatedAt",
        created_by       AS "CreatedBy",
        updated_by       AS "UpdatedBy",
        deleted_at       AS "DeletedAt",
        deleted_by       AS "DeletedBy"
        """;

    private static Usuario MapearEntidad(UsuarioRow r)
    {
        var u = EntityReconstituter.Crear<Usuario>();
        EntityReconstituter.Set(u, "Id", r.Id);
        EntityReconstituter.Set(u, "AuthId", r.AuthId);
        EntityReconstituter.Set(u, "EmpresaId", r.EmpresaId);
        EntityReconstituter.Set(u, "SucursalId", r.SucursalId);
        EntityReconstituter.Set(u, "AreaId", r.AreaId);
        EntityReconstituter.Set(u, "Nombre", r.Nombre);
        EntityReconstituter.Set(u, "Apellido", r.Apellido);
        EntityReconstituter.Set(u, "Correo", Email.Crear(r.Correo));
        EntityReconstituter.Set(u, "NombreUsuario", r.NombreUsuario);
        EntityReconstituter.Set(u, "Telefono", r.Telefono);
        EntityReconstituter.Set(u, "Rol", Enum.Parse<RolTipo>(r.Rol, ignoreCase: true));
        EntityReconstituter.Set(u, "EstadoLaboral", Enum.Parse<EstadoLaboralTipo>(r.EstadoLaboral, ignoreCase: true));
        EntityReconstituter.Set(u, "Activo", r.Activo);
        EntityReconstituter.Set(u, "FotoUrl", r.FotoUrl);
        EntityReconstituter.Set(u, "UltimoAcceso", r.UltimoAcceso);
        EntityReconstituter.Set(u, "Version", r.Version);
        EntityReconstituter.Set(u, "CreatedAt", r.CreatedAt);
        EntityReconstituter.Set(u, "UpdatedAt", r.UpdatedAt);
        EntityReconstituter.Set(u, "CreatedBy", r.CreatedBy);
        EntityReconstituter.Set(u, "UpdatedBy", r.UpdatedBy);
        EntityReconstituter.Set(u, "DeletedAt", r.DeletedAt);
        EntityReconstituter.Set(u, "DeletedBy", r.DeletedBy);
        return u;
    }

    // ── Consultas ──────────────────────────────────────────────────────────

    public async Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM usuarios
            WHERE id = @id
              AND deleted_at IS NULL
            """;
        var row = await cn.QuerySingleOrDefaultAsync<UsuarioRow>(sql, new { id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<Usuario?> ObtenerPorAuthIdAsync(Guid authId, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM usuarios
            WHERE auth_id = @authId
              AND deleted_at IS NULL
            """;
        var row = await cn.QuerySingleOrDefaultAsync<UsuarioRow>(sql, new { authId });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM usuarios
            WHERE correo = @correo
              AND deleted_at IS NULL
            """;
        var row = await cn.QuerySingleOrDefaultAsync<UsuarioRow>(sql, new { correo });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM usuarios
            WHERE nombre_usuario = @nombreUsuario
              AND deleted_at IS NULL
            """;
        var row = await cn.QuerySingleOrDefaultAsync<UsuarioRow>(sql, new { nombreUsuario });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<PagedResult<Usuario>> ListarAsync(
        Guid empresaId,
        Guid? sucursalId,
        RolTipo? rol,
        bool? soloActivos,
        string? busqueda,
        int pagina,
        int tamanoPagina,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        var conditions = new List<string> { "empresa_id = @EmpresaId", "deleted_at IS NULL" };
        var p = new DynamicParameters();
        p.Add("EmpresaId", empresaId);

        if (sucursalId.HasValue)
        {
            conditions.Add("sucursal_id = @SucursalId");
            p.Add("SucursalId", sucursalId.Value);
        }

        if (rol.HasValue)
        {
            conditions.Add("rol = @Rol::rol_tipo");
            p.Add("Rol", rol.Value.ToString());
        }

        if (soloActivos == true)
            conditions.Add("activo = true");
        else if (soloActivos == false)
            conditions.Add("activo = false");

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            conditions.Add("(nombre ILIKE @B OR apellido ILIKE @B OR nombre_usuario ILIKE @B OR correo ILIKE @B)");
            p.Add("B", $"%{busqueda}%");
        }

        var where = string.Join(" AND ", conditions);
        p.Add("Offset", (pagina - 1) * tamanoPagina);
        p.Add("Limit", tamanoPagina);

        var sql = $"""
            SELECT {SelectCols},
                   COUNT(*) OVER() AS "TotalRegistros"
            FROM usuarios
            WHERE {where}
            ORDER BY apellido, nombre
            LIMIT @Limit OFFSET @Offset
            """;

        var rows = (await cn.QueryAsync<UsuarioRow>(sql, p)).AsList();
        var total = rows.Count > 0 ? rows[0].TotalRegistros : 0;

        return new PagedResult<Usuario>
        {
            Items = rows.Select(MapearEntidad).ToList().AsReadOnly(),
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<IReadOnlyList<Usuario>> ListarTecnicosActivosPorEmpresaAsync(
        Guid empresaId,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM usuarios
            WHERE empresa_id = @empresaId
              AND rol = 'TECNICO'::rol_tipo
              AND activo = true
              AND deleted_at IS NULL
            ORDER BY apellido, nombre
            """;
        var rows = await cn.QueryAsync<UsuarioRow>(sql, new { empresaId });
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM usuarios WHERE id = @id AND deleted_at IS NULL
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { id });
    }

    public async Task<bool> ExisteCorreoAsync(
        string correo,
        Guid empresaId,
        Guid? excluirId = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM usuarios
                WHERE correo = @correo
                  AND empresa_id = @empresaId
                  AND deleted_at IS NULL
                  AND (@excluirId::uuid IS NULL OR id <> @excluirId)
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { correo, empresaId, excluirId });
    }

    public async Task<bool> ExisteNombreUsuarioAsync(
        string nombreUsuario,
        Guid? excluirId = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM usuarios
                WHERE nombre_usuario = @nombreUsuario
                  AND deleted_at IS NULL
                  AND (@excluirId::uuid IS NULL OR id <> @excluirId)
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { nombreUsuario, excluirId });
    }

    // ── Escritura ──────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(Usuario usuario, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            INSERT INTO usuarios
                (id, auth_id, empresa_id, sucursal_id, area_id,
                 nombre, apellido, correo, nombre_usuario, telefono,
                 rol, estado_laboral, activo, foto_url, ultimo_acceso,
                 version, created_at, updated_at, created_by, updated_by)
            VALUES
                (@Id, @AuthId, @EmpresaId, @SucursalId, @AreaId,
                 @Nombre, @Apellido, @Correo, @NombreUsuario, @Telefono,
                 @Rol::rol_tipo, @EstadoLaboral::estado_laboral_tipo, @Activo, @FotoUrl, @UltimoAcceso,
                 @Version, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy)
            RETURNING id
            """;
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            usuario.Id,
            usuario.AuthId,
            usuario.EmpresaId,
            usuario.SucursalId,
            usuario.AreaId,
            usuario.Nombre,
            usuario.Apellido,
            Correo = usuario.Correo.Valor,
            usuario.NombreUsuario,
            usuario.Telefono,
            Rol = usuario.Rol.ToString(),
            EstadoLaboral = usuario.EstadoLaboral.ToString(),
            usuario.Activo,
            usuario.FotoUrl,
            usuario.UltimoAcceso,
            usuario.Version,
            usuario.CreatedAt,
            usuario.UpdatedAt,
            usuario.CreatedBy,
            usuario.UpdatedBy
        });
    }

    public async Task ActualizarAsync(Usuario usuario, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            UPDATE usuarios
            SET auth_id        = @AuthId,
                sucursal_id    = @SucursalId,
                area_id        = @AreaId,
                nombre         = @Nombre,
                apellido       = @Apellido,
                correo         = @Correo,
                nombre_usuario = @NombreUsuario,
                telefono       = @Telefono,
                rol            = @Rol::rol_tipo,
                estado_laboral = @EstadoLaboral::estado_laboral_tipo,
                activo         = @Activo,
                foto_url       = @FotoUrl,
                ultimo_acceso  = @UltimoAcceso,
                version        = version + 1,
                updated_at     = @UpdatedAt,
                updated_by     = @UpdatedBy,
                deleted_at     = @DeletedAt,
                deleted_by     = @DeletedBy
            WHERE id = @Id AND version = @Version AND deleted_at IS NULL
            """;
        var afectadas = await cn.ExecuteAsync(sql, new
        {
            usuario.Id,
            usuario.AuthId,
            usuario.SucursalId,
            usuario.AreaId,
            usuario.Nombre,
            usuario.Apellido,
            Correo = usuario.Correo.Valor,
            usuario.NombreUsuario,
            usuario.Telefono,
            Rol = usuario.Rol.ToString(),
            EstadoLaboral = usuario.EstadoLaboral.ToString(),
            usuario.Activo,
            usuario.FotoUrl,
            usuario.UltimoAcceso,
            usuario.Version,
            usuario.UpdatedAt,
            usuario.UpdatedBy,
            usuario.DeletedAt,
            usuario.DeletedBy
        });
        if (afectadas == 0)
            throw new InvalidOperationException($"Conflicto de concurrencia o usuario eliminado. Id={usuario.Id}, Version={usuario.Version}.");
    }
}
