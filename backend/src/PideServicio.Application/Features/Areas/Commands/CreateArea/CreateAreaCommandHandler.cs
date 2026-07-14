namespace PideServicio.Application.Features.Areas.Commands.CreateArea;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CreateAreaCommandHandler : ICommandHandler<CreateAreaCommand, Guid>
{
    private readonly IAreaRepository _areaRepo;
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public CreateAreaCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateAreaCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden crear áreas.");

        var sucursal = await _sucursalRepo.ObtenerPorIdAsync(request.SucursalId, ct);
        if (sucursal is null)
            return Result.NoEncontrado<Guid>("Sucursal", request.SucursalId);

        if (actorDb.Rol == RolTipo.ADMIN && actorDb.EmpresaId != sucursal.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para crear áreas en esta sucursal.");

        if (!sucursal.Activa)
            return Result.Fallo<Guid>("No se pueden crear áreas en una sucursal inactiva.");

        if (await _areaRepo.ExisteNombreAsync(request.SucursalId, request.Nombre, null, ct))
            return Result.Fallo<Guid>($"Ya existe un área con el nombre '{request.Nombre}' en esta sucursal.");

        try
        {
            var area = Area.Crear(
                request.SucursalId,
                request.Nombre,
                request.Descripcion,
                request.ResponsableId,
                actorDb.Id);

            var id = await _areaRepo.CrearAsync(area, ct);
            await _auditService.RegistrarAsync("areas", id, "CREADO", null, new
            {
                area.SucursalId,
                area.Nombre,
                area.Descripcion
            }, ct);
            return Result.Exito(id);
        }
        catch (ValidationException ex)
        {
            return Result.ErrorValidacion<Guid>(ex.Errors);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
    }
}
