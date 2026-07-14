namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

/// <summary>
/// Implementación de ITicketAsignacionRepository sobre PostgreSQL con Dapper.
/// La tabla ticket_asignaciones es APPEND-ONLY: solo INSERT y SELECT.
/// No existen UPDATE ni DELETE: las asignaciones son inmutables para preservar
/// el historial completo de responsabilidad técnica de cada ticket.
/// </summary>
public sealed class TicketAsignacionRepository : ITicketAsignacionRepository
{
    private readonly IDbConnectionFactory _db;

    public TicketAsignacionRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────

    private sealed record AsignacionRow
    {
        public Guid Id { get; init; }
        public Guid TicketId { get; init; }
        public Guid TecnicoId { get; init; }
        public Guid AsignadorId { get; init; }
        public bool EsReasignacion { get; init; }
        public Guid? TecnicoAnteriorId { get; init; }
        public string? MotivoReasignacion { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public Guid CreatedBy { get; init; }
    }

    private const string SelectCols = """
        id                   AS "Id",
        ticket_id            AS "TicketId",
        tecnico_id           AS "TecnicoId",
        asignador_id         AS "AsignadorId",
        es_reasignacion      AS "EsReasignacion",
        tecnico_anterior_id  AS "TecnicoAnteriorId",
        motivo_reasignacion  AS "MotivoReasignacion",
        created_at           AS "CreatedAt",
        created_by           AS "CreatedBy"
        """;

    // ─── Consultas ────────────────────────────────────────────────────────────

    public async Task<IReadOnlyList<TicketAsignacion>> ListarPorTicketAsync(
        Guid ticketId,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM ticket_asignaciones
            WHERE ticket_id = @ticketId
            ORDER BY created_at ASC
            """;

        var rows = await cn.QueryAsync<AsignacionRow>(sql, new { ticketId });
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    // ─── Escritura ────────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(TicketAsignacion asignacion, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            INSERT INTO ticket_asignaciones (
                id, ticket_id, tecnico_id, asignador_id,
                es_reasignacion, tecnico_anterior_id, motivo_reasignacion,
                created_at, created_by)
            VALUES (
                @Id, @TicketId, @TecnicoId, @AsignadorId,
                @EsReasignacion, @TecnicoAnteriorId, @MotivoReasignacion,
                @CreatedAt, @CreatedBy)
            RETURNING id
            """;

        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            asignacion.Id,
            asignacion.TicketId,
            asignacion.TecnicoId,
            asignacion.AsignadorId,
            asignacion.EsReasignacion,
            asignacion.TecnicoAnteriorId,
            asignacion.MotivoReasignacion,
            asignacion.CreatedAt,
            asignacion.CreatedBy
        });
    }

    // ─── Mapeo ────────────────────────────────────────────────────────────────

    private static TicketAsignacion MapearEntidad(AsignacionRow r)
    {
        var entity = EntityReconstituter.Crear<TicketAsignacion>();

        // BaseEntity
        EntityReconstituter.Set(entity, "Id", r.Id);

        // TicketAsignacion — propiedades propias (no hereda de AuditableEntity)
        EntityReconstituter.Set(entity, "TicketId",           r.TicketId);
        EntityReconstituter.Set(entity, "TecnicoId",          r.TecnicoId);
        EntityReconstituter.Set(entity, "AsignadorId",        r.AsignadorId);
        EntityReconstituter.Set(entity, "EsReasignacion",     r.EsReasignacion);
        EntityReconstituter.Set(entity, "TecnicoAnteriorId",  r.TecnicoAnteriorId);
        EntityReconstituter.Set(entity, "MotivoReasignacion", r.MotivoReasignacion);
        EntityReconstituter.Set(entity, "CreatedAt",          r.CreatedAt);
        EntityReconstituter.Set(entity, "CreatedBy",          r.CreatedBy);

        return entity;
    }
}
