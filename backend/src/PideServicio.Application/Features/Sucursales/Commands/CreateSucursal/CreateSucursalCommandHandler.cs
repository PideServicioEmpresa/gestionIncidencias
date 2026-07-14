namespace PideServicio.Application.Features.Sucursales.Commands.CreateSucursal;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CreateSucursalCommandHandler : ICommandHandler<CreateSucursalCommand, Guid>
{
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public CreateSucursalCommandHandler(
        ISucursalRepository sucursalRepo,
        IEmpresaRepository empresaRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _sucursalRepo = sucursalRepo;
        _empresaRepo = empresaRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(CreateSucursalCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden crear sucursales.");

        if (actorDb.Rol == RolTipo.ADMIN && actorDb.EmpresaId != request.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para crear sucursales en esta empresa.");

        var empresa = await _empresaRepo.ObtenerPorIdAsync(request.EmpresaId, ct);
        if (empresa is null)
            return Result.NoEncontrado<Guid>("Empresa", request.EmpresaId);
        if (!empresa.Activa)
            return Result.Fallo<Guid>("No se pueden crear sucursales en una empresa inactiva.");

        if (await _sucursalRepo.ExisteNombreAsync(request.EmpresaId, request.Nombre, null, ct))
            return Result.Fallo<Guid>($"Ya existe una sucursal con el nombre '{request.Nombre}' en esta empresa.");

        try
        {
            var sucursal = Sucursal.Crear(
                request.EmpresaId,
                request.Nombre,
                request.Descripcion,
                request.Direccion,
                request.ResponsableId,
                actorDb.Id);

            var id = await _sucursalRepo.CrearAsync(sucursal, ct);
            await _auditService.RegistrarAsync("sucursales", id, "CREADO", null, new
            {
                sucursal.EmpresaId,
                sucursal.Nombre,
                sucursal.Descripcion,
                sucursal.Direccion
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
