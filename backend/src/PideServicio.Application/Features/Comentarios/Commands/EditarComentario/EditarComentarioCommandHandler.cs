namespace PideServicio.Application.Features.Comentarios.Commands.EditarComentario;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class EditarComentarioCommandHandler
    : ICommandHandler<EditarComentarioCommand, Guid>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketComentarioRepository _comentarioRepository;

    public EditarComentarioCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketComentarioRepository comentarioRepository)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _comentarioRepository = comentarioRepository;
    }

    public async Task<Result<Guid>> Handle(
        EditarComentarioCommand request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado<Guid>();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado<Guid>();

        var comentario = await _comentarioRepository.ObtenerPorIdAsync(request.ComentarioId, cancellationToken);
        if (comentario is null)
            return Result.NoEncontrado<Guid>("TicketComentario", request.ComentarioId);

        bool esAutor = comentario.AutorId == actor.Id;
        bool tienePrivilegio = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN;

        if (!esAutor && !tienePrivilegio)
            return Result.NoPermitido<Guid>("Solo el autor o un administrador puede editar este comentario.");

        try
        {
            comentario.Editar(request.NuevoCuerpo, actor.Id);
            await _comentarioRepository.ActualizarAsync(comentario, cancellationToken);

            return Result.Exito<Guid>(comentario.Id);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
    }
}
