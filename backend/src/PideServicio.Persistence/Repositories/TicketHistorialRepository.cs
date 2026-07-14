namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Persistence.Helpers;

/// <summary>
/// Implementación de ITicketHistorialRepository sobre PostgreSQL con Dapper.
/// La tabla ticket_historial es APPEND-ONLY: solo INSERT y SELECT.
/// No existen UPDATE ni DELETE: las entradas son inmutables por diseño.
/// Las columnas de BD rejection_reason_id / rejection_comment se mapean a las propiedades
/// de dominio MotivoRechazoId / ComentarioRechazo mediante alias SQL con comillas dobles.
/// El campo metadata es JSONB en BD; se lee como text y se almacena como string en dominio.
/// </summary>
public sealed class TicketHistorialRepository : ITicketHistorialRepository
{
    private readonly IDbConnectionFactory _db;

    public TicketHistorialRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────

    private sealed record HistorialRow
    {
        public Guid Id { get; init; }
        public Guid TicketId { get; init; }
        public Guid? ActorId { get; init; }
        public string TipoEvento { get; init; } = string.Empty;
        public string? EstadoAnterior { get; init; }
        public string? EstadoNuevo { get; init; }
        public string? ComentarioTexto { get; init; }
        public Guid? MotivoRechazoId { get; init; }
        public string? ComentarioRechazo { get; init; }
        public string? Metadata { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public Guid? CreatedBy { get; init; }
    }

    // rejection_reason_id y rejection_comment son los nombres reales en BD;
    // se realiasan como MotivoRechazoId y ComentarioRechazo para alinear con el dominio.
    private const string SelectCols = """
        id                    AS "Id",
        ticket_id             AS "TicketId",
        actor_id              AS "ActorId",
        tipo_evento::text     AS "TipoEvento",
        estado_anterior::text AS "EstadoAnterior",
        estado_nuevo::text    AS "EstadoNuevo",
        comentario_texto      AS "ComentarioTexto",
        rejection_reason_id   AS "MotivoRechazoId",
        rejection_comment     AS "ComentarioRechazo",
        metadata::text        AS "Metadata",
        created_at            AS "CreatedAt",
        created_by            AS "CreatedBy"
        """;

    // ─── Consultas ────────────────────────────────────────────────────────────

    public async Task<IReadOnlyList<TicketHistorialEntrada>> ListarPorTicketAsync(
        Guid ticketId,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM ticket_historial
            WHERE ticket_id = @ticketId
            ORDER BY created_at ASC
            """;

        var rows = await cn.QueryAsync<HistorialRow>(sql, new { ticketId });
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    // ─── Escritura ────────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(TicketHistorialEntrada entrada, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        // rejection_reason_id y rejection_comment son los nombres de columna reales.
        // Los campos de ENUM se castean desde texto para no requerir tipos registrados en el DataSource.
        // metadata se castea a jsonb: si el string es null, PostgreSQL lo almacena como NULL.
        const string sql = """
            INSERT INTO ticket_historial (
                id, ticket_id, actor_id, tipo_evento,
                estado_anterior, estado_nuevo, comentario_texto,
                rejection_reason_id, rejection_comment, metadata,
                created_at, created_by)
            VALUES (
                @Id, @TicketId, @ActorId, @TipoEvento::tipo_evento_historial_tipo,
                @EstadoAnterior::ticket_estado_tipo,
                @EstadoNuevo::ticket_estado_tipo,
                @ComentarioTexto,
                @MotivoRechazoId, @ComentarioRechazo, @Metadata::jsonb,
                @CreatedAt, @CreatedBy)
            RETURNING id
            """;

        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            entrada.Id,
            entrada.TicketId,
            entrada.ActorId,
            TipoEvento        = entrada.TipoEvento.ToString(),
            EstadoAnterior    = entrada.EstadoAnterior?.ToString(),
            EstadoNuevo       = entrada.EstadoNuevo?.ToString(),
            entrada.ComentarioTexto,
            MotivoRechazoId   = entrada.MotivoRechazoId,
            ComentarioRechazo = entrada.ComentarioRechazo,
            entrada.Metadata,
            entrada.CreatedAt,
            entrada.CreatedBy
        });
    }

    // ─── Mapeo ────────────────────────────────────────────────────────────────

    private static TicketHistorialEntrada MapearEntidad(HistorialRow r)
    {
        var entity = EntityReconstituter.Crear<TicketHistorialEntrada>();

        // BaseEntity
        EntityReconstituter.Set(entity, "Id", r.Id);

        // TicketHistorialEntrada — propiedades propias (no hereda de AuditableEntity)
        EntityReconstituter.Set(entity, "TicketId",    r.TicketId);
        EntityReconstituter.Set(entity, "ActorId",     r.ActorId);
        EntityReconstituter.Set(entity, "TipoEvento",
            Enum.Parse<TipoEventoHistorialTipo>(r.TipoEvento, ignoreCase: true));
        EntityReconstituter.Set(entity, "EstadoAnterior",
            r.EstadoAnterior is null
                ? (TicketEstadoTipo?)null
                : Enum.Parse<TicketEstadoTipo>(r.EstadoAnterior, ignoreCase: true));
        EntityReconstituter.Set(entity, "EstadoNuevo",
            r.EstadoNuevo is null
                ? (TicketEstadoTipo?)null
                : Enum.Parse<TicketEstadoTipo>(r.EstadoNuevo, ignoreCase: true));
        EntityReconstituter.Set(entity, "ComentarioTexto",   r.ComentarioTexto);
        EntityReconstituter.Set(entity, "MotivoRechazoId",   r.MotivoRechazoId);
        EntityReconstituter.Set(entity, "ComentarioRechazo", r.ComentarioRechazo);
        EntityReconstituter.Set(entity, "Metadata",          r.Metadata);
        EntityReconstituter.Set(entity, "CreatedAt",         r.CreatedAt);
        EntityReconstituter.Set(entity, "CreatedBy",         r.CreatedBy);

        return entity;
    }
}
