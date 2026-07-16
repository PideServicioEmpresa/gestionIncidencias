namespace PideServicio.Application.Features.Tickets.Commands.CancelarTicket;

using Microsoft.Extensions.Logging;
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
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly ILogger<CancelarTicketCommandHandler> _logger;

    public CancelarTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        IMotivoCancelacionRepository motivoCancelacionRepo,
        INotificationService notificationService,
        IEmailService emailService,
        IAuditService auditService,
        ILogger<CancelarTicketCommandHandler> logger)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _motivoCancelacionRepo = motivoCancelacionRepo;
        _notificationService = notificationService;
        _emailService = emailService;
        _auditService = auditService;
        _logger = logger;
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

            // Notificaciones push — fire-and-forget con try-catch explícito
            var notifSolicitanteId = ticket.SolicitanteId;
            var notifTicketId = ticket.Id;
            var notifEmpresaId = ticket.EmpresaId;
            var notifCodigo = ticket.Codigo.Valor;
            var notifTitulo = ticket.Titulo;

            if (!esSolicitante)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.EnviarAsync(
                            notifSolicitanteId,
                            "Ticket cancelado",
                            $"Tu ticket {notifCodigo} ha sido cancelado: {notifTitulo}",
                            tipoEvento: "ticket.cancelado",
                            ticketId: notifTicketId,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget EnviarAsync (solicitante) para ticket {Codigo}", notifCodigo);
                    }
                }, CancellationToken.None);
            }

            if (ticket.TecnicoId.HasValue)
            {
                var notifTecnicoId = ticket.TecnicoId.Value;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.EnviarAsync(
                            notifTecnicoId,
                            "Ticket cancelado",
                            $"El ticket {notifCodigo} ha sido cancelado: {notifTitulo}",
                            tipoEvento: "ticket.cancelado",
                            ticketId: notifTicketId,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget EnviarAsync (tecnico) para ticket {Codigo}", notifCodigo);
                    }
                }, CancellationToken.None);
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.EnviarAGestoresYSuperAdminsAsync(
                        notifEmpresaId,
                        "Ticket cancelado",
                        $"El ticket {notifCodigo} fue cancelado: {notifTitulo}",
                        tipoEvento: "ticket.cancelado",
                        ticketId: notifTicketId,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en fire-and-forget EnviarAGestoresYSuperAdminsAsync para ticket {Codigo}", notifCodigo);
                }
            }, CancellationToken.None);

            // Email al solicitante con el motivo de cancelación — fire-and-forget con try-catch explícito
            var correoSolicitante = esSolicitante
                ? actor.Correo.Valor
                : (await _usuarioRepository.ObtenerPorIdAsync(ticket.SolicitanteId, cancellationToken))?.Correo.Valor;

            if (!string.IsNullOrWhiteSpace(correoSolicitante))
            {
                var motivoEntidad = await _motivoCancelacionRepo.ObtenerPorIdAsync(request.MotivoCancelacionId, cancellationToken);
                var motivoTexto = motivoEntidad?.Texto ?? "Cancelado por el sistema";
                var correoCaptura = correoSolicitante;
                var codigoTicket = ticket.Codigo.Valor;
                var tituloTicket = ticket.Titulo;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.NotificarTicketCanceladoAsync(
                            correoSolicitante: correoCaptura,
                            codigo: codigoTicket,
                            titulo: tituloTicket,
                            motivo: motivoTexto,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget NotificarTicketCanceladoAsync para ticket {Codigo}", codigoTicket);
                    }
                }, CancellationToken.None);
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
