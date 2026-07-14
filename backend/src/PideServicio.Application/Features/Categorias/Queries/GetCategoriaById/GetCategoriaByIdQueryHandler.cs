namespace PideServicio.Application.Features.Categorias.Queries.GetCategoriaById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Categorias.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetCategoriaByIdQueryHandler : IQueryHandler<GetCategoriaByIdQuery, CategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public GetCategoriaByIdQueryHandler(
        ICategoriaRepository categoriaRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _categoriaRepo = categoriaRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<CategoriaDto>> Handle(GetCategoriaByIdQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<CategoriaDto>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<CategoriaDto>();

        var cat = await _categoriaRepo.ObtenerPorIdAsync(request.Id, ct);
        if (cat is null) return Result.NoEncontrado<CategoriaDto>("Categoría", request.Id);

        if (actorDb.Rol != RolTipo.SUPERADMIN && !cat.EsGlobal && cat.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido<CategoriaDto>("No tiene acceso a esta categoría.");

        return Result.Exito(cat.Adapt<CategoriaDto>());
    }
}
