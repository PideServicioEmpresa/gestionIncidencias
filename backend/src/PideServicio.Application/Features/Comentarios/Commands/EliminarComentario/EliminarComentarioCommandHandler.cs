namespace PideServicio.Application.Features.Comentarios.Commands.EliminarComentario;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class EliminarComentarioCommandHandler
    : ICommandHandler<EliminarComentarioCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketComentarioRepository _comentarioRepository;

    public EliminarComentarioCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketComentarioRepository comentarioRepository)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _comentarioRepository = comentarioRepository;
    }

    public async Task<Result> Handle(
        EliminarComentarioCommand request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        var comentario = await _comentarioRepository.ObtenerPorIdAsync(request.ComentarioId, cancellationToken);
        if (comentario is null)
            return Result.NoEncontrado("TicketComentario", request.ComentarioId);

        bool esAutor = comentario.AutorId == actor.Id;
        bool tienePrivilegio = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN;

        if (!esAutor && !tienePrivilegio)
            return Result.NoPermitido("Solo el autor o un administrador puede eliminar este comentario.");

        try
        {
            comentario.EliminarLogicamente(actor.Id);
            await _comentarioRepository.ActualizarAsync(comentario, cancellationToken);

            return Result.Exito();
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
