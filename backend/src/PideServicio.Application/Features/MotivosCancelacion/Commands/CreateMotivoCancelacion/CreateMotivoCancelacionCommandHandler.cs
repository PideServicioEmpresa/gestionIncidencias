namespace PideServicio.Application.Features.MotivosCancelacion.Commands.CreateMotivoCancelacion;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CreateMotivoCancelacionCommandHandler : ICommandHandler<CreateMotivoCancelacionCommand, Guid>
{
    private readonly IMotivoCancelacionRepository _motivoRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public CreateMotivoCancelacionCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateMotivoCancelacionCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden crear motivos de cancelación.");

        Guid? empresaId = actorDb.Rol == RolTipo.SUPERADMIN ? request.EmpresaId : actorDb.EmpresaId;

        if (await _motivoRepo.ExisteTextoAsync(empresaId, request.Texto, null, ct))
            return Result.Fallo<Guid>($"Ya existe un motivo de cancelación con ese texto en este alcance.");

        try
        {
            var motivo = MotivoCancelacion.Crear(request.Texto, empresaId, actorDb.Id);
            var id = await _motivoRepo.CrearAsync(motivo, ct);
            await _auditService.RegistrarAsync("motivos_cancelacion", id, "CREADO", null,
                new { motivo.Texto, motivo.EmpresaId }, ct);
            return Result.Exito(id);
        }
        catch (ValidationException ex) { return Result.ErrorValidacion<Guid>(ex.Errors); }
        catch (DomainException ex)     { return Result.Fallo<Guid>(ex.Message); }
    }
}
