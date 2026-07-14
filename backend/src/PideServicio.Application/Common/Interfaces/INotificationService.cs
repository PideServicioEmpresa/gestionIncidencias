namespace PideServicio.Application.Common.Interfaces;

public interface INotificationService
{
    Task EnviarAsync(Guid usuarioId, string titulo, string cuerpo, string tipoEvento = "general", IDictionary<string, string>? datos = null, CancellationToken cancellationToken = default);
    Task EnviarAEmpresaAsync(Guid empresaId, string titulo, string cuerpo, IDictionary<string, string>? datos = null, CancellationToken cancellationToken = default);
}
