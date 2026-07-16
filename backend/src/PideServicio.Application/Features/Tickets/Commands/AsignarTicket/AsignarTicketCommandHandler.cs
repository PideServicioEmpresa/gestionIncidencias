namespace PideServicio.Application.Features.Tickets.Commands.AsignarTicket;

using Microsoft.Extensions.Logging;
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
    private readonly ISucursalRepository _sucursalRepository;
    private readonly IAreaRepository _areaRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly ITicketAsignacionRepository _asignacionRepo;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;
    private readonly ILogger<AsignarTicketCommandHandler> _logger;

    public AsignarTicketCommandHandler(
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
        ILogger<AsignarTicketCommandHandler> logger)
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

            // Notificaciones push — fire-and-forget con try-catch explícito
            var notifTecnicoId = request.TecnicoId;
            var notifTicketId = ticket.Id;
            var notifEmpresaId = ticket.EmpresaId;
            var notifCodigo = ticket.Codigo.Valor;
            var notifTitulo = ticket.Titulo;

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.EnviarAsync(
                        notifTecnicoId,
                        "Ticket asignado",
                        $"Se te ha asignado el ticket {notifCodigo}: {notifTitulo}",
                        tipoEvento: "ticket.asignado",
                        ticketId: notifTicketId,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en fire-and-forget EnviarAsync (tecnico) para ticket {Codigo}", notifCodigo);
                }
            }, CancellationToken.None);

            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationService.EnviarAGestoresYSuperAdminsAsync(
                        notifEmpresaId,
                        "Ticket asignado",
                        $"El ticket {notifCodigo} fue asignado: {notifTitulo}",
                        tipoEvento: "ticket.asignado",
                        ticketId: notifTicketId,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en fire-and-forget EnviarAGestoresYSuperAdminsAsync para ticket {Codigo}", notifCodigo);
                }
            }, CancellationToken.None);

            // Email al técnico y al solicitante — fire-and-forget con try-catch explícito
            var tecnico = await _usuarioRepository.ObtenerPorIdAsync(request.TecnicoId, cancellationToken);
            var solicitante = await _usuarioRepository.ObtenerPorIdAsync(ticket.SolicitanteId, cancellationToken);
            var sucursal = await _sucursalRepository.ObtenerPorIdAsync(ticket.SucursalId, cancellationToken);
            var area = await _areaRepository.ObtenerPorIdAsync(ticket.AreaId, cancellationToken);

            if (tecnico is not null)
            {
                var correoTecnico = tecnico.Correo.Valor;
                var nombreTecnico = tecnico.NombreCompleto;
                var codigoTicket = ticket.Codigo.Valor;
                var tituloTicket = ticket.Titulo;
                var prioridadTicket = ticket.PrioridadEfectiva.ToString();
                var sucursalNombre = sucursal?.Nombre;
                var areaNombre = area?.Nombre;
                var solicitanteNombre = solicitante?.NombreCompleto;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.NotificarTicketAsignadoAsync(
                            correoTecnico: correoTecnico,
                            codigo: codigoTicket,
                            titulo: tituloTicket,
                            tecnico: nombreTecnico,
                            prioridad: prioridadTicket,
                            sucursal: sucursalNombre,
                            area: areaNombre,
                            solicitante: solicitanteNombre,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en fire-and-forget NotificarTicketAsignadoAsync para ticket {Codigo}", codigoTicket);
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
                                tecnico: nombreTecnico,
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
