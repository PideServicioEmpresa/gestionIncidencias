namespace PideServicio.Application.Features.Notificaciones.Queries.ListNotificaciones;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Notificaciones.DTOs;
using PideServicio.Domain.Exceptions;

public sealed class ListNotificacionesQueryHandler
    : IQueryHandler<ListNotificacionesQuery, PagedResult<NotificacionDto>>
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISucursalActivaService _sucursalActiva;

    public ListNotificacionesQueryHandler(
        INotificacionRepository notificacionRepository,
        ICurrentUserService currentUserService,
        IUsuarioRepository usuarioRepository,
        ISucursalActivaService sucursalActiva)
    {
        _notificacionRepository = notificacionRepository;
        _currentUserService = currentUserService;
        _usuarioRepository = usuarioRepository;
        _sucursalActiva = sucursalActiva;
    }

    public async Task<Result<PagedResult<NotificacionDto>>> Handle(
        ListNotificacionesQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<PagedResult<NotificacionDto>>();

        var usuario = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (usuario is null || !usuario.Activo)
            return Result.NoAutorizado<PagedResult<NotificacionDto>>();

        try
        {
            var pagina = request.Pagina < 1 ? 1 : request.Pagina;
            var tamanoPagina = request.TamanoPagina < 1 ? 20
                : request.TamanoPagina > 100 ? 100
                : request.TamanoPagina;

            var sucursalId = await _sucursalActiva.ObtenerAsync(usuario.Id, usuario.SucursalId, usuario.Rol, cancellationToken);

            var resultado = await _notificacionRepository.ListarPorUsuarioAsync(
                usuario.Id,
                request.SoloNoLeidas,
                pagina,
                tamanoPagina,
                sucursalId,
                cancellationToken);

            var dtos = new PagedResult<NotificacionDto>
            {
                Items = resultado.Items.Adapt<IReadOnlyList<NotificacionDto>>(),
                Pagina = resultado.Pagina,
                TamanoPagina = resultado.TamanoPagina,
                TotalRegistros = resultado.TotalRegistros
            };

            return Result.Exito(dtos);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<PagedResult<NotificacionDto>>(ex.Message);
        }
    }
}
