namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Entities;

public interface IUsuarioEspecialidadRepository
{
    Task<IReadOnlyList<EspecialidadAsignadaDto>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task ReemplazarAsync(Guid usuarioId, IReadOnlyList<UsuarioEspecialidad> nuevas, CancellationToken ct = default);
}
