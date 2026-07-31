namespace PideServicio.Application.Features.Notificaciones.DTOs;

public sealed record ConteoPorSucursalItemDto(Guid SucursalId, string SucursalNombre, int Cantidad);

public sealed record ConteoPorSucursalDto(IReadOnlyList<ConteoPorSucursalItemDto> Items);
