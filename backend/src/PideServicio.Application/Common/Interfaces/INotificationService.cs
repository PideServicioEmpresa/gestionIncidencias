namespace PideServicio.Application.Common.Interfaces;

public interface INotificationService
{
    Task EnviarAsync(Guid usuarioId, string titulo, string cuerpo, string tipoEvento = "general", Guid? ticketId = null, CancellationToken cancellationToken = default);
    Task EnviarAEmpresaAsync(Guid empresaId, string titulo, string cuerpo, string tipoEvento = "general", Guid? ticketId = null, CancellationToken cancellationToken = default);
    Task EnviarAGestoresYSuperAdminsAsync(Guid empresaId, string titulo, string cuerpo, string tipoEvento = "general", Guid? ticketId = null, CancellationToken cancellationToken = default);
}
