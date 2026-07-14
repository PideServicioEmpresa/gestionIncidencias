namespace PideServicio.Application.Features.Tickets.DTOs;

public sealed record TicketHistorialDto(
    Guid Id,
    Guid TicketId,
    Guid? ActorId,
    string TipoEvento,
    string? EstadoAnterior,
    string? EstadoNuevo,
    string? ComentarioTexto,
    Guid? MotivoRechazoId,
    string? ComentarioRechazo,
    string? Metadata,
    DateTimeOffset CreatedAt,
    string? ActorNombre = null);
