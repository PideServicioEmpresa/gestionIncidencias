namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Persistence.Helpers;

public sealed class NotificacionRepository : INotificacionRepository
{
    private readonly IDbConnectionFactory _db;

    public NotificacionRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ──────────────────────────────────────────────────────────
    // Mapeo DB → entidad:
    //   destinatario_id → UsuarioId
    //   leida_en        → LeidoEn
    //   estado_entrega  → EstadoEntrega
    //   EmpresaId no existe en la tabla; se deja en Guid.Empty al reconstituir

    private sealed record NotificacionRow
    {
        public Guid Id { get; init; }
        public Guid DestinatarioId { get; init; }
        public Guid? TicketId { get; init; }
        public string Canal { get; init; } = string.Empty;
        public string Titulo { get; init; } = string.Empty;
        public string Cuerpo { get; init; } = string.Empty;
        public string EstadoEntrega { get; init; } = string.Empty;
        public bool Leida { get; init; }
        public DateTimeOffset? LeidaEn { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public Guid? CreatedBy { get; init; }
        public Guid? UpdatedBy { get; init; }
        public int TotalRegistros { get; init; }
    }

    private const string SelectCols = """
        id               AS "Id",
        destinatario_id  AS "DestinatarioId",
        ticket_id        AS "TicketId",
        canal::text      AS "Canal",
        titulo           AS "Titulo",
        cuerpo           AS "Cuerpo",
        estado_entrega::text AS "EstadoEntrega",
        leida            AS "Leida",
        leida_en         AS "LeidaEn",
        created_at       AS "CreatedAt",
        updated_at       AS "UpdatedAt",
        created_by       AS "CreatedBy",
        updated_by       AS "UpdatedBy"
        """;

    private static Notificacion MapearEntidad(NotificacionRow r)
    {
        var n = EntityReconstituter.Crear<Notificacion>();
        EntityReconstituter.Set(n, "Id", r.Id);
        // EmpresaId no existe en la tabla; se hidrata como Guid.Empty.
        // La capa de aplicación filtra por usuarioId, no por empresaId.
        EntityReconstituter.Set(n, "EmpresaId", Guid.Empty);
        EntityReconstituter.Set(n, "UsuarioId", r.DestinatarioId);
        EntityReconstituter.Set(n, "TicketId", r.TicketId);
        EntityReconstituter.Set(n, "Canal", Enum.Parse<CanalNotificacionTipo>(r.Canal, ignoreCase: true));
        EntityReconstituter.Set(n, "Titulo", r.Titulo);
        EntityReconstituter.Set(n, "Cuerpo", r.Cuerpo);
        EntityReconstituter.Set(n, "EstadoEntrega", Enum.Parse<EstadoEntregaTipo>(r.EstadoEntrega, ignoreCase: true));
        // EnviadoEn y ErrorEntrega/Intentos no están en DB; se dejan en sus valores por defecto
        EntityReconstituter.Set(n, "LeidoEn", r.LeidaEn);
        EntityReconstituter.Set(n, "CreatedAt", r.CreatedAt);
        return n;
    }

    // ── Consultas ──────────────────────────────────────────────────────────

    public async Task<Notificacion?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM notificaciones
            WHERE id = @id
            """;
        var row = await cn.QuerySingleOrDefaultAsync<NotificacionRow>(sql, new { id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<PagedResult<Notificacion>> ListarPorUsuarioAsync(
        Guid usuarioId,
        bool? soloNoLeidas,
        int pagina,
        int tamanoPagina,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        var conditions = new List<string> { "destinatario_id = @UsuarioId" };
        var p = new DynamicParameters();
        p.Add("UsuarioId", usuarioId);

        if (soloNoLeidas == true)
            conditions.Add("leida = false");
        else if (soloNoLeidas == false)
            conditions.Add("leida = true");

        var where = string.Join(" AND ", conditions);
        p.Add("Offset", (pagina - 1) * tamanoPagina);
        p.Add("Limit", tamanoPagina);

        var sql = $"""
            SELECT {SelectCols},
                   COUNT(*) OVER() AS "TotalRegistros"
            FROM notificaciones
            WHERE {where}
            ORDER BY created_at DESC
            LIMIT @Limit OFFSET @Offset
            """;

        var rows = (await cn.QueryAsync<NotificacionRow>(sql, p)).AsList();
        var total = rows.Count > 0 ? rows[0].TotalRegistros : 0;

        return new PagedResult<Notificacion>
        {
            Items = rows.Select(MapearEntidad).ToList().AsReadOnly(),
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<int> ContarNoLeidasAsync(Guid usuarioId, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT COUNT(*)::int
            FROM notificaciones
            WHERE destinatario_id = @usuarioId
              AND leida = false
            """;
        return await cn.ExecuteScalarAsync<int>(sql, new { usuarioId });
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM notificaciones WHERE id = @id
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { id });
    }

    // ── Escritura ──────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(Notificacion notificacion, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            INSERT INTO notificaciones
                (id, destinatario_id, ticket_id, tipo_evento, canal, titulo, cuerpo,
                 estado_entrega, leida, leida_en, created_at, updated_at, created_by, updated_by)
            VALUES
                (@Id, @DestinatarioId, @TicketId, @TipoEvento, @Canal::canal_notificacion_tipo, @Titulo, @Cuerpo,
                 @EstadoEntrega::estado_entrega_tipo, false, NULL, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy)
            RETURNING id
            """;
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            notificacion.Id,
            DestinatarioId = notificacion.UsuarioId,
            notificacion.TicketId,
            TipoEvento = notificacion.TipoEvento,
            Canal = notificacion.Canal.ToString(),
            notificacion.Titulo,
            notificacion.Cuerpo,
            EstadoEntrega = notificacion.EstadoEntrega.ToString(),
            notificacion.CreatedAt,
            UpdatedAt = notificacion.CreatedAt,
            CreatedBy = (Guid?)null,
            UpdatedBy = (Guid?)null
        });
    }

    public async Task ActualizarAsync(Notificacion notificacion, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            UPDATE notificaciones
            SET estado_entrega = @EstadoEntrega::estado_entrega_tipo,
                leida          = @Leida,
                leida_en       = @LeidaEn,
                updated_at     = NOW()
            WHERE id = @Id
            """;
        await cn.ExecuteAsync(sql, new
        {
            notificacion.Id,
            EstadoEntrega = notificacion.EstadoEntrega.ToString(),
            Leida = notificacion.LeidoEn.HasValue,
            LeidaEn = notificacion.LeidoEn
        });
    }

    public async Task MarcarTodasLeidasAsync(Guid usuarioId, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            UPDATE notificaciones
            SET leida      = true,
                leida_en   = NOW(),
                updated_at = NOW()
            WHERE destinatario_id = @usuarioId
              AND leida = false
            """;
        await cn.ExecuteAsync(sql, new { usuarioId });
    }
}
