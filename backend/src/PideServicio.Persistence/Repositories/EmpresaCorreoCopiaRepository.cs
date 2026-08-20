namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Features.Empresas.DTOs;

public sealed class EmpresaCorreoCopiaRepository : IEmpresaCorreoCopiaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmpresaCorreoCopiaRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<string>> ListarCorreosPorEmpresaAsync(
        Guid empresaId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT correo
            FROM   empresa_correos_copia
            WHERE  empresa_id = @EmpresaId
              AND  activo = true
            ORDER  BY created_at
            """;

        await using var cn = (NpgsqlConnection)await _connectionFactory.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<string>(sql, new { EmpresaId = empresaId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<EmpresaCorreoCopiaDto>> ListarConDetallesPorEmpresaAsync(
        Guid empresaId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT id         AS "Id",
                   correo     AS "Correo",
                   activo     AS "Activo",
                   created_at AS "CreatedAt"
            FROM   empresa_correos_copia
            WHERE  empresa_id = @EmpresaId
              AND  activo = true
            ORDER  BY created_at
            """;

        await using var cn = (NpgsqlConnection)await _connectionFactory.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<CorreoCopiaRow>(sql, new { EmpresaId = empresaId });

        return rows
            .Select(r => new EmpresaCorreoCopiaDto(r.Id, r.Correo, r.Activo, r.CreatedAt))
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Fila intermedia con propiedades asignables. Dapper no puede materializar un
    /// record de constructor posicional cuando los tipos de las columnas no coinciden
    /// exactamente con la firma: 'created_at' (timestamptz) llega como DateTime y el
    /// DTO declara DateTimeOffset, así que no encuentra constructor y falla en runtime.
    /// Con propiedades sí aplica la conversión, y el DTO se construye a mano.
    /// </summary>
    private sealed class CorreoCopiaRow
    {
        public Guid Id { get; init; }
        public string Correo { get; init; } = string.Empty;
        public bool Activo { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }

    public async Task<Guid> AgregarAsync(
        Guid empresaId, string correo, Guid? creadoPor = null, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO empresa_correos_copia (empresa_id, correo, created_by)
            VALUES (@EmpresaId, @Correo, @CreadoPor)
            ON CONFLICT (empresa_id, correo)
            DO UPDATE SET activo = true
            RETURNING id
            """;

        await using var cn = (NpgsqlConnection)await _connectionFactory.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<Guid>(sql, new { EmpresaId = empresaId, Correo = correo.Trim().ToLowerInvariant(), CreadoPor = creadoPor });
    }

    public async Task<bool> EliminarAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE empresa_correos_copia
            SET    activo = false
            WHERE  id = @Id
              AND  activo = true
            """;

        await using var cn = (NpgsqlConnection)await _connectionFactory.CrearConexionAsync(ct);
        var rowsAffected = await cn.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> ExisteAsync(Guid empresaId, string correo, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1
                FROM   empresa_correos_copia
                WHERE  empresa_id = @EmpresaId
                  AND  correo = LOWER(@Correo)
                  AND  activo = true
            )
            """;

        await using var cn = (NpgsqlConnection)await _connectionFactory.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql, new { EmpresaId = empresaId, Correo = correo });
    }
}
