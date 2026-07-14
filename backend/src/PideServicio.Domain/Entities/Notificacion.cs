namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Notificación enviada a un usuario. Una notificación lógica puede tener múltiples registros,
/// uno por canal (email, push, in-app). estado_entrega se actualiza via canal de entrega.
/// </summary>
public sealed class Notificacion : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid? TicketId { get; private set; }
    public string TipoEvento { get; private set; } = "general";
    public CanalNotificacionTipo Canal { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Cuerpo { get; private set; } = string.Empty;
    public EstadoEntregaTipo EstadoEntrega { get; private set; } = EstadoEntregaTipo.PENDIENTE;
    public DateTimeOffset? EnviadoEn { get; private set; }
    public DateTimeOffset? LeidoEn { get; private set; }
    public int Intentos { get; private set; }
    public string? ErrorEntrega { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Notificacion() { }

    public static Notificacion Crear(
        Guid empresaId,
        Guid usuarioId,
        CanalNotificacionTipo canal,
        string titulo,
        string cuerpo,
        Guid? ticketId = null,
        string tipoEvento = "general")
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ValidationException("Titulo", "El título de la notificación es requerido.");
        if (string.IsNullOrWhiteSpace(cuerpo))
            throw new ValidationException("Cuerpo", "El cuerpo de la notificación es requerido.");

        return new Notificacion
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            UsuarioId = usuarioId,
            TicketId = ticketId,
            TipoEvento = tipoEvento,
            Canal = canal,
            Titulo = titulo.Trim(),
            Cuerpo = cuerpo.Trim(),
            EstadoEntrega = EstadoEntregaTipo.PENDIENTE,
            Intentos = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void MarcarEnviada()
    {
        EstadoEntrega = EstadoEntregaTipo.ENVIADO;
        EnviadoEn = DateTimeOffset.UtcNow;
    }

    public void MarcarLeida()
    {
        LeidoEn = DateTimeOffset.UtcNow;
    }

    public void RegistrarError(string mensaje)
    {
        Intentos++;
        ErrorEntrega = mensaje;
        EstadoEntrega = EstadoEntregaTipo.FALLIDO;
    }

    public void Reintentar()
    {
        EstadoEntrega = EstadoEntregaTipo.PENDIENTE;
        ErrorEntrega = null;
    }
}
