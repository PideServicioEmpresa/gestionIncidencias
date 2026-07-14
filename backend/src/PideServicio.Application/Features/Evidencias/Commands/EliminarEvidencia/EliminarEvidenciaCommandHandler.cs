namespace PideServicio.Application.Features.Evidencias.Commands.EliminarEvidencia;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class EliminarEvidenciaCommandHandler
    : ICommandHandler<EliminarEvidenciaCommand>
{
    private const string Bucket = "tickets-evidence";

    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITicketEvidenciaRepository _evidenciaRepository;
    private readonly IStorageService _storageService;

    public EliminarEvidenciaCommandHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository,
        ITicketEvidenciaRepository evidenciaRepository,
        IStorageService storageService)
    {
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
        _evidenciaRepository = evidenciaRepository;
        _storageService = storageService;
    }

    public async Task<Result> Handle(
        EliminarEvidenciaCommand request,
        CancellationToken cancellationToken)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null)
            return Result.NoAutorizado();

        var actor = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, cancellationToken)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
        if (actor is null || !actor.Activo) return Result.NoAutorizado();

        var evidencia = await _evidenciaRepository.ObtenerPorIdAsync(request.EvidenciaId, cancellationToken);
        if (evidencia is null)
            return Result.NoEncontrado("TicketEvidencia", request.EvidenciaId);

        bool esAutor = evidencia.AutorId == actor.Id;
        bool tienePrivilegio = actor.Rol is RolTipo.ADMIN or RolTipo.SUPERADMIN;

        if (!esAutor && !tienePrivilegio)
            return Result.NoPermitido("Solo el autor o un administrador puede eliminar esta evidencia.");

        try
        {
            var rutaAlmacenamiento = evidencia.UrlAlmacenamiento;

            evidencia.EliminarLogicamente(actor.Id);
            await _evidenciaRepository.ActualizarAsync(evidencia, cancellationToken);

            await _storageService.EliminarAsync(Bucket, rutaAlmacenamiento, cancellationToken);

            return Result.Exito();
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
