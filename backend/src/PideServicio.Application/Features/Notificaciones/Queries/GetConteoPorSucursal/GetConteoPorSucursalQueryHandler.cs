namespace PideServicio.Application.Features.Notificaciones.Queries.GetConteoPorSucursal;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Notificaciones.DTOs;

public sealed class GetConteoPorSucursalQueryHandler
    : IQueryHandler<GetConteoPorSucursalQuery, ConteoPorSucursalDto>
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsuarioRepository _usuarioRepository;

    public GetConteoPorSucursalQueryHandler(
        INotificacionRepository notificacionRepository,
        ICurrentUserService currentUserService,
        IUsuarioRepository usuarioRepository)
    {
        _notificacionRepository = notificacionRepository;
        _currentUserService = currentUserService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Result<ConteoPorSucursalDto>> Handle(
        GetConteoPorSucursalQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<ConteoPorSucursalDto>();

        var usuario = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (usuario is null || !usuario.Activo)
            return Result.NoAutorizado<ConteoPorSucursalDto>();

        var items = await _notificacionRepository.ContarNoLeidasPorSucursalAsync(usuario.Id, cancellationToken);
        return Result.Exito(new ConteoPorSucursalDto(items));
    }
}
