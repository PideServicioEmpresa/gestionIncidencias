namespace PideServicio.Application.Features.Evidencias.DTOs;

public sealed record EvidenciaDto(
    Guid Id,
    Guid TicketId,
    Guid AutorId,
    string Tipo,
    string NombreOriginal,
    string TipoMime,
    long TamanoBytes,
    string UrlAlmacenamiento,
    DateTimeOffset CreatedAt);
