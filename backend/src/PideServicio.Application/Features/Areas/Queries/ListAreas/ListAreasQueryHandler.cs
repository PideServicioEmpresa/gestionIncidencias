namespace PideServicio.Application.Features.Areas.Queries.ListAreas;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Areas.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListAreasQueryHandler : IQueryHandler<ListAreasQuery, PagedResult<AreaResumenDto>>
{
    private readonly IAreaRepository _areaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public ListAreasQueryHandler(
        IAreaRepository areaRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _areaRepo = areaRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<AreaResumenDto>>> Handle(ListAreasQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<PagedResult<AreaResumenDto>>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<PagedResult<AreaResumenDto>>();

        Guid? empresaIdFiltro;
        if (actorDb.Rol == RolTipo.SUPERADMIN)
        {
            empresaIdFiltro = request.EmpresaId;
        }
        else if (actorDb.Rol is RolTipo.ADMIN or RolTipo.SUPERVISOR or RolTipo.TECNICO or RolTipo.TRABAJADOR)
        {
            empresaIdFiltro = actorDb.EmpresaId;
        }
        else
        {
            return Result.NoPermitido<PagedResult<AreaResumenDto>>("No tiene permisos para listar áreas.");
        }

        var resultado = await _areaRepo.ListarAsync(
            request.SucursalId,
            empresaIdFiltro,
            request.Pagina,
            request.TamanoPagina,
            request.SoloActivas,
            request.Busqueda,
            ct);

        var dtos = resultado.Items.Adapt<List<AreaResumenDto>>();

        var paged = new PagedResult<AreaResumenDto>
        {
            Items = dtos,
            Pagina = resultado.Pagina,
            TamanoPagina = resultado.TamanoPagina,
            TotalRegistros = resultado.TotalRegistros
        };

        return Result.Exito(paged);
    }
}
