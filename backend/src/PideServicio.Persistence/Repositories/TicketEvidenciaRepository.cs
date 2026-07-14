namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Persistence.Helpers;

/// <summary>
/// Implementación de ITicketEvidenciaRepository sobre PostgreSQL con Dapper.
/// Particularidades de la tabla ticket_evidencias:
///   - No tiene updated_at ni updated_by (las evidencias no se editan).
///   - Tiene borrado lógico (deleted_at / deleted_by) via EliminarLogicamente().
///   - ActualizarAsync solo actualiza los campos de borrado lógico.
///   - El ENUM evidencia_tipo se castea a texto en SELECT y desde texto en INSERT.
///   - ListarPorTicketAsync filtra deleted_at IS NULL; opcionalmente filtra por tipo.
/// </summary>
public sealed class TicketEvidenciaRepository : ITicketEvidenciaRepository
{
    private readonly IDbConnectionFactory _db;

    public TicketEvidenciaRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────

    private sealed record EvidenciaRow
    {
        public Guid Id { get; init; }
        public Guid TicketId { get; init; }
        public Guid AutorId { get; init; }
        public string Tipo { get; init; } = string.Empty;
        public string NombreOriginal { get; init; } = string.Empty;
        public string TipoMime { get; init; } = string.Empty;
        public long TamanoBytes { get; init; }
        public string UrlAlmacenamiento { get; init; } = string.Empty;
        public DateTimeOffset CreatedAt { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTimeOffset? DeletedAt { get; init; }
        public Guid? DeletedBy { get; init; }
    }

    private const string SelectCols = """
        id                   AS "Id",
        ticket_id            AS "TicketId",
        autor_id             AS "AutorId",
        tipo::text           AS "Tipo",
        nombre_original      AS "NombreOriginal",
        tipo_mime            AS "TipoMime",
        tamano_bytes         AS "TamanoBytes",
        url_almacenamiento   AS "UrlAlmacenamiento",
        created_at           AS "CreatedAt",
        created_by           AS "CreatedBy",
        deleted_at           AS "DeletedAt",
        deleted_by           AS "DeletedBy"
        """;

    // ─── Consultas ────────────────────────────────────────────────────────────

    public async Task<TicketEvidencia?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM ticket_evidencias
            WHERE id = @id AND deleted_at IS NULL
            """;

        var row = await cn.QuerySingleOrDefaultAsync<EvidenciaRow>(sql, new { id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<TicketEvidencia>> ListarPorTicketAsync(
        Guid ticketId,
        EvidenciaTipo? tipo = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        var conditions = new List<string>
        {
            "ticket_id = @TicketId",
            "deleted_at IS NULL"
        };
        var parameters = new DynamicParameters();
        parameters.Add("TicketId", ticketId);

        if (tipo.HasValue)
        {
            conditions.Add("tipo = @Tipo::evidencia_tipo");
            parameters.Add("Tipo", tipo.Value.ToString());
        }

        var where = string.Join(" AND ", conditions);
        var sql = $"""
            SELECT {SelectCols}
            FROM ticket_evidencias
            WHERE {where}
            ORDER BY created_at ASC
            """;

        var rows = await cn.QueryAsync<EvidenciaRow>(sql, parameters);
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM ticket_evidencias
                WHERE id = @id AND deleted_at IS NULL)
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { id });
    }

    // ─── Escritura ────────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(TicketEvidencia evidencia, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            INSERT INTO ticket_evidencias (
                id, ticket_id, autor_id, tipo,
                nombre_original, tipo_mime, tamano_bytes, url_almacenamiento,
                created_at, created_by)
            VALUES (
                @Id, @TicketId, @AutorId, @Tipo::evidencia_tipo,
                @NombreOriginal, @TipoMime, @TamanoBytes, @UrlAlmacenamiento,
                @CreatedAt, @CreatedBy)
            RETURNING id
            """;

        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            evidencia.Id,
            evidencia.TicketId,
            evidencia.AutorId,
            Tipo               = evidencia.Tipo.ToString(),
            evidencia.NombreOriginal,
            evidencia.TipoMime,
            evidencia.TamanoBytes,
            evidencia.UrlAlmacenamiento,
            evidencia.CreatedAt,
            evidencia.CreatedBy
        });
    }

    public async Task ActualizarAsync(TicketEvidencia evidencia, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        // TicketEvidencia no es editable. La única mutación válida es el borrado lógico
        // (EliminarLogicamente). El UPDATE solo toca deleted_at y deleted_by.
        const string sql = """
            UPDATE ticket_evidencias SET
                deleted_at = @DeletedAt,
                deleted_by = @DeletedBy
            WHERE id = @Id
            """;

        await cn.ExecuteAsync(sql, new
        {
            evidencia.Id,
            evidencia.DeletedAt,
            evidencia.DeletedBy
        });
    }

    // ─── Mapeo ────────────────────────────────────────────────────────────────

    private static TicketEvidencia MapearEntidad(EvidenciaRow r)
    {
        var entity = EntityReconstituter.Crear<TicketEvidencia>();

        // BaseEntity
        EntityReconstituter.Set(entity, "Id", r.Id);

        // TicketEvidencia — propiedades propias (no hereda de AuditableEntity)
        EntityReconstituter.Set(entity, "TicketId",          r.TicketId);
        EntityReconstituter.Set(entity, "AutorId",           r.AutorId);
        EntityReconstituter.Set(entity, "Tipo",              Enum.Parse<EvidenciaTipo>(r.Tipo, ignoreCase: true));
        EntityReconstituter.Set(entity, "NombreOriginal",    r.NombreOriginal);
        EntityReconstituter.Set(entity, "TipoMime",          r.TipoMime);
        EntityReconstituter.Set(entity, "TamanoBytes",       r.TamanoBytes);
        EntityReconstituter.Set(entity, "UrlAlmacenamiento", r.UrlAlmacenamiento);
        EntityReconstituter.Set(entity, "CreatedAt",         r.CreatedAt);
        EntityReconstituter.Set(entity, "CreatedBy",         r.CreatedBy);
        EntityReconstituter.Set(entity, "DeletedAt",         r.DeletedAt);
        EntityReconstituter.Set(entity, "DeletedBy",         r.DeletedBy);

        return entity;
    }
}
