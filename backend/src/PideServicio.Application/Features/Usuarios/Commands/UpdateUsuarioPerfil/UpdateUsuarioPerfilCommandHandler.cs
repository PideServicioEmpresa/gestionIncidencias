namespace PideServicio.Application.Features.Usuarios.Commands.UpdateUsuarioPerfil;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class UpdateUsuarioPerfilCommandHandler : ICommandHandler<UpdateUsuarioPerfilCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public UpdateUsuarioPerfilCommandHandler(
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(UpdateUsuarioPerfilCommand request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null) return Result.NoAutorizado();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado();

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.UsuarioId, ct);
        if (usuario is null)
            return Result.NoEncontrado("Usuario", request.UsuarioId);

        var esAdminOSuperAdmin = actorDb.Rol is (RolTipo.ADMIN or RolTipo.SUPERADMIN);

        if (!esAdminOSuperAdmin)
        {
            if (usuario.Id != actorDb.Id)
                return Result.NoPermitido("Solo puede editar su propio perfil.");
        }
        else if (actorDb.Rol == RolTipo.ADMIN)
        {
            if (usuario.EmpresaId != actorDb.EmpresaId)
                return Result.NoPermitido("Solo puede editar usuarios de su empresa.");
        }

        try
        {
            var antes = new
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Telefono = usuario.Telefono,
                AreaId = usuario.AreaId,
                FotoUrl = usuario.FotoUrl
            };

            var areaIdFinal = esAdminOSuperAdmin ? request.AreaId : usuario.AreaId;

            usuario.ActualizarPerfil(
                nombre: request.Nombre,
                apellido: request.Apellido,
                telefono: request.Telefono,
                areaId: areaIdFinal,
                actorId: actorDb.Id);

            if (request.ActualizarFoto)
                usuario.ActualizarFoto(request.FotoUrl, actorDb.Id);

            await _usuarioRepository.ActualizarAsync(usuario, ct);

            await _auditService.RegistrarAsync(
                entidad: "Usuario",
                entidadId: usuario.Id,
                accion: "ActualizarPerfil",
                antes: antes,
                despues: new
                {
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Telefono = usuario.Telefono,
                    AreaId = usuario.AreaId,
                    FotoUrl = usuario.FotoUrl
                },
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
