namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Archivo de evidencia adjunto a un ticket. Tiene borrado lógico pero no actualización.
/// url_almacenamiento = ruta en Storage, no URL pública.
/// </summary>
public sealed class TicketEvidencia : BaseEntity
{
    public Guid TicketId { get; private set; }
    public Guid AutorId { get; private set; }
    public EvidenciaTipo Tipo { get; private set; }
    public string NombreOriginal { get; private set; } = string.Empty;
    public string TipoMime { get; private set; } = string.Empty;
    public long TamanoBytes { get; private set; }
    public string UrlAlmacenamiento { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;

    private TicketEvidencia() { }

    public static TicketEvidencia Crear(
        Guid ticketId,
        Guid autorId,
        EvidenciaTipo tipo,
        string nombreOriginal,
        string tipoMime,
        long tamanoBytes,
        string urlAlmacenamiento)
    {
        if (string.IsNullOrWhiteSpace(nombreOriginal))
            throw new ValidationException("NombreOriginal", "El nombre del archivo es requerido.");
        if (string.IsNullOrWhiteSpace(tipoMime))
            throw new ValidationException("TipoMime", "El tipo MIME del archivo es requerido.");
        if (tamanoBytes <= 0)
            throw new ValidationException("TamanoBytes", "El tamaño del archivo debe ser mayor que cero.");
        if (string.IsNullOrWhiteSpace(urlAlmacenamiento))
            throw new ValidationException("UrlAlmacenamiento", "La ruta de almacenamiento es requerida.");

        return new TicketEvidencia
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            AutorId = autorId,
            Tipo = tipo,
            NombreOriginal = nombreOriginal.Trim(),
            TipoMime = tipoMime.Trim().ToLowerInvariant(),
            TamanoBytes = tamanoBytes,
            UrlAlmacenamiento = urlAlmacenamiento,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = autorId
        };
    }

    public void EliminarLogicamente(Guid eliminadoPor)
    {
        if (IsDeleted)
            throw new ValidationException("Estado", "La evidencia ya fue eliminada.");

        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = eliminadoPor;
    }
}
