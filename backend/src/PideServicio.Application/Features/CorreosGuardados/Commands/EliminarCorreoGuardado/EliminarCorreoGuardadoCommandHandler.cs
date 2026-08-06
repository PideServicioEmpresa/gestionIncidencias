namespace PideServicio.Application.Features.CorreosGuardados.Commands.EliminarCorreoGuardado;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;

public sealed class EliminarCorreoGuardadoCommandHandler : ICommandHandler<EliminarCorreoGuardadoCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICorreoGuardadoRepository _correoRepo;

    public EliminarCorreoGuardadoCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ICorreoGuardadoRepository correoRepo)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _correoRepo = correoRepo;
    }

    public async Task<Result> Handle(
        EliminarCorreoGuardadoCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        var eliminado = await _correoRepo.EliminarAsync(request.CorreoId, actor.Id, cancellationToken);
        if (!eliminado)
            return Result.NoEncontrado("CorreoGuardado", request.CorreoId);

        return Result.Exito();
    }
}
