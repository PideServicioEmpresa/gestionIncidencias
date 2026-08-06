namespace PideServicio.Application.Common.Interfaces;

public interface INotificationService
{
    Task EnviarAsync(Guid usuarioId, string titulo, string cuerpo, string tipoEvento = "general", Guid? ticketId = null, CancellationToken cancellationToken = default);
    Task EnviarAEmpresaAsync(Guid empresaId, string titulo, string cuerpo, string tipoEvento = "general", Guid? ticketId = null, CancellationToken cancellationToken = default);
    Task EnviarAGestoresYSuperAdminsAsync(
        Guid empresaId,
        string titulo,
        string cuerpo,
        string tipoEvento = "general",
        Guid? ticketId = null,
        string? eventoEmail = null,
        string? codigoEmail = null,
        string? tituloEmail = null,
        string? actorNombreEmail = null,
        string? detalleEmail = null,
        Guid? actorId = null,
        CancellationToken cancellationToken = default);
}
