namespace PideServicio.Application.Features.Notificaciones.Queries.GetConteoNoLeidas;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Notificaciones.DTOs;

public sealed record GetConteoNoLeidasQuery : IQuery<ConteoNotificacionesDto>;
