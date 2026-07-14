namespace PideServicio.Application.Features.MotivosRechazo.Queries.GetMotivoRechazoById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.MotivosRechazo.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetMotivoRechazoByIdQueryHandler : IQueryHandler<GetMotivoRechazoByIdQuery, MotivoRechazoDto>
{
    private readonly IMotivoRechazoRepository _motivoRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public GetMotivoRechazoByIdQueryHandler(
        IMotivoRechazoRepository motivoRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _motivoRepo = motivoRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<MotivoRechazoDto>> Handle(GetMotivoRechazoByIdQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<MotivoRechazoDto>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<MotivoRechazoDto>();

        var motivo = await _motivoRepo.ObtenerPorIdAsync(request.Id, ct);
        if (motivo is null) return Result.NoEncontrado<MotivoRechazoDto>("MotivoRechazo", request.Id);

        if (actorDb.Rol != RolTipo.SUPERADMIN && !motivo.EsGlobal && motivo.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido<MotivoRechazoDto>("No tiene acceso a este motivo de rechazo.");

        return Result.Exito(motivo.Adapt<MotivoRechazoDto>());
    }
}
