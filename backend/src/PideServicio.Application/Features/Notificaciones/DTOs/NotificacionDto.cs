namespace PideServicio.Application.Features.Notificaciones.DTOs;

public sealed record NotificacionDto(
    Guid Id,
    Guid UsuarioId,
    Guid? TicketId,
    string Canal,
    string Titulo,
    string Cuerpo,
    string EstadoEntrega,
    DateTimeOffset? LeidoEn,
    DateTimeOffset CreatedAt,
    bool EsLeida);
