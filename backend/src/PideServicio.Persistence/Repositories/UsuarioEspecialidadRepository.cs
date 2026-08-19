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
