namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

/// <summary>
/// Implementación de ITicketComentarioRepository sobre PostgreSQL con Dapper.
/// Particularidades de la tabla ticket_comentarios:
///   - No tiene updated_at; usa editado_en para registrar la fecha de la última edición.
///   - Tiene borrado lógico (deleted_at / deleted_by).
///   - created_by es NOT NULL (Guid, no Guid?).
///   - ListarPorTicketAsync filtra deleted_at IS NULL siempre;
///     incluirInternos controla la visibilidad de comentarios con es_interno = true.
/// </summary>
public sealed class TicketComentarioRepository : ITicketComentarioRepository
{
    private readonly IDbConnectionFactory _db;

    public TicketComentarioRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────

    private sealed record ComentarioRow
    {
        public Guid Id { get; init; }
        public Guid TicketId { get; init; }
        public Guid AutorId { get; init; }
        public string Cuerpo { get; init; } = string.Empty;
        public bool EsInterno { get; init; }
        public DateTimeOffset? EditadoEn { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public Guid CreatedBy { get; init; }
        public Guid? UpdatedBy { get; init; }
        public DateTimeOffset? DeletedAt { get; init; }
        public Guid? DeletedBy { get; init; }
    }

    private const string SelectCols = """
        id          AS "Id",
        ticket_id   AS "TicketId",
        autor_id    AS "AutorId",
        cuerpo      AS "Cuerpo",
        es_interno  AS "EsInterno",
        editado_en  AS "EditadoEn",
        created_at  AS "CreatedAt",
        created_by  AS "CreatedBy",
        updated_by  AS "UpdatedBy",
        deleted_at  AS "DeletedAt",
        deleted_by  AS "DeletedBy"
        """;

    // ─── Consultas ────────────────────────────────────────────────────────────

    public async Task<TicketComentario?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM ticket_comentarios
            WHERE id = @id AND deleted_at IS NULL
            """;

        var row = await cn.QuerySingleOrDefaultAsync<ComentarioRow>(sql, new { id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<TicketComentario>> ListarPorTicketAsync(
        Guid ticketId,
        bool incluirInternos = false,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        // Usuarios externos (Trabajador / Usuario) no ven comentarios internos.
        // Admin y Técnico pueden solicitarlos con incluirInternos = true.
        var filtroInterno = incluirInternos ? string.Empty : "AND es_interno = false";

        var sql = $"""
            SELECT {SelectCols}
            FROM ticket_comentarios
            WHERE ticket_id = @ticketId
              AND deleted_at IS NULL
              {filtroInterno}
            ORDER BY created_at ASC
            """;

        var rows = await cn.QueryAsync<ComentarioRow>(sql, new { ticketId });
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM ticket_comentarios
                WHERE id = @id AND deleted_at IS NULL)
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { id });
    }

    // ─── Escritura ────────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(TicketComentario comentario, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            INSERT INTO ticket_comentarios (
                id, ticket_id, autor_id, cuerpo, es_interno,
                created_at, created_by)
            VALUES (
                @Id, @TicketId, @AutorId, @Cuerpo, @EsInterno,
                @CreatedAt, @CreatedBy)
            RETURNING id
            """;

        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            comentario.Id,
            comentario.TicketId,
            comentario.AutorId,
            comentario.Cuerpo,
            comentario.EsInterno,
            comentario.CreatedAt,
            comentario.CreatedBy
        });
    }

    public async Task ActualizarAsync(TicketComentario comentario, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        // Cubre las dos operaciones de dominio:
        //   Editar()              → actualiza Cuerpo, EditadoEn, UpdatedBy
        //   EliminarLogicamente() → actualiza DeletedAt, DeletedBy
        const string sql = """
            UPDATE ticket_comentarios SET
                cuerpo     = @Cuerpo,
                editado_en = @EditadoEn,
                updated_by = @UpdatedBy,
                deleted_at = @DeletedAt,
                deleted_by = @DeletedBy
            WHERE id = @Id
            """;

        await cn.ExecuteAsync(sql, new
        {
            comentario.Id,
            comentario.Cuerpo,
            comentario.EditadoEn,
            comentario.UpdatedBy,
            comentario.DeletedAt,
            comentario.DeletedBy
        });
    }

    // ─── Mapeo ────────────────────────────────────────────────────────────────

    private static TicketComentario MapearEntidad(ComentarioRow r)
    {
        var entity = EntityReconstituter.Crear<TicketComentario>();

        // BaseEntity
        EntityReconstituter.Set(entity, "Id", r.Id);

        // TicketComentario — propiedades propias (no hereda de AuditableEntity)
        EntityReconstituter.Set(entity, "TicketId",  r.TicketId);
        EntityReconstituter.Set(entity, "AutorId",   r.AutorId);
        EntityReconstituter.Set(entity, "Cuerpo",    r.Cuerpo);
        EntityReconstituter.Set(entity, "EsInterno", r.EsInterno);
        EntityReconstituter.Set(entity, "EditadoEn", r.EditadoEn);
        EntityReconstituter.Set(entity, "CreatedAt", r.CreatedAt);
        EntityReconstituter.Set(entity, "CreatedBy", r.CreatedBy);
        EntityReconstituter.Set(entity, "UpdatedBy", r.UpdatedBy);
        EntityReconstituter.Set(entity, "DeletedAt", r.DeletedAt);
        EntityReconstituter.Set(entity, "DeletedBy", r.DeletedBy);

        return entity;
    }
}
