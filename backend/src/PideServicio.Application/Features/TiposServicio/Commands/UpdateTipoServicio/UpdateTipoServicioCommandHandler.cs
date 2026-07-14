namespace PideServicio.Application.Features.TiposServicio.Commands.UpdateTipoServicio;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class UpdateTipoServicioCommandHandler : ICommandHandler<UpdateTipoServicioCommand, Guid>
{
    private readonly ITipoServicioRepository _tipoServicioRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public UpdateTipoServicioCommandHandler(
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

    public async Task<Result<Guid>> Handle(UpdateTipoServicioCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden editar tipos de servicio.");

        var ts = await _tipoServicioRepo.ObtenerPorIdAsync(request.Id, ct);
        if (ts is null) return Result.NoEncontrado<Guid>("TipoServicio", request.Id);

        if (actorDb.Rol == RolTipo.ADMIN && actorDb.EmpresaId != ts.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para editar este tipo de servicio.");

        if (await _tipoServicioRepo.ExisteNombreAsync(ts.EmpresaId, request.Nombre, request.Id, ct))
            return Result.Fallo<Guid>($"Ya existe un tipo de servicio con el nombre '{request.Nombre}' en esta empresa.");

        if (await _tipoServicioRepo.ExisteOrdenAsync(ts.EmpresaId, request.Orden, request.Id, ct))
            return Result.Fallo<Guid>($"Ya existe un tipo de servicio con el orden '{request.Orden}' en esta empresa.");

        var anterior = new { ts.Nombre, ts.Orden, ts.Descripcion };
        try
        {
            ts.Actualizar(request.Nombre, request.Orden, request.Descripcion, actorDb.Id);
            await _tipoServicioRepo.ActualizarAsync(ts, ct);
            await _auditService.RegistrarAsync("tipos_servicio", ts.Id, "ACTUALIZADO", anterior,
                new { ts.Nombre, ts.Orden, ts.Descripcion }, ct);
            return Result.Exito(ts.Id);
        }
        catch (ValidationException ex) { return Result.ErrorValidacion<Guid>(ex.Errors); }
        catch (DomainException ex)     { return Result.Fallo<Guid>(ex.Message); }
    }
}
