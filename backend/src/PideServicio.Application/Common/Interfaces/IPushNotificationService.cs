namespace PideServicio.Application.Common.Interfaces;

public interface IPushNotificationService
{
    Task EnviarAsync(
        Guid usuarioId,
        string titulo,
        string cuerpo,
        IDictionary<string, string>? datos = null,
        CancellationToken ct = default);

    Task EnviarAMultiplesAsync(
        IReadOnlyList<Guid> usuarioIds,
        string titulo,
        string cuerpo,
        IDictionary<string, string>? datos = null,
        CancellationToken ct = default);
}
