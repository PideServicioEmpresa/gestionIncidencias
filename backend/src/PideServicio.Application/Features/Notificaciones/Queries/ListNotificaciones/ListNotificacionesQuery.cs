namespace PideServicio.Application.Features.Notificaciones.Queries.ListNotificaciones;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Notificaciones.DTOs;

public sealed record ListNotificacionesQuery(
    bool? SoloNoLeidas,
    int Pagina,
    int TamanoPagina) : IQuery<PagedResult<NotificacionDto>>;
