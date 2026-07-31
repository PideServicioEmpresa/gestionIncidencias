namespace PideServicio.Application.Features.Notificaciones.Queries.GetConteoPorSucursal;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Notificaciones.DTOs;

public sealed record GetConteoPorSucursalQuery : IQuery<ConteoPorSucursalDto>;
