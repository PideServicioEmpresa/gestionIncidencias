namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Persistence.Helpers;

/// <summary>
/// Repositorio de parámetros de configuración. Tabla: parametros_sistema.
/// Parámetros no tienen deleted_at ni created_by: se actualizan, no se borran lógicamente.
/// empresa_id nullable: NULL = parámetro global; UUID = override por empresa.
/// El ENUM tipo_dato_parametro_tipo se lee como text (::text) para evitar dependencia
/// del mapeo de ENUMs de Npgsql. El Application layer gestiona la lógica de
/// fallback global → override cuando empresa no tiene su propio valor.
/// Solo ActualizarAsync está expuesto: clave, empresa_id y tipo_dato son inmutables.
/// </summary>
public sealed class ParametroRepository : IParametroRepository
{
    private readonly IDbConnectionFactory _db;

    public ParametroRepository(IDbConnectionFactory db) => _db = db;

    // ── Shadow row ────────────────────────────────────────────────────────────
    // tipo_dato se lee como string (alias TipoDatoStr) y se parsea al enum en MapearEntidad.
    private sealed record ParametroRow
    {
        public Guid           Id          { get; init; }
        public Guid?          EmpresaId   { get; init; }
        public string         Clave       { get; init; } = "";
        public string         Valor       { get; init; } = "";
        public string         TipoDatoStr { get; init; } = "";   // tipo_dato::text
        public string?        Descripcion { get; init; }
        public DateTimeOffset CreatedAt   { get; init; }
        public DateTimeOffset UpdatedAt   { get; init; }
        public Guid?          UpdatedBy   { get; init; }
    }

    // tipo_dato se aliasa como TipoDatoStr porque el nombre del enum en PG
    // no coincide con ninguna propiedad en PascalCase que Dapper pueda mapear directamente.
    private const string SelectCols = """
        id                    AS "Id",
        empresa_id            AS "EmpresaId",
        clave                 AS "Clave",
        valor                 AS "Valor",
        tipo_dato::text       AS "TipoDatoStr",
        descripcion           AS "Descripcion",
        created_at            AS "CreatedAt",
        updated_at            AS "UpdatedAt",
        updated_by            AS "UpdatedBy"
        """;

    private static Parametro MapearEntidad(ParametroRow r)
    {
        var tipoDato = Enum.Parse<TipoDatoParametroTipo>(r.TipoDatoStr, ignoreCase: true);

        var e = EntityReconstituter.Crear<Parametro>();
        EntityReconstituter.Set(e, "Id",          r.Id);
        EntityReconstituter.Set(e, "EmpresaId",   r.EmpresaId);
        EntityReconstituter.Set(e, "Clave",       r.Clave);
        EntityReconstituter.Set(e, "Valor",       r.Valor);
        EntityReconstituter.Set(e, "TipoDato",    tipoDato);
        EntityReconstituter.Set(e, "Descripcion", r.Descripcion);
        EntityReconstituter.Set(e, "CreatedAt",   r.CreatedAt);
        EntityReconstituter.Set(e, "UpdatedAt",   r.UpdatedAt);
        EntityReconstituter.Set(e, "UpdatedBy",   r.UpdatedBy);
        return e;
    }

    // ── Consultas ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Obtiene un parámetro por clave y alcance.
    /// Si empresaId es null: busca el parámetro global (empresa_id IS NULL).
    /// Si empresaId tiene valor: busca el override de esa empresa (no incluye globales).
    /// La lógica de fallback global → empresa es responsabilidad del Application layer.
    /// </summary>
    public async Task<Parametro?> ObtenerPorClaveAsync(
        string clave, Guid? empresaId = null, CancellationToken ct = default)
    {
        string sql;
        object param;

        if (empresaId.HasValue)
        {
            sql = $"""
                SELECT {SelectCols}
                FROM parametros_sistema
                WHERE clave = @Clave
                  AND empresa_id = @EmpresaId
                """;
            param = new { Clave = clave, EmpresaId = empresaId.Value };
        }
        else
        {
            sql = $"""
                SELECT {SelectCols}
                FROM parametros_sistema
                WHERE clave = @Clave
                  AND empresa_id IS NULL
                """;
            param = new { Clave = clave };
        }

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var row = await cn.QuerySingleOrDefaultAsync<ParametroRow>(sql, param);
        return row is null ? null : MapearEntidad(row);
    }

