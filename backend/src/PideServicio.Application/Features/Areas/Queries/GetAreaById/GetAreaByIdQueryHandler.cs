namespace PideServicio.Application.Features.Areas.Queries.GetAreaById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Areas.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetAreaByIdQueryHandler : IQueryHandler<GetAreaByIdQuery, AreaDto>
{
    private readonly IAreaRepository _areaRepo;
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public GetAreaByIdQueryHandler(
        IAreaRepository areaRepo,
        ISucursalRepository sucursalRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _areaRepo = areaRepo;
        _sucursalRepo = sucursalRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<AreaDto>> Handle(GetAreaByIdQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<AreaDto>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<AreaDto>();

        var area = await _areaRepo.ObtenerPorIdAsync(request.Id, ct);
        if (area is null)
            return Result.NoEncontrado<AreaDto>("Área", request.Id);

        if (actorDb.Rol != RolTipo.SUPERADMIN)
        {
            var sucursal = await _sucursalRepo.ObtenerPorIdAsync(area.SucursalId, ct);
            if (sucursal is null || actorDb.EmpresaId != sucursal.EmpresaId)
                return Result.NoPermitido<AreaDto>("No tiene acceso a esta área.");
        }

        return Result.Exito(area.Adapt<AreaDto>());
    }
}
