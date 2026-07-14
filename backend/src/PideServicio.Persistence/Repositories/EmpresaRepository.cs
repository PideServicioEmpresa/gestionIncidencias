namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Persistence.Helpers;

public sealed class EmpresaRepository : IEmpresaRepository
{
    private readonly IDbConnectionFactory _db;

    public EmpresaRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row — cada campo coincide exactamente con el alias de columna ──

    private sealed record EmpresaRow
    {
        public Guid Id { get; init; }
        public string NombreComercial { get; init; } = string.Empty;
        public string RazonSocial { get; init; } = string.Empty;
        public string IdentificacionFiscal { get; init; } = string.Empty;
        public string? LogoUrl { get; init; }
        public string? ColorPrimario { get; init; }
        public string? ColorSecundario { get; init; }
        public string ZonaHoraria { get; init; } = string.Empty;
        public bool Activa { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public Guid? CreatedBy { get; init; }
        public Guid? UpdatedBy { get; init; }
        public DateTimeOffset? DeletedAt { get; init; }
        public Guid? DeletedBy { get; init; }
        public int TotalRegistros { get; init; }
    }

    private const string SelectCols = """
        id                    AS "Id",
        nombre_comercial      AS "NombreComercial",
        razon_social          AS "RazonSocial",
        identificacion_fiscal AS "IdentificacionFiscal",
        logo_url              AS "LogoUrl",
        color_primario        AS "ColorPrimario",
        color_secundario      AS "ColorSecundario",
        zona_horaria          AS "ZonaHoraria",
        activa                AS "Activa",
        created_at            AS "CreatedAt",
        updated_at            AS "UpdatedAt",
        created_by            AS "CreatedBy",
        updated_by            AS "UpdatedBy",
        deleted_at            AS "DeletedAt",
        deleted_by            AS "DeletedBy"
        """;

    private static Empresa MapearEntidad(EmpresaRow r)
    {
        var e = EntityReconstituter.Crear<Empresa>();
        EntityReconstituter.Set(e, "Id", r.Id);
        EntityReconstituter.Set(e, "NombreComercial", r.NombreComercial);
        EntityReconstituter.Set(e, "RazonSocial", r.RazonSocial);
        EntityReconstituter.Set(e, "IdentificacionFiscal", r.IdentificacionFiscal);
        EntityReconstituter.Set(e, "LogoUrl", r.LogoUrl);
        EntityReconstituter.Set(e, "ColorPrimario", r.ColorPrimario);
        EntityReconstituter.Set(e, "ColorSecundario", r.ColorSecundario);
        EntityReconstituter.Set(e, "ZonaHoraria", r.ZonaHoraria);
        EntityReconstituter.Set(e, "Activa", r.Activa);
        EntityReconstituter.Set(e, "CreatedAt", r.CreatedAt);
        EntityReconstituter.Set(e, "UpdatedAt", r.UpdatedAt);
        EntityReconstituter.Set(e, "CreatedBy", r.CreatedBy);
        EntityReconstituter.Set(e, "UpdatedBy", r.UpdatedBy);
        EntityReconstituter.Set(e, "DeletedAt", r.DeletedAt);
        EntityReconstituter.Set(e, "DeletedBy", r.DeletedBy);
        return e;
    }

    // ── Consultas ──────────────────────────────────────────────────────────────

    public async Task<Empresa?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectCols}
            FROM empresas
            WHERE id = @id
              AND deleted_at IS NULL
            """;
        var row = await cn.QuerySingleOrDefaultAsync<EmpresaRow>(sql, new { id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<Empresa?> ObtenerPorIdentificacionFiscalAsync(string identificacion, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = $"""
            SELECT {SelectCols}
            FROM empresas
            WHERE identificacion_fiscal = @identificacion
              AND deleted_at IS NULL
            """;
        var row = await cn.QuerySingleOrDefaultAsync<EmpresaRow>(sql, new { identificacion });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<PagedResult<Empresa>> ListarAsync(
        int pagina,
        int tamanoPagina,
        bool? soloActivas = null,
        string? busqueda = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        var offset = (pagina - 1) * tamanoPagina;

        const string sql = $"""
            SELECT {SelectCols},
                   COUNT(*) OVER() AS "TotalRegistros"
            FROM empresas
            WHERE deleted_at IS NULL
              AND (@soloActivas::boolean IS NULL OR activa = @soloActivas)
              AND (@busqueda IS NULL OR nombre_comercial ILIKE '%' || @busqueda || '%'
                                    OR razon_social      ILIKE '%' || @busqueda || '%')
            ORDER BY nombre_comercial
            LIMIT @tamanoPagina OFFSET @offset
            """;

        var rows = (await cn.QueryAsync<EmpresaRow>(sql, new { soloActivas, busqueda, tamanoPagina, offset }))
                   .AsList();

        var total = rows.Count > 0 ? rows[0].TotalRegistros : 0;

        return new PagedResult<Empresa>
        {
            Items = rows.Select(MapearEntidad).ToList().AsReadOnly(),
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM empresas WHERE id = @id AND deleted_at IS NULL
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { id });
    }

    public async Task<bool> ExisteIdentificacionFiscalAsync(
        string identificacion,
        Guid? excluirId = null,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM empresas
                WHERE identificacion_fiscal = @identificacion
                  AND deleted_at IS NULL
                  AND (@excluirId::uuid IS NULL OR id <> @excluirId)
            )
            """;
        return await cn.ExecuteScalarAsync<bool>(sql, new { identificacion, excluirId });
    }

    // ── Escritura ──────────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(Empresa empresa, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            INSERT INTO empresas
                (id, nombre_comercial, razon_social, identificacion_fiscal,
                 logo_url, color_primario, color_secundario, zona_horaria, activa,
                 created_at, updated_at, created_by, updated_by)
            VALUES
                (@Id, @NombreComercial, @RazonSocial, @IdentificacionFiscal,
                 @LogoUrl, @ColorPrimario, @ColorSecundario, @ZonaHoraria, @Activa,
                 @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy)
            RETURNING id
            """;
        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            empresa.Id,
            empresa.NombreComercial,
            empresa.RazonSocial,
            empresa.IdentificacionFiscal,
            empresa.LogoUrl,
            empresa.ColorPrimario,
            empresa.ColorSecundario,
            empresa.ZonaHoraria,
            empresa.Activa,
            empresa.CreatedAt,
            empresa.UpdatedAt,
            empresa.CreatedBy,
            empresa.UpdatedBy
        });
    }

    public async Task ActualizarAsync(Empresa empresa, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = """
            UPDATE empresas
            SET nombre_comercial  = @NombreComercial,
                razon_social      = @RazonSocial,
                logo_url          = @LogoUrl,
                color_primario    = @ColorPrimario,
                color_secundario  = @ColorSecundario,
                zona_horaria      = @ZonaHoraria,
                activa            = @Activa,
                updated_at        = @UpdatedAt,
                updated_by        = @UpdatedBy,
                deleted_at        = @DeletedAt,
                deleted_by        = @DeletedBy
            WHERE id = @Id
            """;
        await cn.ExecuteAsync(sql, new
        {
            empresa.Id,
            empresa.NombreComercial,
            empresa.RazonSocial,
            empresa.LogoUrl,
            empresa.ColorPrimario,
            empresa.ColorSecundario,
            empresa.ZonaHoraria,
            empresa.Activa,
            empresa.UpdatedAt,
            empresa.UpdatedBy,
            empresa.DeletedAt,
            empresa.DeletedBy
        });
    }
}
