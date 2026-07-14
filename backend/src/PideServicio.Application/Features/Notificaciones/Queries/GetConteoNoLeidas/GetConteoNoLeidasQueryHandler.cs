namespace PideServicio.Application.Features.Notificaciones.Queries.GetConteoNoLeidas;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Notificaciones.DTOs;
using PideServicio.Domain.Exceptions;

public sealed class GetConteoNoLeidasQueryHandler
    : IQueryHandler<GetConteoNoLeidasQuery, ConteoNotificacionesDto>
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsuarioRepository _usuarioRepository;

    public GetConteoNoLeidasQueryHandler(
        INotificacionRepository notificacionRepository,
        ICurrentUserService currentUserService,
        IUsuarioRepository usuarioRepository)
    {
        _notificacionRepository = notificacionRepository;
        _currentUserService = currentUserService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Result<ConteoNotificacionesDto>> Handle(
        GetConteoNoLeidasQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<ConteoNotificacionesDto>();

        var usuario = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (usuario is null || !usuario.Activo)
            return Result.NoAutorizado<ConteoNotificacionesDto>();

        try
        {
            var conteo = await _notificacionRepository.ContarNoLeidasAsync(
                usuario.Id,
                cancellationToken);

            return Result.Exito(new ConteoNotificacionesDto(conteo));
        }
        catch (DomainException ex)
        {
            return Result.Fallo<ConteoNotificacionesDto>(ex.Message);
        }
    }
}
