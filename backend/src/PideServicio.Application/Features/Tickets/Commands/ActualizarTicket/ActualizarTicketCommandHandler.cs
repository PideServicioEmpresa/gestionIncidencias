namespace PideServicio.Application.Features.Tickets.Commands.ActualizarTicket;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ActualizarTicketCommandHandler : ICommandHandler<ActualizarTicketCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly ITicketHistorialRepository _historialRepo;
    private readonly IAuditService _auditService;

    public ActualizarTicketCommandHandler(
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

    public async Task<Result> Handle(ActualizarTicketCommand request, CancellationToken cancellationToken)
    {
        if (request.NuevoTitulo is null && !request.NuevoTipoServicioId.HasValue)
            return Result.Exito();

        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        if (actor.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido("Solo administradores pueden actualizar el título y tipo de servicio de un ticket.");

        var ticket = await _ticketRepo.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado("Ticket", request.TicketId);

        var tituloAnterior = ticket.Titulo;
        var tipoAnterior = ticket.TipoServicioId;

        try
        {
            ticket.ActualizarDatos(request.NuevoTitulo, request.NuevoTipoServicioId, actor.Id);

            await _ticketRepo.ActualizarAsync(ticket, cancellationToken);

            var historial = TicketHistorialEntrada.Crear(
                ticketId: ticket.Id,
                tipoEvento: TipoEventoHistorialTipo.DATOS_ACTUALIZADOS,
                actorId: actor.Id,
                metadata: $"{{\"tituloAnterior\":\"{tituloAnterior}\",\"tipoAnterior\":\"{tipoAnterior}\"}}");

            await _historialRepo.CrearAsync(historial, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                ticket.Id,
                "ACTUALIZAR_DATOS",
                new { Titulo = tituloAnterior, TipoServicioId = tipoAnterior },
                new { Titulo = ticket.Titulo, TipoServicioId = ticket.TipoServicioId },
                cancellationToken);

            return Result.Exito();
        }
        catch (ValidationException ex)
        {
            return Result.Fallo(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
