namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly IDbConnectionFactory _db;

    public AuditLogRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ──────────────────────────────────────────────────────────
    // Mapeo DB → entidad (no son 1:1):
    //   actor_id     → UsuarioId
    //   entidad_tipo → Tabla
    //   entidad_id   → RegistroId
    //   valor_anterior (jsonb) → ValoresAnteriores (string)
    //   valor_nuevo   (jsonb) → ValoresNuevos    (string)
    //   EmpresaId no existe en DB; se hidrata como Guid.Empty

    private sealed record AuditLogRow
    {
        public Guid Id { get; init; }
        public Guid? ActorId { get; init; }
        public string ActorNombre { get; init; } = string.Empty;
        public string ActorRol { get; init; } = string.Empty;
        public string Accion { get; init; } = string.Empty;
        public string Modulo { get; init; } = string.Empty;
        public string EntidadTipo { get; init; } = string.Empty;
        public Guid EntidadId { get; init; }
        public string? ValorAnterior { get; init; }
        public string? ValorNuevo { get; init; }
        public string? Ip { get; init; }
        public string? UserAgent { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public int TotalRegistros { get; init; }
    }

    private const string SelectCols = """
        id              AS "Id",
        actor_id        AS "ActorId",
        actor_nombre    AS "ActorNombre",
        actor_rol       AS "ActorRol",
        accion          AS "Accion",
        modulo          AS "Modulo",
        entidad_tipo    AS "EntidadTipo",
        entidad_id      AS "EntidadId",
        valor_anterior::text AS "ValorAnterior",
        valor_nuevo::text    AS "ValorNuevo",
        ip::text        AS "Ip",
        user_agent      AS "UserAgent",
        created_at      AS "CreatedAt"
        """;

    private static AuditLog MapearEntidad(AuditLogRow r)
    {
        var a = EntityReconstituter.Crear<AuditLog>();
        EntityReconstituter.Set(a, "Id", r.Id);
        // EmpresaId no existe en audit_logs; se hidrata como Guid.Empty.
        // El filtro por empresa se aplica vía sucursal_id en la query de listado.
        EntityReconstituter.Set(a, "EmpresaId", Guid.Empty);
        EntityReconstituter.Set(a, "Tabla", r.EntidadTipo);
        EntityReconstituter.Set(a, "RegistroId", r.EntidadId);
        EntityReconstituter.Set(a, "Accion", r.Accion);
        EntityReconstituter.Set(a, "UsuarioId", r.ActorId);
        EntityReconstituter.Set(a, "ValoresAnteriores", r.ValorAnterior);
        EntityReconstituter.Set(a, "ValoresNuevos", r.ValorNuevo);
        EntityReconstituter.Set(a, "IpAddress", r.Ip);
        EntityReconstituter.Set(a, "UserAgent", r.UserAgent);
        EntityReconstituter.Set(a, "CreatedAt", r.CreatedAt);
        return a;
    }

    // ── Consultas ──────────────────────────────────────────────────────────

    public async Task<PagedResult<AuditLog>> ListarAsync(
        Guid empresaId,
        string? tabla,
        Guid? registroId,
        Guid? usuarioId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        int pagina,
        int tamanoPagina,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        // Filtramos empresa a través de las sucursales que pertenecen a ella.
        // Si la consulta resultara costosa en producción, se puede agregar empresa_id
        // directamente a audit_logs (cambio de esquema fuera del MVP).
        var conditions = new List<string>
        {
            "al.sucursal_id IN (SELECT id FROM sucursales WHERE empresa_id = @EmpresaId AND deleted_at IS NULL)"
        };
        var p = new DynamicParameters();
        p.Add("EmpresaId", empresaId);

        if (!string.IsNullOrWhiteSpace(tabla))
        {
            conditions.Add("al.entidad_tipo = @Tabla");
            p.Add("Tabla", tabla);
        }

        if (registroId.HasValue)
        {
            conditions.Add("al.entidad_id = @RegistroId");
            p.Add("RegistroId", registroId.Value);
        }

        if (usuarioId.HasValue)
        {
            conditions.Add("al.actor_id = @UsuarioId");
            p.Add("UsuarioId", usuarioId.Value);
        }

        if (desde.HasValue)
        {
            conditions.Add("al.created_at >= @Desde");
            p.Add("Desde", desde.Value);
        }

        if (hasta.HasValue)
        {
            conditions.Add("al.created_at <= @Hasta");
            p.Add("Hasta", hasta.Value);
        }

        var where = string.Join(" AND ", conditions);
        p.Add("Offset", (pagina - 1) * tamanoPagina);
        p.Add("Limit", tamanoPagina);

        var sql = $"""
            SELECT {SelectCols},
                   COUNT(*) OVER() AS "TotalRegistros"
            FROM audit_logs al
            WHERE {where}
            ORDER BY al.created_at DESC
            LIMIT @Limit OFFSET @Offset
            """;

        var rows = (await cn.QueryAsync<AuditLogRow>(sql, p)).AsList();
        var total = rows.Count > 0 ? rows[0].TotalRegistros : 0;

        return new PagedResult<AuditLog>
        {
            Items = rows.Select(MapearEntidad).ToList().AsReadOnly(),
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total
        };
    }

    // ── Escritura ──────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(AuditLog log, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            INSERT INTO audit_logs
                (id, actor_id, actor_nombre, actor_rol, accion, modulo,
                 entidad_tipo, entidad_id, valor_anterior, valor_nuevo,
                 ip, user_agent, created_at)
            VALUES
                (@Id, @ActorId, @ActorNombre, @ActorRol, @Accion, @Modulo,
                 @EntidadTipo, @EntidadId, @ValorAnterior::jsonb, @ValorNuevo::jsonb,
                 @Ip::inet, @UserAgent, @CreatedAt)
            RETURNING id
            """;
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            log.Id,
            ActorId = log.UsuarioId,
            ActorNombre = "Sistema",
            ActorRol = "SISTEMA",
            log.Accion,
            Modulo = "sistema",
            EntidadTipo = log.Tabla,
            EntidadId = log.RegistroId,
            ValorAnterior = log.ValoresAnteriores,
            ValorNuevo = log.ValoresNuevos,
            Ip = log.IpAddress,
            UserAgent = log.UserAgent,
            log.CreatedAt
        });
    }
}
