namespace PideServicio.Infrastructure.Notifications;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PideServicio.Application.Common.Interfaces;

/// <summary>
/// Implementa IPushNotificationService. Stub para Web Push.
/// La implementación real (Web Push Protocol / FCM) se realiza en FASE 6.5+.
/// Si PushHabilitado = false, el envío se omite silenciosamente.
/// </summary>
public sealed class PushNotificationService : IPushNotificationService
{
    private readonly NotificationOptions _options;
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(
        IOptions<NotificationOptions> options,
        ILogger<PushNotificationService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task EnviarAsync(
        Guid usuarioId,
        string titulo,
        string cuerpo,
        IDictionary<string, string>? datos = null,
        CancellationToken ct = default)
    {
        if (!_options.PushHabilitado)
        {
            _logger.LogDebug(
                "Push deshabilitado. Se omite notificación a {UsuarioId}: {Titulo}",
                usuarioId, titulo);
            return;
        }

        // TODO FASE 6.5+: Implementar Web Push Protocol usando suscripciones almacenadas
        // en la tabla push_subscriptions. Considerar FCM como alternativa.
        _logger.LogWarning(
            "Push notifications pendiente de implementar. UsuarioId: {UsuarioId}",
            usuarioId);

        await Task.CompletedTask;
    }

    public async Task EnviarAMultiplesAsync(
        IReadOnlyList<Guid> usuarioIds,
        string titulo,
        string cuerpo,
        IDictionary<string, string>? datos = null,
        CancellationToken ct = default)
    {
        var tareas = usuarioIds.Select(id => EnviarAsync(id, titulo, cuerpo, datos, ct));
        await Task.WhenAll(tareas);
    }
}
