namespace PideServicio.Application.Features.Comentarios.DTOs;

public sealed record ComentarioDto(
    Guid Id,
    Guid TicketId,
    Guid AutorId,
    string Cuerpo,
    bool EsInterno,
    DateTimeOffset? EditadoEn,
    DateTimeOffset CreatedAt,
    string? AutorNombre = null,
    string? AutorRol = null);
