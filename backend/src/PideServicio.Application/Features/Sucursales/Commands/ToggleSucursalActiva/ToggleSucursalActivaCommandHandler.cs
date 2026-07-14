namespace PideServicio.Application.Features.Sucursales.Commands.ToggleSucursalActiva;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class ToggleSucursalActivaCommandHandler : ICommandHandler<ToggleSucursalActivaCommand, Guid>
{
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public ToggleSucursalActivaCommandHandler(
        ISucursalRepository sucursalRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _sucursalRepo = sucursalRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(ToggleSucursalActivaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden activar o desactivar sucursales.");

        var sucursal = await _sucursalRepo.ObtenerPorIdAsync(request.Id, ct);
        if (sucursal is null)
            return Result.NoEncontrado<Guid>("Sucursal", request.Id);

        if (actorDb.Rol == RolTipo.ADMIN && actorDb.EmpresaId != sucursal.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para modificar esta sucursal.");

        var estadoAnterior = sucursal.Activa;

        if (sucursal.Activa)
        {
            var activas = await _sucursalRepo.ContarActivasPorEmpresaAsync(sucursal.EmpresaId, ct);
            if (activas <= 1)
                return Result.Fallo<Guid>("No se puede desactivar la última sucursal activa de la empresa.");
        }

        try
        {
            if (sucursal.Activa)
                sucursal.Desactivar(actorDb.Id);
            else
                sucursal.Activar(actorDb.Id);

            await _sucursalRepo.ActualizarAsync(sucursal, ct);
            await _auditService.RegistrarAsync(
                "sucursales", sucursal.Id,
                sucursal.Activa ? "ACTIVADO" : "DESACTIVADO",
                new { Activa = estadoAnterior },
                new { sucursal.Activa },
                ct);

            return Result.Exito(sucursal.Id);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
    }
}
