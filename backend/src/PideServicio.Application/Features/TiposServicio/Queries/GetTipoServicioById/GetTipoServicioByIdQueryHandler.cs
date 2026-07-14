namespace PideServicio.Application.Features.TiposServicio.Queries.GetTipoServicioById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.TiposServicio.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetTipoServicioByIdQueryHandler
    : IQueryHandler<GetTipoServicioByIdQuery, TipoServicioDto>
{
    private readonly ITipoServicioRepository _tipoServicioRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public GetTipoServicioByIdQueryHandler(
        ITipoServicioRepository tipoServicioRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _tipoServicioRepo = tipoServicioRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<TipoServicioDto>> Handle(GetTipoServicioByIdQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<TipoServicioDto>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<TipoServicioDto>();

        var ts = await _tipoServicioRepo.ObtenerPorIdAsync(request.Id, ct);
        if (ts is null) return Result.NoEncontrado<TipoServicioDto>("TipoServicio", request.Id);

        if (actorDb.Rol != RolTipo.SUPERADMIN && actorDb.EmpresaId != ts.EmpresaId)
            return Result.NoPermitido<TipoServicioDto>("No tiene acceso a este tipo de servicio.");

        return Result.Exito(ts.Adapt<TipoServicioDto>());
    }
}
