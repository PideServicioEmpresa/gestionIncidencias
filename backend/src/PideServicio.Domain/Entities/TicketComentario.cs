namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Comentario en un ticket. Usa editado_en en lugar de updated_at.
/// es_interno = true significa que solo personal interno (Admin/Técnico) puede verlo.
/// </summary>
public sealed class TicketComentario : BaseEntity
{
    public Guid TicketId { get; private set; }
    public Guid AutorId { get; private set; }
    public string Cuerpo { get; private set; } = string.Empty;
    public bool EsInterno { get; private set; }
    public DateTimeOffset? EditadoEn { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;
    public bool FueEditado => EditadoEn.HasValue;

    private TicketComentario() { }

    public static TicketComentario Crear(
        Guid ticketId,
        Guid autorId,
        string cuerpo,
        bool esInterno = false)
    {
        if (string.IsNullOrWhiteSpace(cuerpo))
            throw new ValidationException("Cuerpo", "El cuerpo del comentario es requerido.");
        if (cuerpo.Length > 2000)
            throw new ValidationException("Cuerpo", "El comentario no puede exceder 2000 caracteres.");

        return new TicketComentario
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            AutorId = autorId,
            Cuerpo = cuerpo.Trim(),
            EsInterno = esInterno,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = autorId
        };
    }

    public void Editar(string nuevoCuerpo, Guid editorId)
    {
        if (IsDeleted)
            throw new ValidationException("Estado", "No se puede editar un comentario eliminado.");
        if (string.IsNullOrWhiteSpace(nuevoCuerpo))
            throw new ValidationException("Cuerpo", "El cuerpo del comentario es requerido.");
        if (nuevoCuerpo.Length > 2000)
            throw new ValidationException("Cuerpo", "El comentario no puede exceder 2000 caracteres.");

        Cuerpo = nuevoCuerpo.Trim();
        EditadoEn = DateTimeOffset.UtcNow;
        UpdatedBy = editorId;
    }

    public void EliminarLogicamente(Guid eliminadoPor)
    {
        if (IsDeleted)
            throw new ValidationException("Estado", "El comentario ya fue eliminado.");

        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = eliminadoPor;
    }
}
