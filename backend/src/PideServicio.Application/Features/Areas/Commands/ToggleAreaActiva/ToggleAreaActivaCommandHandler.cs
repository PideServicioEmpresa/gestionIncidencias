namespace PideServicio.Application.Features.Areas.Commands.ToggleAreaActiva;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ToggleAreaActivaCommandHandler : ICommandHandler<ToggleAreaActivaCommand, Guid>
{
    private readonly IAreaRepository _areaRepo;
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public ToggleAreaActivaCommandHandler(
        IAreaRepository areaRepo,
        ISucursalRepository sucursalRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _areaRepo = areaRepo;
        _sucursalRepo = sucursalRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(ToggleAreaActivaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden activar o desactivar áreas.");

        var area = await _areaRepo.ObtenerPorIdAsync(request.Id, ct);
        if (area is null)
            return Result.NoEncontrado<Guid>("Área", request.Id);

        if (actorDb.Rol == RolTipo.ADMIN)
        {
            var sucursal = await _sucursalRepo.ObtenerPorIdAsync(area.SucursalId, ct);
            if (sucursal is null || actorDb.EmpresaId != sucursal.EmpresaId)
                return Result.NoPermitido<Guid>("No tiene permisos para modificar esta área.");
        }

        var estadoAnterior = area.Activa;

        try
        {
            if (area.Activa)
                area.Desactivar(actorDb.Id);
            else
                area.Activar(actorDb.Id);

            await _areaRepo.ActualizarAsync(area, ct);
            await _auditService.RegistrarAsync(
                "areas",
                area.Id,
                area.Activa ? "ACTIVADO" : "DESACTIVADO",
                new { Activa = estadoAnterior },
                new { area.Activa },
                ct);

            return Result.Exito(area.Id);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
    }
}
