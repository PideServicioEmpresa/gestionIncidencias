namespace PideServicio.Application.Features.TiposServicio.Commands.DesactivarTipoServicio;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;

public sealed class DesactivarTipoServicioCommandHandler : ICommandHandler<DesactivarTipoServicioCommand, Guid>
{
    private readonly ITipoServicioRepository _tipoServicioRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public DesactivarTipoServicioCommandHandler(
        ITipoServicioRepository tipoServicioRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _tipoServicioRepo = tipoServicioRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(DesactivarTipoServicioCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden desactivar tipos de servicio.");

        var ts = await _tipoServicioRepo.ObtenerPorIdAsync(request.Id, ct);
        if (ts is null) return Result.NoEncontrado<Guid>("TipoServicio", request.Id);

        if (actorDb.Rol == RolTipo.ADMIN && actorDb.EmpresaId != ts.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para modificar este tipo de servicio.");

        if (!ts.Activo) return Result.Fallo<Guid>("El tipo de servicio ya está inactivo.");

        ts.Desactivar(actorDb.Id);
        await _tipoServicioRepo.ActualizarAsync(ts, ct);
        await _auditService.RegistrarAsync("tipos_servicio", ts.Id, "DESACTIVADO",
            new { Activo = true }, new { Activo = false }, ct);
        return Result.Exito(ts.Id);
    }
}
