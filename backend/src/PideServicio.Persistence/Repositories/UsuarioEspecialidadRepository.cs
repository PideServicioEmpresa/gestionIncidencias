namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Entities;

/// <summary>
/// Repositorio de la relación Usuario-Especialidad. Tabla: usuario_especialidades.
/// La columna DB es 'activo' (bool); se mapea a la propiedad 'Activo' de la entidad.
/// Relación sin jerarquía: no existe el concepto de especialidad principal.
/// </summary>
public sealed class UsuarioEspecialidadRepository : IUsuarioEspecialidadRepository
{
    private readonly IDbConnectionFactory _db;

    public UsuarioEspecialidadRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IReadOnlyList<EspecialidadAsignadaDto>> ListarPorUsuarioAsync(
        Guid usuarioId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT ue.especialidad_id AS "EspecialidadId",
                   e.nombre           AS "EspecialidadNombre",
                   ue.activo          AS "Activo"
            FROM   usuario_especialidades ue
            JOIN   especialidades e ON e.id = ue.especialidad_id
            WHERE  ue.usuario_id = @UsuarioId
              AND  e.deleted_at IS NULL
            ORDER  BY e.nombre
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<EspecialidadAsignadaDto>(sql, new { UsuarioId = usuarioId });
        return rows.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyList<string>>> ListarNombresPorUsuariosAsync(
        IReadOnlyList<Guid> usuarioIds, CancellationToken ct = default)
    {
        if (usuarioIds.Count == 0)
            return new Dictionary<Guid, IReadOnlyList<string>>();

        // Una sola consulta agregada para TODOS los usuarios de la página: array_agg
        // agrupa los nombres por usuario y evita una consulta por técnico (N+1).
        const string sql = """
            SELECT ue.usuario_id                        AS "UsuarioId",
                   array_agg(e.nombre ORDER BY e.nombre) AS "Nombres"
            FROM   usuario_especialidades ue
            JOIN   especialidades e ON e.id = ue.especialidad_id
            WHERE  ue.usuario_id = ANY(@UsuarioIds)
              AND  ue.activo = true
              AND  e.activo = true
              AND  e.deleted_at IS NULL
            GROUP  BY ue.usuario_id
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var filas = await cn.QueryAsync<(Guid UsuarioId, string[] Nombres)>(
            sql, new { UsuarioIds = usuarioIds.ToArray() });

        return filas.ToDictionary(
            f => f.UsuarioId,
            f => (IReadOnlyList<string>)f.Nombres);
    }

    public async Task ReemplazarAsync(
        Guid usuarioId,
        IReadOnlyList<UsuarioEspecialidad> nuevas,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        await using var tx = await cn.BeginTransactionAsync(ct);

        try
        {
            // 1. Eliminar todas las asignaciones actuales
            await cn.ExecuteAsync(
                "DELETE FROM usuario_especialidades WHERE usuario_id = @UsuarioId",
                new { UsuarioId = usuarioId },
                tx);

            // 2. Insertar las nuevas
            const string insertSql = """
                INSERT INTO usuario_especialidades
                    (id, usuario_id, especialidad_id, activo,
                     created_at, updated_at, created_by)
                VALUES
                    (@Id, @UsuarioId, @EspecialidadId, @Activa,
                     @CreatedAt, @UpdatedAt, @CreatedBy)
                """;

            foreach (var ue in nuevas)
            {
                await cn.ExecuteAsync(insertSql, new
                {
                    ue.Id,
                    ue.UsuarioId,
                    ue.EspecialidadId,
                    Activa = ue.Activo,
                    ue.CreatedAt,
                    ue.UpdatedAt,
                    ue.CreatedBy
                }, tx);
            }

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
