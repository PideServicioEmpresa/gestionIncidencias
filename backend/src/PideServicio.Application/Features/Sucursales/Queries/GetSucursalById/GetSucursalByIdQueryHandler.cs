namespace PideServicio.Application.Features.Sucursales.Queries.GetSucursalById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Sucursales.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetSucursalByIdQueryHandler : IQueryHandler<GetSucursalByIdQuery, SucursalDto>
{
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public GetSucursalByIdQueryHandler(
        ISucursalRepository sucursalRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _sucursalRepo = sucursalRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<SucursalDto>> Handle(GetSucursalByIdQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<SucursalDto>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<SucursalDto>();

        var sucursal = await _sucursalRepo.ObtenerPorIdAsync(request.Id, ct);
        if (sucursal is null)
            return Result.NoEncontrado<SucursalDto>("Sucursal", request.Id);

        if (actorDb.Rol != RolTipo.SUPERADMIN && actorDb.EmpresaId != sucursal.EmpresaId)
            return Result.NoPermitido<SucursalDto>("No tiene acceso a esta sucursal.");

        var dto = sucursal.Adapt<SucursalDto>();
        return Result.Exito(dto);
    }
}
