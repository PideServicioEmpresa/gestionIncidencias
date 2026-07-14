namespace PideServicio.Infrastructure.Notifications;

using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;

/// <summary>
/// Implementa INotificationService. Crea registros en la tabla notificaciones
/// para el canal IN_APP. No envía emails ni push; esos son servicios separados.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly INotificacionRepository _notifRepo;
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly ICurrentUserService _currentUser;

    public NotificationService(
        INotificacionRepository notifRepo,
        IUsuarioRepository usuarioRepo,
        ICurrentUserService currentUser)
    {
        _notifRepo = notifRepo;
        _usuarioRepo = usuarioRepo;
        _currentUser = currentUser;
    }

    public async Task EnviarAsync(
        Guid usuarioId,
        string titulo,
        string cuerpo,
        string tipoEvento = "general",
        IDictionary<string, string>? datos = null,
        CancellationToken cancellationToken = default)
    {
        var destinatario = await _usuarioRepo.ObtenerPorIdAsync(usuarioId, cancellationToken);
        if (destinatario is null || !destinatario.Activo) return;

        var notificacion = Notificacion.Crear(
            empresaId: destinatario.EmpresaId,
            usuarioId: usuarioId,
            canal: CanalNotificacionTipo.IN_APP,
            titulo: titulo,
            cuerpo: cuerpo,
            tipoEvento: tipoEvento);

        await _notifRepo.CrearAsync(notificacion, cancellationToken);
    }

    public async Task EnviarAEmpresaAsync(
        Guid empresaId,
        string titulo,
        string cuerpo,
        IDictionary<string, string>? datos = null,
        CancellationToken cancellationToken = default)
    {
        // Obtiene técnicos activos de la empresa y crea una notificación IN_APP por cada uno.
        // TODO FASE 6.5+: Extender con administradores y roles adicionales según configuración.
        var tecnicos = await _usuarioRepo.ListarTecnicosActivosPorEmpresaAsync(empresaId, cancellationToken);

        var tareas = tecnicos.Select(t => EnviarAsync(t.Id, titulo, cuerpo, datos: datos, cancellationToken: cancellationToken));
        await Task.WhenAll(tareas);
    }
}
