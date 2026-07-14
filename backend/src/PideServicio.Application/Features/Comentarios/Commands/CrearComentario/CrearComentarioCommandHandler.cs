namespace PideServicio.Application.Features.Comentarios.Commands.CrearComentario;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CrearComentarioCommandHandler
    : ICommandHandler<CrearComentarioCommand, Guid>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketComentarioRepository _comentarioRepository;
    private readonly ITicketHistorialRepository _historialRepository;

    public CrearComentarioCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketRepository ticketRepository,
        ITicketComentarioRepository comentarioRepository,
        ITicketHistorialRepository historialRepository)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _ticketRepository = ticketRepository;
        _comentarioRepository = comentarioRepository;
        _historialRepository = historialRepository;
    }

    public async Task<Result<Guid>> Handle(
        CrearComentarioCommand request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<Guid>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado<Guid>();

        var ticket = await _ticketRepository.ObtenerPorIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
            return Result.NoEncontrado<Guid>("Ticket", request.TicketId);

        if (actor.Rol != RolTipo.SUPERADMIN && ticket.EmpresaId != actor.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene acceso a este ticket.");

        var tieneAccesoAdministrativo = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN or RolTipo.SUPERVISOR;
        bool esInterno = request.EsInterno && tieneAccesoAdministrativo;

        try
        {
            var comentario = TicketComentario.Crear(
                request.TicketId,
                actor.Id,
                request.Cuerpo,
                esInterno);

            var id = await _comentarioRepository.CrearAsync(comentario, cancellationToken);

            var entrada = TicketHistorialEntrada.Crear(
                ticketId: request.TicketId,
                tipoEvento: TipoEventoHistorialTipo.COMENTADO,
                actorId: actor.Id,
                comentarioTexto: request.Cuerpo);

            await _historialRepository.CrearAsync(entrada, cancellationToken);

            return Result.Exito<Guid>(id);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
    }
}
