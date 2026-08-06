namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Features.CorreosGuardados.DTOs;

public sealed class CorreoGuardadoRepository : ICorreoGuardadoRepository
{
    private readonly IDbConnectionFactory _db;
    public CorreoGuardadoRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IReadOnlyList<CorreoGuardadoDto>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT id AS "Id", correo AS "Correo"
            FROM correos_guardados
            WHERE usuario_id = @UsuarioId
            ORDER BY created_at DESC
            """;
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<CorreoGuardadoDto>(sql, new { UsuarioId = usuarioId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<bool> ExisteAsync(Guid usuarioId, string correo, CancellationToken ct = default)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1 FROM correos_guardados
                WHERE usuario_id = @UsuarioId AND correo = LOWER(@Correo)
            )
            """;
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<bool>(sql, new { UsuarioId = usuarioId, Correo = correo });
    }

    public async Task<int> ContarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(*)::int FROM correos_guardados WHERE usuario_id = @UsuarioId";
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.ExecuteScalarAsync<int>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<CorreoGuardadoDto> AgregarAsync(Guid usuarioId, string correo, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO correos_guardados (usuario_id, correo)
            VALUES (@UsuarioId, LOWER(@Correo))
            RETURNING id AS "Id", correo AS "Correo"
            """;
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        return await cn.QuerySingleAsync<CorreoGuardadoDto>(sql, new { UsuarioId = usuarioId, Correo = correo.Trim() });
    }

    public async Task<bool> EliminarAsync(Guid id, Guid usuarioId, CancellationToken ct = default)
    {
        const string sql = """
            DELETE FROM correos_guardados
            WHERE id = @Id AND usuario_id = @UsuarioId
            """;
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.ExecuteAsync(sql, new { Id = id, UsuarioId = usuarioId });
        return rows > 0;
    }
}
