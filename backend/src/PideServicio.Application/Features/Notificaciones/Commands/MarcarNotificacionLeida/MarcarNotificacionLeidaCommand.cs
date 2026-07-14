namespace PideServicio.Application.Features.Notificaciones.Commands.MarcarNotificacionLeida;

using PideServicio.Application.Common.CQRS;

public sealed record MarcarNotificacionLeidaCommand(Guid NotificacionId) : ICommand;
