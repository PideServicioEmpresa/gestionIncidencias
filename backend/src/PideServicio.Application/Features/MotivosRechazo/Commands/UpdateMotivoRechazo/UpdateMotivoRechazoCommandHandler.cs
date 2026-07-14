namespace PideServicio.Application.Features.MotivosRechazo.Commands.UpdateMotivoRechazo;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class UpdateMotivoRechazoCommandHandler : ICommandHandler<UpdateMotivoRechazoCommand, Guid>
{
    private readonly IMotivoRechazoRepository _motivoRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public UpdateMotivoRechazoCommandHandler(
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

    public async Task<Result<Guid>> Handle(UpdateMotivoRechazoCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden editar motivos de rechazo.");

        var motivo = await _motivoRepo.ObtenerPorIdAsync(request.Id, ct);
        if (motivo is null) return Result.NoEncontrado<Guid>("MotivoRechazo", request.Id);

        if (motivo.EsGlobal && actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo los SuperAdministradores pueden editar motivos de rechazo globales.");

        if (!motivo.EsGlobal && actorDb.Rol == RolTipo.ADMIN && motivo.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para editar este motivo de rechazo.");

        if (await _motivoRepo.ExisteCodigoAsync(motivo.EmpresaId, request.Codigo, request.Id, ct))
            return Result.Fallo<Guid>($"Ya existe un motivo con el código '{request.Codigo.ToUpperInvariant()}' en este alcance.");

        if (await _motivoRepo.ExisteNombreAsync(motivo.EmpresaId, request.Nombre, request.Id, ct))
            return Result.Fallo<Guid>($"Ya existe un motivo con el nombre '{request.Nombre}' en este alcance.");

        var anterior = new { motivo.Nombre, motivo.Codigo, motivo.Orden, motivo.Descripcion };
        try
        {
            motivo.Actualizar(request.Nombre, request.Codigo, request.Orden, request.Descripcion, actorDb.Id);
            await _motivoRepo.ActualizarAsync(motivo, ct);
            await _auditService.RegistrarAsync("motivos_rechazo", motivo.Id, "ACTUALIZADO", anterior,
                new { motivo.Nombre, motivo.Codigo, motivo.Orden, motivo.Descripcion }, ct);
            return Result.Exito(motivo.Id);
        }
        catch (ValidationException ex) { return Result.ErrorValidacion<Guid>(ex.Errors); }
        catch (DomainException ex)     { return Result.Fallo<Guid>(ex.Message); }
    }
}
