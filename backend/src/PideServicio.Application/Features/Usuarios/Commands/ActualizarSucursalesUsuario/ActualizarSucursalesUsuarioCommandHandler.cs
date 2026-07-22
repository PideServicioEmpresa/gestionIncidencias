namespace PideServicio.Application.Features.Usuarios.Commands.ActualizarSucursalesUsuario;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ActualizarSucursalesUsuarioCommandHandler
    : ICommandHandler<ActualizarSucursalesUsuarioCommand>
{
    private readonly IUsuarioRepository         _usuarioRepository;
    private readonly IUsuarioSucursalRepository _usuarioSucursalRepository;
    private readonly ICurrentUserService        _currentUserService;
    private readonly IAuditService              _auditService;

    public ActualizarSucursalesUsuarioCommandHandler(
        IUsuarioRepository         usuarioRepository,
        IUsuarioSucursalRepository usuarioSucursalRepository,
        ICurrentUserService        currentUserService,
        IAuditService              auditService)
    {
        _usuarioRepository         = usuarioRepository;
        _usuarioSucursalRepository = usuarioSucursalRepository;
        _currentUserService        = currentUserService;
        _auditService              = auditService;
    }

    public async Task<Result> Handle(ActualizarSucursalesUsuarioCommand request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null) return Result.NoAutorizado();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido("Solo administradores pueden modificar las sucursales de un usuario.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.UsuarioId, ct);
        if (usuario is null)
            return Result.NoEncontrado("Usuario", request.UsuarioId);

        if (actorDb.Rol == RolTipo.ADMIN && usuario.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido("Solo puede gestionar usuarios de su empresa.");

        try
        {
            var nuevas = request.Sucursales
                .Select(s => UsuarioSucursal.Asignar(request.UsuarioId, s.SucursalId, s.EsPrincipal, actorDb.Id))
                .ToList()
                .AsReadOnly();

            // ReemplazarAsync elimina, inserta y sincroniza usuarios.sucursal_id en una transacción.
            await _usuarioSucursalRepository.ReemplazarAsync(request.UsuarioId, nuevas, ct);

            await _auditService.RegistrarAsync(
                entidad:             "UsuarioSucursal",
                entidadId:           request.UsuarioId,
                accion:              "ActualizarSucursales",
                antes:               null,
                despues:             new { Sucursales = request.Sucursales.Select(s => new { s.SucursalId, s.EsPrincipal }) },
                cancellationToken:   ct);

            return Result.Exito();
        }
        catch (ValidationException ex) { return Result.ErrorValidacion(ex.Errors); }
        catch (NotFoundException   ex) { return Result.NoEncontrado(ex.Message); }
        catch (DomainException     ex) { return Result.Fallo(ex.Message); }
        catch (Exception           ex) { return Result.Fallo($"Error al actualizar las sucursales: {ex.Message}"); }
    }
}
