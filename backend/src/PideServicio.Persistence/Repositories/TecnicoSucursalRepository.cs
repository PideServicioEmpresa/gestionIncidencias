namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

/// <summary>
/// Repositorio de la relación Técnico-Sucursal. Tabla: tecnico_sucursales.
/// La tabla no tiene deleted_at: el soft-disable usa activa = false.
/// Diferencia de nomenclatura: columna DB 'activa' ↔ propiedad de entidad 'Activo'.
/// El invariante de única sucursal principal por Técnico es responsabilidad
/// del trigger trg_fn_tecnico_sucursales_guard_principal en la BD.
/// </summary>
public sealed class TecnicoSucursalRepository : ITecnicoSucursalRepository
{
    private readonly IDbConnectionFactory _db;

    public TecnicoSucursalRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────
    // 'activa' (DB) se mapea a Activo en la entidad vía EntityReconstituter.
    private sealed record TecnicoSucursalRow
    {
        public Guid           Id          { get; init; }
        public Guid           TecnicoId   { get; init; }
        public Guid           SucursalId  { get; init; }
        public bool           EsPrincipal { get; init; }
        public bool           Activo      { get; init; }   // alias de columna 'activa'
        public DateTimeOffset CreatedAt   { get; init; }
        public DateTimeOffset UpdatedAt   { get; init; }
        public Guid?          CreatedBy   { get; init; }
        public Guid?          UpdatedBy   { get; init; }
    }

    // 'activa' se aliasa como "Activo" para que Dapper lo mapee a la propiedad Activo del row.
    private const string SelectCols = """
        id           AS "Id",
        tecnico_id   AS "TecnicoId",
        sucursal_id  AS "SucursalId",
        es_principal AS "EsPrincipal",
        activa       AS "Activo",
        created_at   AS "CreatedAt",
        updated_at   AS "UpdatedAt",
        created_by   AS "CreatedBy",
        updated_by   AS "UpdatedBy"
        """;

    private static TecnicoSucursal MapearEntidad(TecnicoSucursalRow r)
    {
        var e = EntityReconstituter.Crear<TecnicoSucursal>();
        EntityReconstituter.Set(e, "Id",          r.Id);
        EntityReconstituter.Set(e, "TecnicoId",   r.TecnicoId);
        EntityReconstituter.Set(e, "SucursalId",  r.SucursalId);
        EntityReconstituter.Set(e, "EsPrincipal", r.EsPrincipal);
        EntityReconstituter.Set(e, "Activo",      r.Activo);
        EntityReconstituter.Set(e, "CreatedAt",   r.CreatedAt);
        EntityReconstituter.Set(e, "UpdatedAt",   r.UpdatedAt);
        EntityReconstituter.Set(e, "CreatedBy",   r.CreatedBy);
        EntityReconstituter.Set(e, "UpdatedBy",   r.UpdatedBy);
        return e;
    }

    // ── Consultas ─────────────────────────────────────────────────────────────
    public async Task<TecnicoSucursal?> ObtenerAsync(
        Guid tecnicoId, Guid sucursalId, CancellationToken ct = default)
    {
        string sql = $"""
            SELECT {SelectCols}
            FROM tecnico_sucursales
            WHERE tecnico_id  = @TecnicoId
              AND sucursal_id = @SucursalId
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var row = await cn.QuerySingleOrDefaultAsync<TecnicoSucursalRow>(sql,
            new { TecnicoId = tecnicoId, SucursalId = sucursalId });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<IReadOnlyList<TecnicoSucursal>> ListarPorTecnicoAsync(
        Guid tecnicoId, bool? soloActivos = null, CancellationToken ct = default)
    {
        var filtro = soloActivos switch
        {
            true  => "AND activa = true",
            false => "AND activa = false",
            null  => ""
        };

        string sql = $"""
            SELECT {SelectCols}
            FROM tecnico_sucursales
            WHERE tecnico_id = @TecnicoId
            {filtro}
            ORDER BY es_principal DESC, sucursal_id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<TecnicoSucursalRow>(sql, new { TecnicoId = tecnicoId });
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<TecnicoSucursal>> ListarPorSucursalAsync(
        Guid sucursalId, bool? soloActivos = null, CancellationToken ct = default)
    {
        var filtro = soloActivos switch
        {
            true  => "AND activa = true",
            false => "AND activa = false",
            null  => ""
        };

        string sql = $"""
            SELECT {SelectCols}
            FROM tecnico_sucursales
            WHERE sucursal_id = @SucursalId
            {filtro}
            ORDER BY es_principal DESC, tecnico_id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<TecnicoSucursalRow>(sql, new { SucursalId = sucursalId });
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    public async Task<bool> ExisteAsync(Guid tecnicoId, Guid sucursalId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM tecnico_sucursales
                WHERE tecnico_id  = @TecnicoId
                  AND sucursal_id = @SucursalId
            )
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql,
            new { TecnicoId = tecnicoId, SucursalId = sucursalId });
    }

    // ── Escritura ─────────────────────────────────────────────────────────────
    public async Task<Guid> CrearAsync(TecnicoSucursal tecnicoSucursal, CancellationToken ct = default)
    {
        // La columna DB es 'activa'; el parámetro Dapper @Activa toma el valor de Activo.
        const string sql = """
            INSERT INTO tecnico_sucursales
                (id, tecnico_id, sucursal_id, es_principal, activa,
                 created_at, updated_at, created_by)
            VALUES
                (@Id, @TecnicoId, @SucursalId, @EsPrincipal, @Activa,
                 @CreatedAt, @UpdatedAt, @CreatedBy)
            RETURNING id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            tecnicoSucursal.Id,
            tecnicoSucursal.TecnicoId,
            tecnicoSucursal.SucursalId,
            tecnicoSucursal.EsPrincipal,
            Activa     = tecnicoSucursal.Activo,   // entity.Activo → columna DB activa
            tecnicoSucursal.CreatedAt,
            tecnicoSucursal.UpdatedAt,
            tecnicoSucursal.CreatedBy
        });
    }

    public async Task ActualizarAsync(TecnicoSucursal tecnicoSucursal, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE tecnico_sucursales
            SET es_principal = @EsPrincipal,
                activa       = @Activa,
                updated_at   = @UpdatedAt,
                updated_by   = @UpdatedBy
            WHERE id = @Id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        await cn.ExecuteAsync(sql, new
        {
            tecnicoSucursal.Id,
            tecnicoSucursal.EsPrincipal,
            Activa     = tecnicoSucursal.Activo,   // entity.Activo → columna DB activa
            tecnicoSucursal.UpdatedAt,
            tecnicoSucursal.UpdatedBy
        });
    }
}
