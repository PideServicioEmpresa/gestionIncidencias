namespace PideServicio.Application.Features.MotivosCancelacion.Queries.ListMotivosCancelacion;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.MotivosCancelacion.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListMotivosCancelacionQueryHandler
    : IQueryHandler<ListMotivosCancelacionQuery, PagedResult<MotivoCancelacionResumenDto>>
{
    private readonly IMotivoCancelacionRepository _motivoRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public ListMotivosCancelacionQueryHandler(
        IMotivoCancelacionRepository motivoRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _motivoRepo = motivoRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<MotivoCancelacionResumenDto>>> Handle(
        ListMotivosCancelacionQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<PagedResult<MotivoCancelacionResumenDto>>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<PagedResult<MotivoCancelacionResumenDto>>();

        Guid? empresaId = actorDb.Rol == RolTipo.SUPERADMIN ? request.EmpresaId : actorDb.EmpresaId;

        var resultado = await _motivoRepo.ListarAsync(
            empresaId, request.Pagina, request.TamanoPagina,
            request.SoloActivos, request.Busqueda, request.SoloGlobales, ct);

        var dto = new PagedResult<MotivoCancelacionResumenDto>
        {
            Items          = resultado.Items.Adapt<List<MotivoCancelacionResumenDto>>().AsReadOnly(),
            Pagina         = resultado.Pagina,
            TamanoPagina   = resultado.TamanoPagina,
            TotalRegistros = resultado.TotalRegistros
        };

        return Result.Exito(dto);
    }
}
