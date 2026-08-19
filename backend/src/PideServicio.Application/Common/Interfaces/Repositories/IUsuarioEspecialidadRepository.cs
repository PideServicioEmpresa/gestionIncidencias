namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Entities;

public interface IUsuarioEspecialidadRepository
{
    Task<IReadOnlyList<EspecialidadAsignadaDto>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    /// <summary>
    /// Nombres de especialidades activas de varios usuarios, resueltos en UNA sola
    /// consulta agregada. Pensado para listados: evita el N+1 de consultar por usuario.
    /// Los usuarios sin especialidades simplemente no aparecen en el diccionario.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, IReadOnlyList<string>>> ListarNombresPorUsuariosAsync(
        IReadOnlyList<Guid> usuarioIds, CancellationToken ct = default);

    Task ReemplazarAsync(Guid usuarioId, IReadOnlyList<UsuarioEspecialidad> nuevas, CancellationToken ct = default);
}
