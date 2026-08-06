namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Features.CorreosGuardados.DTOs;

public interface ICorreoGuardadoRepository
{
    Task<IReadOnlyList<CorreoGuardadoDto>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid usuarioId, string correo, CancellationToken ct = default);
    Task<int> ContarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task<CorreoGuardadoDto> AgregarAsync(Guid usuarioId, string correo, CancellationToken ct = default);
    Task<bool> EliminarAsync(Guid id, Guid usuarioId, CancellationToken ct = default);
}
