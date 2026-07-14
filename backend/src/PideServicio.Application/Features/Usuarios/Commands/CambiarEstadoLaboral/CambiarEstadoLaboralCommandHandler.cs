namespace PideServicio.Application.Features.Usuarios.Commands.CambiarEstadoLaboral;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CambiarEstadoLaboralCommandHandler : ICommandHandler<CambiarEstadoLaboralCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public CambiarEstadoLaboralCommandHandler(
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(CambiarEstadoLaboralCommand request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null) return Result.NoAutorizado();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido("Solo administradores pueden cambiar el estado laboral de usuarios.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.UsuarioId, ct);
        if (usuario is null)
            return Result.NoEncontrado("Usuario", request.UsuarioId);

        if (actorDb.Rol == RolTipo.ADMIN && usuario.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido("Solo puede modificar usuarios de su empresa.");

        try
        {
            var estadoAnterior = usuario.EstadoLaboral.ToString();

            usuario.CambiarEstadoLaboral(request.NuevoEstado, actorDb.Id);

            await _usuarioRepository.ActualizarAsync(usuario, ct);

            await _auditService.RegistrarAsync(
                entidad: "Usuario",
                entidadId: usuario.Id,
                accion: "CambiarEstadoLaboral",
                antes: new { EstadoLaboral = estadoAnterior },
                despues: new { EstadoLaboral = usuario.EstadoLaboral.ToString() },
                cancellationToken: ct);

            return Result.Exito();
        }
        catch (ValidationException ex)
        {
            return Result.ErrorValidacion(ex.Errors);
        }
        catch (NotFoundException ex)
        {
            return Result.NoEncontrado(ex.Message);
        }
        catch (ForbiddenException ex)
        {
            return Result.NoPermitido(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.Fallo(ex.Message);
        }
    }
}
