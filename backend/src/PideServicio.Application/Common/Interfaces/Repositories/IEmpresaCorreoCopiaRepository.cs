namespace PideServicio.Application.Common.Interfaces.Repositories;

using PideServicio.Application.Features.Empresas.DTOs;

public interface IEmpresaCorreoCopiaRepository
{
    Task<IReadOnlyList<string>> ListarCorreosPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<IReadOnlyList<EmpresaCorreoCopiaDto>> ListarConDetallesPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    /// <summary>
    /// Agrega el correo, o lo reactiva si ya existía retirado. No recibe el autor: la
    /// tabla no tiene columnas de autoría (ver EmpresaCorreoCopiaRepository).
    /// </summary>
    Task<Guid> AgregarAsync(Guid empresaId, string correo, CancellationToken ct = default);
    Task<bool> EliminarAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid empresaId, string correo, CancellationToken ct = default);
}
