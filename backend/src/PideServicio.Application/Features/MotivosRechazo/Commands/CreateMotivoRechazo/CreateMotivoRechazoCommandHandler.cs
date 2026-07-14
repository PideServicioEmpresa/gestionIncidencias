namespace PideServicio.Application.Features.MotivosRechazo.Commands.CreateMotivoRechazo;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CreateMotivoRechazoCommandHandler : ICommandHandler<CreateMotivoRechazoCommand, Guid>
{
    private readonly IMotivoRechazoRepository _motivoRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public CreateMotivoRechazoCommandHandler(
        IMotivoRechazoRepository motivoRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _motivoRepo = motivoRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(CreateMotivoRechazoCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden crear motivos de rechazo.");

        Guid? empresaId = actorDb.Rol == RolTipo.SUPERADMIN ? request.EmpresaId : actorDb.EmpresaId;

        if (await _motivoRepo.ExisteCodigoAsync(empresaId, request.Codigo, null, ct))
            return Result.Fallo<Guid>($"Ya existe un motivo de rechazo con el código '{request.Codigo.ToUpperInvariant()}' en este alcance.");

        if (await _motivoRepo.ExisteNombreAsync(empresaId, request.Nombre, null, ct))
            return Result.Fallo<Guid>($"Ya existe un motivo de rechazo con el nombre '{request.Nombre}' en este alcance.");

        try
        {
            var motivo = MotivoRechazo.Crear(request.Codigo, request.Nombre, request.Orden,
                request.EsOtro, empresaId, request.Descripcion, actorDb.Id);
            var id = await _motivoRepo.CrearAsync(motivo, ct);
            await _auditService.RegistrarAsync("motivos_rechazo", id, "CREADO", null,
                new { motivo.Codigo, motivo.Nombre, motivo.EsOtro, motivo.EmpresaId }, ct);
            return Result.Exito(id);
        }
        catch (ValidationException ex) { return Result.ErrorValidacion<Guid>(ex.Errors); }
        catch (DomainException ex)     { return Result.Fallo<Guid>(ex.Message); }
    }
}
