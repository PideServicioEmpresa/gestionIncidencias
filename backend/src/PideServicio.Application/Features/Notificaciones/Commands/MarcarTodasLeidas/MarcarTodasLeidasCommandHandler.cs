namespace PideServicio.Application.Features.Notificaciones.Commands.MarcarTodasLeidas;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Exceptions;

public sealed class MarcarTodasLeidasCommandHandler
    : ICommandHandler<MarcarTodasLeidasCommand>
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsuarioRepository _usuarioRepository;

    public MarcarTodasLeidasCommandHandler(
        INotificacionRepository notificacionRepository,
        ICurrentUserService currentUserService,
        IUsuarioRepository usuarioRepository)
    {
        _notificacionRepository = notificacionRepository;
        _currentUserService = currentUserService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Result> Handle(
        MarcarTodasLeidasCommand request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var usuario = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (usuario is null || !usuario.Activo)
            return Result.NoAutorizado();

        try
        {
            await _notificacionRepository.MarcarTodasLeidasAsync(usuario.Id, cancellationToken);
            return Result.Exito();
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
