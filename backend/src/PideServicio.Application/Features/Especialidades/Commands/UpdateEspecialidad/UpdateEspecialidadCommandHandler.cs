namespace PideServicio.Application.Features.Especialidades.Commands.UpdateEspecialidad;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class UpdateEspecialidadCommandHandler : ICommandHandler<UpdateEspecialidadCommand, Guid>
{
    private readonly IEspecialidadRepository _especialidadRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public UpdateEspecialidadCommandHandler(
        IEspecialidadRepository especialidadRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _especialidadRepo = especialidadRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(UpdateEspecialidadCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden editar especialidades.");

        var esp = await _especialidadRepo.ObtenerPorIdAsync(request.Id, ct);
        if (esp is null) return Result.NoEncontrado<Guid>("Especialidad", request.Id);

        if (esp.EsGlobal && actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo los SuperAdministradores pueden editar especialidades globales.");

        if (!esp.EsGlobal && actorDb.Rol == RolTipo.ADMIN && esp.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para editar esta especialidad.");

        if (await _especialidadRepo.ExisteNombreAsync(esp.EmpresaId, request.Nombre, request.Id, ct))
            return Result.Fallo<Guid>($"Ya existe una especialidad con el nombre '{request.Nombre}' en este alcance.");

        var anterior = new { esp.Nombre, esp.Descripcion };
        try
        {
            esp.Actualizar(request.Nombre, request.Descripcion, actorDb.Id);
            await _especialidadRepo.ActualizarAsync(esp, ct);
            await _auditService.RegistrarAsync("especialidades", esp.Id, "ACTUALIZADO", anterior,
                new { esp.Nombre, esp.Descripcion }, ct);
            return Result.Exito(esp.Id);
        }
        catch (ValidationException ex) { return Result.ErrorValidacion<Guid>(ex.Errors); }
        catch (DomainException ex)     { return Result.Fallo<Guid>(ex.Message); }
    }
}
