namespace PideServicio.Application.Features.Tickets.Commands.CancelarTicket;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CancelarTicketCommandHandler : ICommandHandler<CancelarTicketCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly IMotivoCancelacionRepository _motivoCancelacionRepo;
    private readonly IAuditService _auditService;

    public CancelarTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        IMotivoCancelacionRepository motivoCancelacionRepo,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _motivoCancelacionRepo = motivoCancelacionRepo;
        _auditService = auditService;
    }

    public async Task<Result> Handle(CancelarTicketCommand request, CancellationToken cancellationToken)
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

        var esSolicitante = ticket.SolicitanteId == actor.Id;
        var tieneAccesoAdministrativo = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN or RolTipo.SUPERVISOR;
        var puedeCanselarComoSolicitante = esSolicitante &&
            (ticket.Estado == TicketEstadoTipo.NUEVO || ticket.Estado == TicketEstadoTipo.SIN_ASIGNAR);

        if (!tieneAccesoAdministrativo && !puedeCanselarComoSolicitante)
        {
            return Result.NoPermitido(
                "Solo administradores pueden cancelar tickets en proceso. El solicitante solo puede cancelar tickets en estado Nuevo o Sin Asignar.");
        }

        var existeMotivo = await _motivoCancelacionRepo.ExisteAsync(request.MotivoCancelacionId, cancellationToken);
        if (!existeMotivo)
            return Result.NoEncontrado("MotivoCancelacion", request.MotivoCancelacionId);

        var estadoAnterior = ticket.Estado;

        try
        {
            ticket.Cancelar(actor.Id, request.MotivoCancelacionId);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.ESTADO_CAMBIADO,
                actorId: actor.Id,
                estadoAnterior: estadoAnterior,
                estadoNuevo: TicketEstadoTipo.CANCELADO);

            await _historialRepo.CrearAsync(historial, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "CANCELAR",
                new { Estado = estadoAnterior.ToString() },
                new { Estado = ticket.Estado.ToString(), ticket.MotivoCancelacionId },
                cancellationToken);

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
