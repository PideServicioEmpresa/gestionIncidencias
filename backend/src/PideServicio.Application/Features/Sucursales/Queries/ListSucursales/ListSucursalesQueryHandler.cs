namespace PideServicio.Application.Features.Sucursales.Queries.ListSucursales;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Sucursales.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListSucursalesQueryHandler : IQueryHandler<ListSucursalesQuery, PagedResult<SucursalResumenDto>>
{
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public ListSucursalesQueryHandler(
        ISucursalRepository sucursalRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _sucursalRepo = sucursalRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<SucursalResumenDto>>> Handle(ListSucursalesQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<PagedResult<SucursalResumenDto>>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<PagedResult<SucursalResumenDto>>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<PagedResult<SucursalResumenDto>>("No tiene permisos para listar sucursales.");

        // ADMIN solo ve sucursales de su empresa; SuperAdmin puede filtrar por cualquier empresa
        var empresaIdFiltro = actorDb.Rol == RolTipo.SUPERADMIN
            ? request.EmpresaId
            : actorDb.EmpresaId;

        var resultado = await _sucursalRepo.ListarAsync(
            empresaIdFiltro,
            request.Pagina,
            request.TamanoPagina,
            request.SoloActivas,
            request.Busqueda,
            ct);
        var dtos = resultado.Items.Adapt<List<SucursalResumenDto>>();

        var paged = new PagedResult<SucursalResumenDto>
        {
            Items = dtos,
            Pagina = resultado.Pagina,
            TamanoPagina = resultado.TamanoPagina,
            TotalRegistros = resultado.TotalRegistros
        };

        return Result.Exito(paged);
    }
}
