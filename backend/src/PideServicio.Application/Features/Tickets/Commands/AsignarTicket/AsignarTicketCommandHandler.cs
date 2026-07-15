namespace PideServicio.Application.Features.Tickets.Commands.AsignarTicket;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class AsignarTicketCommandHandler : ICommandHandler<AsignarTicketCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly ITicketAsignacionRepository _asignacionRepo;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;

    public AsignarTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        ITicketAsignacionRepository asignacionRepo,
        INotificationService notificationService,
        IEmailService emailService,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _asignacionRepo = asignacionRepo;
        _notificationService = notificationService;
        _emailService = emailService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(AsignarTicketCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        if (actor.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN or RolTipo.SUPERVISOR))
            return Result.NoPermitido("Solo administradores y supervisores pueden asignar tickets.");

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado("Ticket", request.TicketId);

        var estadoAnterior = ticket.Estado;

        try
        {
            ticket.Asignar(request.TecnicoId, actor.Id);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.ASIGNADO,
                actorId: actor.Id,
                estadoAnterior: estadoAnterior,
                estadoNuevo: TicketEstadoTipo.ASIGNADO);

            await _historialRepo.CrearAsync(historial, cancellationToken);

            var asignacion = TicketAsignacion.Registrar(
                ticketId: ticket.Id,
                tecnicoId: request.TecnicoId,
                asignadorId: actor.Id,
                esReasignacion: false);

            await _asignacionRepo.CrearAsync(asignacion, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "ASIGNAR",
                new { Estado = estadoAnterior.ToString(), TecnicoId = (Guid?)null },
                new { Estado = ticket.Estado.ToString(), ticket.TecnicoId },
                cancellationToken);

            await _notificationService.EnviarAsync(
                request.TecnicoId,
                "Ticket asignado",
                $"Se te ha asignado el ticket {ticket.Codigo.Valor}: {ticket.Titulo}",
                tipoEvento: "ticket.asignado",
                datos: new Dictionary<string, string>
                {
                    ["ticketId"] = ticket.Id.ToString(),
                    ["codigo"] = ticket.Codigo.Valor
                },
                cancellationToken: cancellationToken);

            // Email al técnico con copia a inmoveg
            var tecnico = await _usuarioRepository.ObtenerPorIdAsync(request.TecnicoId, cancellationToken);
            if (tecnico is not null)
            {
                _ = _emailService.NotificarTicketAsignadoAsync(
                    correoTecnico: tecnico.Correo.Valor,
                    codigo: ticket.Codigo.Valor,
                    titulo: ticket.Titulo,
                    tecnico: tecnico.NombreCompleto,
                    prioridad: ticket.PrioridadEfectiva.ToString(),
                    cancellationToken: CancellationToken.None);
            }

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
