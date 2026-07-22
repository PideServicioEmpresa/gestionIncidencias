namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Entities;

public interface IUsuarioSucursalRepository
{
    Task<IReadOnlyList<SucursalAsignacionDto>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task InsertarAsync(UsuarioSucursal usuarioSucursal, CancellationToken ct = default);
    Task ReemplazarAsync(Guid usuarioId, IReadOnlyList<UsuarioSucursal> nuevas, CancellationToken ct = default);
}
