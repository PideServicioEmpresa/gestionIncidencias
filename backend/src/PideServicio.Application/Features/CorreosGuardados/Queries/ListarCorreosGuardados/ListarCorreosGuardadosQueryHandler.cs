namespace PideServicio.Application.Features.CorreosGuardados.Queries.ListarCorreosGuardados;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.CorreosGuardados.DTOs;

public sealed class ListarCorreosGuardadosQueryHandler
    : IQueryHandler<ListarCorreosGuardadosQuery, IReadOnlyList<CorreoGuardadoDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICorreoGuardadoRepository _correoRepo;

    public ListarCorreosGuardadosQueryHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ICorreoGuardadoRepository correoRepo)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _correoRepo = correoRepo;
    }

    public async Task<Result<IReadOnlyList<CorreoGuardadoDto>>> Handle(
        ListarCorreosGuardadosQuery request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<IReadOnlyList<CorreoGuardadoDto>>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo)
            return Result.NoAutorizado<IReadOnlyList<CorreoGuardadoDto>>();

        var lista = await _correoRepo.ListarPorUsuarioAsync(actor.Id, cancellationToken);
        return Result.Exito(lista);
    }
}