    /// <summary>
    /// Lista todos los parámetros de un alcance.
    /// Si empresaId es null: devuelve los parámetros globales (empresa_id IS NULL).
    /// Si empresaId tiene valor: devuelve los overrides de esa empresa.
    /// </summary>
    public async Task<IReadOnlyList<Parametro>> ListarPorEmpresaAsync(
        Guid? empresaId, CancellationToken ct = default)
    {
        string sql;
        object param;

        if (empresaId.HasValue)
        {
            sql = $"""
                SELECT {SelectCols}
                FROM parametros_sistema
                WHERE empresa_id = @EmpresaId
                ORDER BY clave
                """;
            param = new { EmpresaId = empresaId.Value };
        }
        else
        {
            sql = $"""
                SELECT {SelectCols}
                FROM parametros_sistema
                WHERE empresa_id IS NULL
                ORDER BY clave
                """;
            param = new { };
        }

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<ParametroRow>(sql, param);
        return rows.Select(MapearEntidad).ToList().AsReadOnly();
    }

    /// <summary>
    /// Verifica si existe una clave en el alcance indicado.
    /// </summary>
    public async Task<bool> ExisteClaveAsync(
        string clave, Guid? empresaId = null, CancellationToken ct = default)
    {
        string sql;
        object param;

        if (empresaId.HasValue)
        {
            sql = """
                SELECT EXISTS(
                    SELECT 1 FROM parametros_sistema
                    WHERE clave = @Clave AND empresa_id = @EmpresaId
                )
                """;
            param = new { Clave = clave, EmpresaId = empresaId.Value };
        }
        else
        {
            sql = """
                SELECT EXISTS(
                    SELECT 1 FROM parametros_sistema
                    WHERE clave = @Clave AND empresa_id IS NULL
                )
                """;
            param = new { Clave = clave };
        }

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql, param);
    }

    // ── Escritura ─────────────────────────────────────────────────────────────
    // Solo se actualizan valor, updated_at y updated_by.
    // Clave, empresa_id y tipo_dato son inmutables post-creación (invariante de dominio).
    public async Task ActualizarAsync(Parametro parametro, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE parametros_sistema
            SET valor      = @Valor,
                updated_at = @UpdatedAt,
                updated_by = @UpdatedBy
            WHERE id = @Id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        await cn.ExecuteAsync(sql, new
        {
            parametro.Id,
            parametro.Valor,
            parametro.UpdatedAt,
            parametro.UpdatedBy
        });
    }

    /// <summary>
    /// Actualiza el parámetro si ya existe (match por clave + empresa_id, con soporte de NULL);
    /// si no existe, lo inserta. Evita ON CONFLICT con columnas NULLables usando CTE UPDATE + INSERT.
    /// tipo_dato se pasa como texto y se castea al ENUM de PostgreSQL en la propia sentencia SQL.
    /// </summary>
    public async Task UpsertAsync(Parametro parametro, CancellationToken ct = default)
    {
        // Estrategia: UPDATE primero (devuelve el id si tocó alguna fila).
        // Si no se actualizó nada (fila nueva), INSERT condicional.
        // La condición de igualdad con empresa_id maneja correctamente NULL en ambos lados:
        //   - empresa_id = @EmpresaId  →  falla cuando ambos son NULL (NULL = NULL → NULL)
        //   - empresa_id IS NULL AND @EmpresaId IS NULL  →  cubre el caso global
        const string sql = """
            WITH actualizado AS (
                UPDATE parametros_sistema
                SET valor      = @Valor,
                    updated_at = @UpdatedAt,
                    updated_by = @UpdatedBy
                WHERE clave = @Clave
                  AND (   (empresa_id = @EmpresaId)
                       OR (empresa_id IS NULL AND @EmpresaId IS NULL))
                RETURNING id
            )
            INSERT INTO parametros_sistema
                (id, clave, valor, tipo_dato, descripcion, empresa_id, created_at, updated_at, updated_by)
            SELECT @Id,
                   @Clave,
                   @Valor,
                   @TipoDato::tipo_dato_parametro_tipo,
                   @Descripcion,
                   @EmpresaId,
                   @CreatedAt,
                   @UpdatedAt,
                   @UpdatedBy
            WHERE NOT EXISTS (SELECT 1 FROM actualizado)
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        await cn.ExecuteAsync(sql, new
        {
            parametro.Id,
            parametro.Clave,
            parametro.Valor,
            TipoDato   = parametro.TipoDato.ToString(),  // TEXT → cast en SQL
            parametro.Descripcion,
            parametro.EmpresaId,
            parametro.CreatedAt,
            parametro.UpdatedAt,
            parametro.UpdatedBy
        });
    }
}
