namespace PideServicio.Application.Features.Tickets.Commands.ReasignarTicket;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ReasignarTicketCommandHandler : ICommandHandler<ReasignarTicketCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly ITicketAsignacionRepository _asignacionRepo;
    private readonly INotificationService _notificationService;
    private readonly IAuditService _auditService;

    public ReasignarTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        ITicketAsignacionRepository asignacionRepo,
        INotificationService notificationService,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _asignacionRepo = asignacionRepo;
        _notificationService = notificationService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(ReasignarTicketCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        if (actor.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido("Solo administradores pueden reasignar tickets.");

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado("Ticket", request.TicketId);

        var tecnicoAnteriorId = ticket.TecnicoId;
        var estadoAnterior = ticket.Estado;

        try
        {
            ticket.Reasignar(request.NuevoTecnicoId, actor.Id, request.Motivo);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.REASIGNADO,
                actorId: actor.Id,
                estadoAnterior: estadoAnterior,
                estadoNuevo: ticket.Estado,
                comentarioTexto: request.Motivo);

            await _historialRepo.CrearAsync(historial, cancellationToken);

            var asignacion = TicketAsignacion.Registrar(
                ticketId: ticket.Id,
                tecnicoId: request.NuevoTecnicoId,
                asignadorId: actor.Id,
                esReasignacion: true,
                tecnicoAnteriorId: tecnicoAnteriorId,
                motivoReasignacion: request.Motivo);

            await _asignacionRepo.CrearAsync(asignacion, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "REASIGNAR",
                new { TecnicoId = tecnicoAnteriorId },
                new { TecnicoId = request.NuevoTecnicoId, Motivo = request.Motivo },
                cancellationToken);

            await _notificationService.EnviarAsync(
                request.NuevoTecnicoId,
                "Ticket reasignado",
                $"Se te ha reasignado el ticket {ticket.Codigo.Valor}: {ticket.Titulo}",
                tipoEvento: "ticket.reasignado",
                ticketId: ticket.Id,
                cancellationToken: cancellationToken);

            await _notificationService.EnviarAGestoresYSuperAdminsAsync(
                ticket.EmpresaId,
                "Ticket reasignado",
                $"El ticket {ticket.Codigo.Valor} fue reasignado: {ticket.Titulo}",
                tipoEvento: "ticket.reasignado",
                ticketId: ticket.Id,
                cancellationToken: cancellationToken);

            return Result.Exito();
        }
        catch (TransicionEstadoInvalidaException ex)
        {
            return Result.Fallo(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
