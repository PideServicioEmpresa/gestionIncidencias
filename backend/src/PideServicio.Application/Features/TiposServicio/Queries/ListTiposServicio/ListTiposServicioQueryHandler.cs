namespace PideServicio.Application.Features.TiposServicio.Queries.ListTiposServicio;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.TiposServicio.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListTiposServicioQueryHandler
    : IQueryHandler<ListTiposServicioQuery, PagedResult<TipoServicioResumenDto>>
{
    private readonly ITipoServicioRepository _tipoServicioRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public ListTiposServicioQueryHandler(
        ITipoServicioRepository tipoServicioRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _tipoServicioRepo = tipoServicioRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<TipoServicioResumenDto>>> Handle(
        ListTiposServicioQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<PagedResult<TipoServicioResumenDto>>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<PagedResult<TipoServicioResumenDto>>();

        var empresaId = actorDb.Rol == RolTipo.SUPERADMIN
            ? request.EmpresaId
            : actorDb.EmpresaId;

        if (empresaId is null || empresaId == Guid.Empty)
            return Result.ErrorValidacion<PagedResult<TipoServicioResumenDto>>(
                "EmpresaId", "Debe especificar una empresa para listar los tipos de servicio.");

        var resultado = await _tipoServicioRepo.ListarAsync(
            empresaId.Value, request.Pagina, request.TamanoPagina,
            request.SoloActivos, request.Busqueda, ct);

        var dto = new PagedResult<TipoServicioResumenDto>
        {
            Items          = resultado.Items.Adapt<List<TipoServicioResumenDto>>().AsReadOnly(),
            Pagina         = resultado.Pagina,
            TamanoPagina   = resultado.TamanoPagina,
            TotalRegistros = resultado.TotalRegistros
        };

        return Result.Exito(dto);
    }
}
