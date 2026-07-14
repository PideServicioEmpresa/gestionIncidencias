namespace PideServicio.Application.Features.MotivosCancelacion.Commands.DesactivarMotivoCancelacion;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;

public sealed class DesactivarMotivoCancelacionCommandHandler : ICommandHandler<DesactivarMotivoCancelacionCommand, Guid>
{
    private readonly IMotivoCancelacionRepository _motivoRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public DesactivarMotivoCancelacionCommandHandler(
        IMotivoCancelacionRepository motivoRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _motivoRepo = motivoRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(DesactivarMotivoCancelacionCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden desactivar motivos de cancelación.");

        var motivo = await _motivoRepo.ObtenerPorIdAsync(request.Id, ct);
        if (motivo is null) return Result.NoEncontrado<Guid>("MotivoCancelacion", request.Id);

        if (motivo.EsGlobal && actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo los SuperAdministradores pueden desactivar motivos de cancelación globales.");

        if (!motivo.EsGlobal && actorDb.Rol == RolTipo.ADMIN && motivo.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para modificar este motivo de cancelación.");

        if (!motivo.Activo) return Result.Fallo<Guid>("El motivo de cancelación ya está inactivo.");

        motivo.Desactivar(actorDb.Id);
        await _motivoRepo.ActualizarAsync(motivo, ct);
        await _auditService.RegistrarAsync("motivos_cancelacion", motivo.Id, "DESACTIVADO",
            new { Activo = true }, new { Activo = false }, ct);
        return Result.Exito(motivo.Id);
    }
}
