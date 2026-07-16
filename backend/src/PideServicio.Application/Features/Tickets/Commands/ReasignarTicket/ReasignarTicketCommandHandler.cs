namespace PideServicio.Application.Features.Tickets.Commands.ReasignarTicket;

using Microsoft.Extensions.Logging;
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
    private readonly ISucursalRepository _sucursalRepository;
    private readonly IAreaRepository _areaRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly ITicketAsignacionRepository _asignacionRepo;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly ILogger<ReasignarTicketCommandHandler> _logger;

    public ReasignarTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ISucursalRepository sucursalRepository,
        IAreaRepository areaRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        ITicketAsignacionRepository asignacionRepo,
        INotificationService notificationService,
        IEmailService emailService,
        IAuditService auditService,
        ILogger<ReasignarTicketCommandHandler> logger)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _sucursalRepository = sucursalRepository;
        _areaRepository = areaRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _asignacionRepo = asignacionRepo;
        _notificationService = notificationService;
        _emailService = emailService;
        _auditService = auditService;
        _logger = logger;
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

            // Notificaciones push — fire-and-forget con try-catch explícito
            var notifTicketId = ticket.Id;
            var notifEmpresaId = ticket.EmpresaId;
            var notifCodigo = ticket.Codigo.Valor;
            var notifTitulo = ticket.Titulo;
            var notifNuevoTecnicoId = request.NuevoTecnicoId;

            if (tecnicoAnteriorId.HasValue)
            {
                var notifTecnicoAnteriorId = tecnicoAnteriorId.Value;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.EnviarAsync(
                            notifTecnicoAnteriorId,
                            "Ticket reasignado",
                            $"Has sido desasignado del ticket {notifCodigo}: {notifTitulo}",
                            tipoEvento: "ticket.reasignado",
                            ticketId: notifTicketId,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget EnviarAsync (tecnico anterior) para ticket {Codigo}", notifCodigo);
                    }
                }, CancellationToken.None);
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.EnviarAsync(
                        notifNuevoTecnicoId,
                        "Ticket reasignado",
                        $"Se te ha reasignado el ticket {notifCodigo}: {notifTitulo}",
                        tipoEvento: "ticket.reasignado",
                        ticketId: notifTicketId,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en fire-and-forget EnviarAsync (nuevo tecnico) para ticket {Codigo}", notifCodigo);
                }
            }, CancellationToken.None);

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.EnviarAGestoresYSuperAdminsAsync(
                        notifEmpresaId,
                        "Ticket reasignado",
                        $"El ticket {notifCodigo} fue reasignado: {notifTitulo}",
                        tipoEvento: "ticket.reasignado",
                        ticketId: notifTicketId,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en fire-and-forget EnviarAGestoresYSuperAdminsAsync para ticket {Codigo}", notifCodigo);
                }
            }, CancellationToken.None);

            // Emails — fire-and-forget con try-catch explícito
            var nuevoTecnico = await _usuarioRepository.ObtenerPorIdAsync(request.NuevoTecnicoId, cancellationToken);
            var solicitante = await _usuarioRepository.ObtenerPorIdAsync(ticket.SolicitanteId, cancellationToken);
            var sucursal = await _sucursalRepository.ObtenerPorIdAsync(ticket.SucursalId, cancellationToken);
            var area = await _areaRepository.ObtenerPorIdAsync(ticket.AreaId, cancellationToken);

            var codigoTicket = ticket.Codigo.Valor;
            var tituloTicket = ticket.Titulo;
            var motivo = request.Motivo;
            var prioridadTicket = ticket.PrioridadEfectiva.ToString();
            var sucursalNombre = sucursal?.Nombre;
            var areaNombre = area?.Nombre;
            var solicitanteNombre = solicitante?.NombreCompleto;

            // Email al técnico ANTERIOR informando la desasignación
            if (tecnicoAnteriorId.HasValue)
            {
                var tecnicoAnterior = await _usuarioRepository.ObtenerPorIdAsync(tecnicoAnteriorId.Value, cancellationToken);
                if (tecnicoAnterior is not null)
                {
                    var correoTecnicoAnterior = tecnicoAnterior.Correo.Valor;
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.NotificarDesasignacionTecnicoAsync(
                                correoTecnico: correoTecnicoAnterior,
                                codigo: codigoTicket,
                                titulo: tituloTicket,
                                motivo: motivo,
                                cancellationToken: CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error en fire-and-forget NotificarDesasignacionTecnicoAsync para ticket {Codigo}", codigoTicket);
                        }
                    }, CancellationToken.None);
                }
            }

            // Email al técnico NUEVO y al solicitante
            if (nuevoTecnico is not null)
            {
                var correoNuevoTecnico = nuevoTecnico.Correo.Valor;
                var nombreNuevoTecnico = nuevoTecnico.NombreCompleto;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.NotificarTicketReasignadoAsync(
                            correoTecnico: correoNuevoTecnico,
                            codigo: codigoTicket,
                            titulo: tituloTicket,
                            tecnico: nombreNuevoTecnico,
                            prioridad: prioridadTicket,
                            motivo: motivo,
                            sucursal: sucursalNombre,
                            area: areaNombre,
                            solicitante: solicitanteNombre,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget NotificarTicketReasignadoAsync para ticket {Codigo}", codigoTicket);
                    }
                }, CancellationToken.None);

                if (solicitante is not null)
                {
                    var correoSolicitante = solicitante.Correo.Valor;
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.NotificarAsignacionASolicitanteAsync(
                                correoSolicitante: correoSolicitante,
                                codigo: codigoTicket,
                                titulo: tituloTicket,
                                tecnico: nombreNuevoTecnico,
                                prioridad: prioridadTicket,
                                cancellationToken: CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error en fire-and-forget NotificarAsignacionASolicitanteAsync para ticket {Codigo}", codigoTicket);
                        }
                    }, CancellationToken.None);
                }
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
