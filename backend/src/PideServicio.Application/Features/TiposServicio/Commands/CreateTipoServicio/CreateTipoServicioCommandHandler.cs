namespace PideServicio.Application.Features.TiposServicio.Commands.CreateTipoServicio;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CreateTipoServicioCommandHandler : ICommandHandler<CreateTipoServicioCommand, Guid>
{
    private readonly ITipoServicioRepository _tipoServicioRepo;
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public CreateTipoServicioCommandHandler(
        ITipoServicioRepository tipoServicioRepo,
        IEmpresaRepository empresaRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _tipoServicioRepo = tipoServicioRepo;
        _empresaRepo = empresaRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(CreateTipoServicioCommand request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo Administradores o SuperAdministradores pueden crear tipos de servicio.");

        if (actorDb.Rol == RolTipo.ADMIN && actorDb.EmpresaId != request.EmpresaId)
            return Result.NoPermitido<Guid>("No tiene permisos para crear tipos de servicio en esta empresa.");

        var empresa = await _empresaRepo.ObtenerPorIdAsync(request.EmpresaId, ct);
        if (empresa is null)
            return Result.NoEncontrado<Guid>("Empresa", request.EmpresaId);

        if (await _tipoServicioRepo.ExisteNombreAsync(request.EmpresaId, request.Nombre, null, ct))
            return Result.Fallo<Guid>($"Ya existe un tipo de servicio con el nombre '{request.Nombre}' en esta empresa.");

        if (await _tipoServicioRepo.ExisteOrdenAsync(request.EmpresaId, request.Orden, null, ct))
            return Result.Fallo<Guid>($"Ya existe un tipo de servicio con el orden '{request.Orden}' en esta empresa.");

        try
        {
            var ts = TipoServicio.Crear(request.EmpresaId, request.Nombre, request.Orden, request.Descripcion, actorDb.Id);
            var id = await _tipoServicioRepo.CrearAsync(ts, ct);
            await _auditService.RegistrarAsync("tipos_servicio", id, "CREADO", null,
                new { ts.EmpresaId, ts.Nombre, ts.Orden }, ct);
            return Result.Exito(id);
        }
        catch (ValidationException ex) { return Result.ErrorValidacion<Guid>(ex.Errors); }
        catch (DomainException ex)     { return Result.Fallo<Guid>(ex.Message); }
    }
}
