namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Entities;

/// <summary>
/// Repositorio de la relación Usuario-Sucursal. Tabla: usuario_sucursales.
/// La columna DB es 'activa' (bool); se mapea a la propiedad 'Activo' de la entidad.
/// El invariante de única sucursal principal por Usuario es responsabilidad
/// del trigger trg_usuario_sucursales_guard_principal en la BD.
/// </summary>
public sealed class UsuarioSucursalRepository : IUsuarioSucursalRepository
{
    private readonly IDbConnectionFactory _db;

    public UsuarioSucursalRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IReadOnlyList<SucursalAsignacionDto>> ListarPorUsuarioAsync(
        Guid usuarioId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT us.sucursal_id  AS "SucursalId",
                   s.nombre        AS "SucursalNombre",
                   us.es_principal AS "EsPrincipal",
                   us.activa       AS "Activo"
            FROM   usuario_sucursales us
            JOIN   sucursales s ON s.id = us.sucursal_id
            WHERE  us.usuario_id = @UsuarioId
            ORDER  BY us.es_principal DESC, s.nombre
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var rows = await cn.QueryAsync<SucursalAsignacionDto>(sql, new { UsuarioId = usuarioId });
        return rows.ToList().AsReadOnly();
    }

    public async Task InsertarAsync(UsuarioSucursal us, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO usuario_sucursales
                (id, usuario_id, sucursal_id, es_principal, activa,
                 created_at, updated_at, created_by)
            VALUES
                (@Id, @UsuarioId, @SucursalId, @EsPrincipal, @Activa,
                 @CreatedAt, @UpdatedAt, @CreatedBy)
            """;

        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        await cn.ExecuteAsync(sql, new
        {
            us.Id,
            us.UsuarioId,
            us.SucursalId,
            us.EsPrincipal,
            Activa    = us.Activo,
            us.CreatedAt,
            us.UpdatedAt,
            us.CreatedBy
        });
    }

    public async Task ReemplazarAsync(
        Guid usuarioId,
        IReadOnlyList<UsuarioSucursal> nuevas,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        await using var tx = await cn.BeginTransactionAsync(ct);

        try
        {
            // 1. Eliminar todas las asignaciones actuales
            await cn.ExecuteAsync(
                "DELETE FROM usuario_sucursales WHERE usuario_id = @UsuarioId",
                new { UsuarioId = usuarioId },
                tx);

            // 2. Insertar las nuevas — no-principales primero, principal al final (trigger-safe)
            const string insertSql = """
                INSERT INTO usuario_sucursales
                    (id, usuario_id, sucursal_id, es_principal, activa,
                     created_at, updated_at, created_by)
                VALUES
                    (@Id, @UsuarioId, @SucursalId, @EsPrincipal, @Activa,
                     @CreatedAt, @UpdatedAt, @CreatedBy)
                """;

            foreach (var us in nuevas.OrderBy(x => x.EsPrincipal))
            {
                await cn.ExecuteAsync(insertSql, new
                {
                    us.Id,
                    us.UsuarioId,
                    us.SucursalId,
                    us.EsPrincipal,
                    Activa    = us.Activo,
                    us.CreatedAt,
                    us.UpdatedAt,
                    us.CreatedBy
                }, tx);
            }

            // 3. Sincronizar usuarios.sucursal_id con la sucursal principal
            var principal = nuevas.Single(x => x.EsPrincipal);
            await cn.ExecuteAsync(
                "UPDATE usuarios SET sucursal_id = @SucursalId, updated_at = NOW() WHERE id = @UsuarioId",
                new { SucursalId = principal.SucursalId, UsuarioId = usuarioId },
                tx);

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
