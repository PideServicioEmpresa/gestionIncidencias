namespace PideServicio.Application.Features.Tickets.Commands.CambiarSucursal;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CambiarSucursalCommandHandler : ICommandHandler<CambiarSucursalCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISucursalRepository _sucursalRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly IAuditService _auditService;

    public CambiarSucursalCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ISucursalRepository sucursalRepository,
        ITicketRepository ticketRepo,
        ITicketHistorialRepository historialRepo,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _sucursalRepository = sucursalRepository;
        _ticketRepo = ticketRepo;
        _historialRepo = historialRepo;
        _auditService = auditService;
    }

    public async Task<Result> Handle(CambiarSucursalCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        if (actor.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido("Solo administradores pueden cambiar la sucursal de un ticket.");

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado("Ticket", request.TicketId);

        // Aislamiento entre empresas: nadie opera sobre tickets de una empresa ajena
        if (actor.Rol != RolTipo.SUPERADMIN && ticket.EmpresaId != actor.EmpresaId)
            return Result.NoPermitido("No tiene acceso a este ticket.");

        if (ticket.SucursalId == request.NuevaSucursalId)
            return Result.Exito();

        // La sucursal destino debe existir, estar activa y pertenecer a la empresa del ticket
        var sucursalDestino = await _sucursalRepository.ObtenerPorIdAsync(request.NuevaSucursalId, cancellationToken);
        if (sucursalDestino is null)
            return Result.ErrorValidacion("NuevaSucursalId", "La sucursal indicada no existe.");

        if (!sucursalDestino.Activa)
            return Result.ErrorValidacion("NuevaSucursalId", "La sucursal indicada está inactiva.");

        if (sucursalDestino.EmpresaId != ticket.EmpresaId)
            return Result.ErrorValidacion("NuevaSucursalId", "La sucursal indicada no pertenece a la empresa del ticket.");

        var sucursalAnteriorId = ticket.SucursalId;
        var tecnicoAnteriorId = ticket.TecnicoId;

        try
        {
            var huboDesasignacion = ticket.CambiarSucursal(request.NuevaSucursalId, actor.Id);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.SUCURSAL_CAMBIADA,
                actorId: actor.Id,
                metadata: $"{{\"sucursalAnterior\":\"{sucursalAnteriorId}\",\"sucursalNueva\":\"{request.NuevaSucursalId}\"}}");

            await _historialRepo.CrearAsync(historial, cancellationToken);

            if (huboDesasignacion)
            {
                var historialDesasignacion = TicketHistorialEntrada.Crear(
                    ticketId: ticket.Id,
                    tipoEvento: TipoEventoHistorialTipo.ESTADO_CAMBIADO,
                    actorId: actor.Id,
                    metadata: $"{{\"motivo\":\"Desasignación automática por cambio de sucursal\",\"tecnicoAnterior\":\"{tecnicoAnteriorId}\"}}");

                await _historialRepo.CrearAsync(historialDesasignacion, cancellationToken);
            }

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "CAMBIAR_SUCURSAL",
                new { SucursalId = sucursalAnteriorId, TecnicoId = tecnicoAnteriorId },
                new { SucursalId = request.NuevaSucursalId, TecnicoId = ticket.TecnicoId },
                cancellationToken);

            return Result.Exito();
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
