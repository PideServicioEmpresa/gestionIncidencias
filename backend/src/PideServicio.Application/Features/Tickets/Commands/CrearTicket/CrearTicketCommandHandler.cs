namespace PideServicio.Application.Features.Tickets.Commands.CrearTicket;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Exceptions;

public sealed class CrearTicketCommandHandler : ICommandHandler<CrearTicketCommand, Guid>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly IAuditService _auditService;

    public CrearTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepo,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepo = ticketRepo;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(CrearTicketCommand request, CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<Guid>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado<Guid>();

        try
        {
            var codigo = await _ticketRepo.GenerarCodigoAsync(cancellationToken);

            var ticket = Ticket.Crear(
                codigo,
                request.Titulo,
                request.Descripcion,
                actor.EmpresaId,
                request.SucursalId,
                request.AreaId,
                request.TipoServicioId,
                request.CategoriaId,
                request.Prioridad,
                actor.Id,
                request.Ubicacion,
                actor.Id);

            ticket.SubmitParaAsignacion(actor.Id);

            var id = await _ticketRepo.CrearAsync(ticket, cancellationToken);

            // El trigger trg_tickets_after_insert inserta automáticamente
            // la entrada CREADO en ticket_historial; no duplicar aquí.

            await _auditService.RegistrarAsync(
                "tickets",
                id,
                "CREAR",
                null,
                new { ticket.Estado, ticket.Titulo, ticket.SolicitanteId },
                cancellationToken);

            return Result.Exito(id);
        }
        catch (TransicionEstadoInvalidaException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
    }
}
