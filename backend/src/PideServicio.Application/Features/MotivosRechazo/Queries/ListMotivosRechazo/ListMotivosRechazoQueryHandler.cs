namespace PideServicio.Application.Features.MotivosRechazo.Queries.ListMotivosRechazo;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.MotivosRechazo.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListMotivosRechazoQueryHandler
    : IQueryHandler<ListMotivosRechazoQuery, PagedResult<MotivoRechazoResumenDto>>
{
    private readonly IMotivoRechazoRepository _motivoRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public ListMotivosRechazoQueryHandler(
        IMotivoRechazoRepository motivoRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _motivoRepo = motivoRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<MotivoRechazoResumenDto>>> Handle(
        ListMotivosRechazoQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<PagedResult<MotivoRechazoResumenDto>>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<PagedResult<MotivoRechazoResumenDto>>();

        Guid? empresaId = actorDb.Rol == RolTipo.SUPERADMIN ? request.EmpresaId : actorDb.EmpresaId;

        var resultado = await _motivoRepo.ListarAsync(
            empresaId, request.Pagina, request.TamanoPagina,
            request.SoloActivos, request.Busqueda, request.SoloGlobales, ct);

        var dto = new PagedResult<MotivoRechazoResumenDto>
        {
            Items          = resultado.Items.Adapt<List<MotivoRechazoResumenDto>>().AsReadOnly(),
            Pagina         = resultado.Pagina,
            TamanoPagina   = resultado.TamanoPagina,
            TotalRegistros = resultado.TotalRegistros
        };

        return Result.Exito(dto);
    }
}
