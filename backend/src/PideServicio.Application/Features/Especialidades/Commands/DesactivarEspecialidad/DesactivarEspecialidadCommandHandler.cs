namespace PideServicio.Application.Features.Especialidades.Commands.DesactivarEspecialidad;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;

public sealed class DesactivarEspecialidadCommandHandler : ICommandHandler<DesactivarEspecialidadCommand, Guid>
{
    private readonly IEspecialidadRepository _especialidadRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public DesactivarEspecialidadCommandHandler(
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

    public async Task<Result<Guid>> Handle(DesactivarEspecialidadCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden desactivar especialidades.");

        var esp = await _especialidadRepo.ObtenerPorIdAsync(request.Id, ct);
        if (esp is null) return Result.NoEncontrado<Guid>("Especialidad", request.Id);

        if (esp.EsGlobal && actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo los SuperAdministradores pueden desactivar especialidades globales.");

        if (!esp.EsGlobal && actorDb.Rol == RolTipo.ADMIN && esp.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para modificar esta especialidad.");

        if (!esp.Activo) return Result.Fallo<Guid>("La especialidad ya está inactiva.");

        esp.Desactivar(actorDb.Id);
        await _especialidadRepo.ActualizarAsync(esp, ct);
        await _auditService.RegistrarAsync("especialidades", esp.Id, "DESACTIVADO",
            new { Activo = true }, new { Activo = false }, ct);
        return Result.Exito(esp.Id);
    }
}
