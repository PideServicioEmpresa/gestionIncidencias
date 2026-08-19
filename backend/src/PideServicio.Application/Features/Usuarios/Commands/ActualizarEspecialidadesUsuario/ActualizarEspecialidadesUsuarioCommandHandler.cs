namespace PideServicio.Application.Features.Usuarios.Commands.ActualizarEspecialidadesUsuario;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ActualizarEspecialidadesUsuarioCommandHandler
    : ICommandHandler<ActualizarEspecialidadesUsuarioCommand>
{
    private readonly IUsuarioRepository              _usuarioRepository;
    private readonly IUsuarioEspecialidadRepository  _usuarioEspecialidadRepository;
    private readonly IEspecialidadRepository         _especialidadRepository;
    private readonly ICurrentUserService             _currentUserService;
    private readonly IAuditService                   _auditService;

    public ActualizarEspecialidadesUsuarioCommandHandler(
        IUsuarioRepository             usuarioRepository,
        IUsuarioEspecialidadRepository usuarioEspecialidadRepository,
        IEspecialidadRepository        especialidadRepository,
        ICurrentUserService            currentUserService,
        IAuditService                  auditService)
    {
        _usuarioRepository             = usuarioRepository;
        _usuarioEspecialidadRepository = usuarioEspecialidadRepository;
        _especialidadRepository        = especialidadRepository;
        _currentUserService            = currentUserService;
        _auditService                  = auditService;
    }

    public async Task<Result> Handle(ActualizarEspecialidadesUsuarioCommand request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null) return Result.NoAutorizado();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido("Solo administradores pueden modificar las especialidades de un usuario.");

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.UsuarioId, ct);
        if (usuario is null)
            return Result.NoEncontrado("Usuario", request.UsuarioId);

        // Aislamiento de empresa: el SuperAdmin es el único que cruza empresas.
        if (actorDb.Rol != RolTipo.SUPERADMIN && usuario.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido("No tiene acceso a este usuario.");

        var ids = request.Especialidades ?? [];

        // La lista vacía es válida: deja al usuario sin especialidades.
        if (ids.Distinct().Count() != ids.Count)
            return Result.ErrorValidacion("Especialidades", "Hay especialidades repetidas en la lista.");

        foreach (var especialidadId in ids)
        {
            if (!await _especialidadRepository.ExisteAsync(especialidadId, ct))
                return Result.ErrorValidacion("Especialidades", "Una de las especialidades indicadas no existe.");
        }

        try
        {
            var nuevas = ids
                .Select(id => UsuarioEspecialidad.Asignar(request.UsuarioId, id, actorDb.Id))
                .ToList()
                .AsReadOnly();

            // ReemplazarAsync elimina e inserta en una transacción atómica.
            await _usuarioEspecialidadRepository.ReemplazarAsync(request.UsuarioId, nuevas, ct);

            await _auditService.RegistrarAsync(
                entidad:           "UsuarioEspecialidad",
                entidadId:         request.UsuarioId,
                accion:            "ActualizarEspecialidades",
                antes:             null,
                despues:           new { Especialidades = ids },
                cancellationToken: ct);

            return Result.Exito();
        }
        catch (ValidationException ex) { return Result.ErrorValidacion(ex.Errors); }
        catch (NotFoundException   ex) { return Result.NoEncontrado(ex.Message); }
        catch (DomainException     ex) { return Result.Fallo(ex.Message); }
        catch (Exception           ex) { return Result.Fallo($"Error al actualizar las especialidades: {ex.Message}"); }
    }
}
