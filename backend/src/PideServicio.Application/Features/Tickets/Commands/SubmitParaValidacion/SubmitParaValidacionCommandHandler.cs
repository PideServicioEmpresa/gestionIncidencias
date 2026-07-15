namespace PideServicio.Application.Features.Tickets.Commands.SubmitParaValidacion;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class SubmitParaValidacionCommandHandler : ICommandHandler<SubmitParaValidacionCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;

    public SubmitParaValidacionCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        INotificationService notificationService,
        IEmailService emailService,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _notificationService = notificationService;
        _emailService = emailService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(SubmitParaValidacionCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado("Ticket", request.TicketId);

        var esTecnicoAsignado = ticket.TecnicoId == actor.Id;
        var tieneAccesoAdministrativo = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN or RolTipo.SUPERVISOR;
        if (!esTecnicoAsignado && !tieneAccesoAdministrativo)
            return Result.NoPermitido("Solo el técnico asignado o un administrador puede enviar el ticket a validación.");

        var estadoAnterior = ticket.Estado;

        try
        {
            ticket.SubmitParaValidacion(actor.Id);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.ESTADO_CAMBIADO,
                actorId: actor.Id,
                estadoAnterior: estadoAnterior,
                estadoNuevo: TicketEstadoTipo.PENDIENTE_VALIDACION);

            await _historialRepo.CrearAsync(historial, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "SUBMIT_VALIDACION",
                new { Estado = estadoAnterior.ToString() },
                new { Estado = ticket.Estado.ToString() },
                cancellationToken);

            await _notificationService.EnviarAsync(
                ticket.SolicitanteId,
                "Ticket pendiente de validación",
                $"El ticket {ticket.Codigo.Valor} está listo para tu validación: {ticket.Titulo}",
                tipoEvento: "ticket.pendiente_validacion",
                datos: new Dictionary<string, string>
                {
                    ["ticketId"] = ticket.Id.ToString(),
                    ["codigo"] = ticket.Codigo.Valor
                },
                cancellationToken: cancellationToken);

            // Email al solicitante con copia a inmoveg
            var solicitante = await _usuarioRepository.ObtenerPorIdAsync(ticket.SolicitanteId, cancellationToken);
            if (solicitante is not null)
            {
                // El técnico es el actor si es técnico asignado; si es admin, usar el nombre del actor igualmente
                var tecnicoNombre = actor.NombreCompleto;
                if (!esTecnicoAsignado && ticket.TecnicoId.HasValue)
                {
                    var tecnico = await _usuarioRepository.ObtenerPorIdAsync(ticket.TecnicoId.Value, cancellationToken);
                    tecnicoNombre = tecnico?.NombreCompleto ?? tecnicoNombre;
                }

                _ = _emailService.NotificarTicketPendienteValidacionAsync(
                    correoSolicitante: solicitante.Correo.Valor,
                    codigo: ticket.Codigo.Valor,
                    titulo: ticket.Titulo,
                    tecnico: tecnicoNombre,
                    cancellationToken: CancellationToken.None);
            }

            return Result.Exito();
        }
        catch (TransicionEstadoInvalidaException ex)
        {
            return Result.Fallo(ex.Message);
        }
        catch (TicketCerradoException ex)
        {
            return Result.Fallo(ex.Message);
        }
        catch (TicketCanceladoException ex)
        {
            return Result.Fallo(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
