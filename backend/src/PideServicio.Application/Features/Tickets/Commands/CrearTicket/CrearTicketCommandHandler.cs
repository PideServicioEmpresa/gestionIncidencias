namespace PideServicio.Application.Features.Tickets.Commands.CrearTicket;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Exceptions;
using StringComparison = System.StringComparison;

public sealed class CrearTicketCommandHandler : ICommandHandler<CrearTicketCommand, Guid>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAreaRepository _areaRepository;
    private readonly ITicketRepository _ticketRepo;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly IAuditService _auditService;

    public CrearTicketCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        IAreaRepository areaRepository,
        ITicketRepository ticketRepo,
        INotificationService notificationService,
        IEmailService emailService,
        IAuditService auditService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _areaRepository = areaRepository;
        _ticketRepo = ticketRepo;
        _notificationService = notificationService;
        _emailService = emailService;
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
            // Resolver el área: buscar por nombre en la sucursal, crear si no existe
            var areasExistentes = await _areaRepository.ListarPorSucursalAsync(
                request.SucursalId, soloActivas: true, ct: cancellationToken);

            var areaEncontrada = areasExistentes.FirstOrDefault(a =>
                string.Equals(a.Nombre, request.AreaNombre.Trim(), StringComparison.OrdinalIgnoreCase));

            Guid areaId;
            var areaNombre = request.AreaNombre.Trim();
            if (areaEncontrada is not null)
            {
                areaId = areaEncontrada.Id;
                areaNombre = areaEncontrada.Nombre;
            }
            else
            {
                var nuevaArea = Area.Crear(request.SucursalId, areaNombre, creadoPor: actor.Id);
                areaId = await _areaRepository.CrearAsync(nuevaArea, cancellationToken);
            }

            var codigo = await _ticketRepo.GenerarCodigoAsync(cancellationToken);

            var ticket = Ticket.Crear(
                codigo,
                request.Titulo,
                request.Descripcion,
                actor.EmpresaId,
                request.SucursalId,
                areaId,
                request.TipoServicioId,
                request.CategoriaId,
                request.Prioridad,
                actor.Id,
                request.Ubicacion,
                actor.Id);

            ticket.SubmitParaAsignacion(actor.Id);

            var id = await _ticketRepo.CrearAsync(ticket, cancellationToken);

            await _auditService.RegistrarAsync(
                "tickets",
                id,
                "CREAR",
                null,
                new { ticket.Estado, ticket.Titulo, ticket.SolicitanteId },
                cancellationToken);

            _ = _emailService.NotificarTicketCreadoAsync(
                correoSolicitante: actor.Correo.Valor,
                codigo: ticket.Codigo.Valor,
                titulo: ticket.Titulo,
                prioridad: ticket.PrioridadEfectiva.ToString(),
                area: areaNombre,
                solicitante: actor.NombreCompleto,
                cancellationToken: CancellationToken.None);

            await _notificationService.EnviarAGestoresYSuperAdminsAsync(
                ticket.EmpresaId,
                "Nuevo ticket creado",
                $"Se creó el ticket {ticket.Codigo.Valor}: {ticket.Titulo}",
                tipoEvento: "ticket.nuevo",
                ticketId: id,
                cancellationToken: cancellationToken);

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
