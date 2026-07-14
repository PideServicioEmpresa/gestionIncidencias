namespace PideServicio.Application.Features.Usuarios.Commands.ActivarUsuario;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ActivarUsuarioCommandHandler : ICommandHandler<ActivarUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public ActivarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(ActivarUsuarioCommand request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null) return Result.NoAutorizado();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido("Solo administradores pueden activar usuarios.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.UsuarioId, ct);
        if (usuario is null)
            return Result.NoEncontrado("Usuario", request.UsuarioId);

        if (actorDb.Rol == RolTipo.ADMIN && usuario.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido("Solo puede activar usuarios de su empresa.");

        if (usuario.Activo)
            return Result.Fallo("El usuario ya se encuentra activo.");

        try
        {
            usuario.Activar(actorDb.Id);

            await _usuarioRepository.ActualizarAsync(usuario, ct);

            await _auditService.RegistrarAsync(
                entidad: "Usuario",
                entidadId: usuario.Id,
                accion: "Activar",
                antes: new { Activo = false, EstadoLaboral = "RETIRADO" },
                despues: new { Activo = true, EstadoLaboral = usuario.EstadoLaboral.ToString() },
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
