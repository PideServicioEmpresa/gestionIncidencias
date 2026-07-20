namespace PideServicio.Application.Features.Tickets.Commands.CambiarArea;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CambiarAreaCommandHandler : ICommandHandler<CambiarAreaCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly IAuditService _auditService;

    public CambiarAreaCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _auditService = auditService;
    }

    public async Task<Result> Handle(CambiarAreaCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        var tieneAccesoAdministrativo = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN or RolTipo.SUPERVISOR;
        if (!tieneAccesoAdministrativo)
            return Result.NoPermitido("Solo administradores y supervisores pueden cambiar el área de un ticket.");

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado("Ticket", request.TicketId);

        var areaAnteriorId = ticket.AreaId;

        try
        {
            if (ticket.Estado == TicketEstadoTipo.SIN_ASIGNAR)
            {
                ticket.CambiarArea(request.NuevaAreaId, actor.Id);
            }
            else if (actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN)
            {
                ticket.CambiarAreaAdmin(request.NuevaAreaId, actor.Id);
            }
            else
            {
                return Result.NoPermitido(
                    "El supervisor solo puede cambiar el área cuando el ticket está en estado Sin Asignar.");
            }

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.AREA_CAMBIADA,
                actorId: actor.Id,
                metadata: $"{{\"areaAnterior\":\"{areaAnteriorId}\",\"areaNueva\":\"{request.NuevaAreaId}\"}}");

            await _historialRepo.CrearAsync(historial, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "CAMBIAR_AREA",
                new { AreaId = areaAnteriorId },
                new { AreaId = request.NuevaAreaId },
                cancellationToken);

            return Result.Exito();
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
