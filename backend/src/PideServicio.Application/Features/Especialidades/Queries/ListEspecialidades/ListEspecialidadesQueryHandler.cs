namespace PideServicio.Application.Features.Especialidades.Queries.ListEspecialidades;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Especialidades.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListEspecialidadesQueryHandler
    : IQueryHandler<ListEspecialidadesQuery, PagedResult<EspecialidadResumenDto>>
{
    private readonly IEspecialidadRepository _especialidadRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public ListEspecialidadesQueryHandler(
        IEspecialidadRepository especialidadRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _especialidadRepo = especialidadRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<EspecialidadResumenDto>>> Handle(
        ListEspecialidadesQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<PagedResult<EspecialidadResumenDto>>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<PagedResult<EspecialidadResumenDto>>();

        // El SuperAdmin puede filtrar por cualquier empresa (o ver todas si no envía filtro).
        // El resto solo ve las de su empresa más las globales.
        var empresaId = actorDb.Rol == RolTipo.SUPERADMIN
            ? request.EmpresaId
            : actorDb.EmpresaId;

        var resultado = await _especialidadRepo.ListarAsync(
            empresaId, request.Pagina, request.TamanoPagina,
            request.SoloActivas, request.Busqueda, ct);

        var dto = new PagedResult<EspecialidadResumenDto>
        {
            Items          = resultado.Items.Adapt<List<EspecialidadResumenDto>>().AsReadOnly(),
            Pagina         = resultado.Pagina,
            TamanoPagina   = resultado.TamanoPagina,
            TotalRegistros = resultado.TotalRegistros
        };

        return Result.Exito(dto);
    }
}
