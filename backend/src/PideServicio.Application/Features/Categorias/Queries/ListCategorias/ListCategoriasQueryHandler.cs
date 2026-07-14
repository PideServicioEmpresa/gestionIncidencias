namespace PideServicio.Application.Features.Categorias.Queries.ListCategorias;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Categorias.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListCategoriasQueryHandler
    : IQueryHandler<ListCategoriasQuery, PagedResult<CategoriaResumenDto>>
{
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public ListCategoriasQueryHandler(
        ICategoriaRepository categoriaRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _categoriaRepo = categoriaRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<CategoriaResumenDto>>> Handle(ListCategoriasQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<PagedResult<CategoriaResumenDto>>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<PagedResult<CategoriaResumenDto>>();

        Guid? empresaId;
        bool soloGlobales = request.SoloGlobales;

        if (actorDb.Rol == RolTipo.SUPERADMIN)
            empresaId = request.EmpresaId;
        else
            empresaId = actorDb.EmpresaId;

        var resultado = await _categoriaRepo.ListarAsync(
            empresaId, request.Pagina, request.TamanoPagina,
            request.SoloActivas, request.Busqueda, soloGlobales, ct);

        var dto = new PagedResult<CategoriaResumenDto>
        {
            Items          = resultado.Items.Adapt<List<CategoriaResumenDto>>().AsReadOnly(),
            Pagina         = resultado.Pagina,
            TamanoPagina   = resultado.TamanoPagina,
            TotalRegistros = resultado.TotalRegistros
        };

        return Result.Exito(dto);
    }
}
