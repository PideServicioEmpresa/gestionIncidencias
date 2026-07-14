namespace PideServicio.Application.Features.MotivosCancelacion.Queries.GetMotivoCancelacionById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.MotivosCancelacion.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetMotivoCancelacionByIdQueryHandler
    : IQueryHandler<GetMotivoCancelacionByIdQuery, MotivoCancelacionDto>
{
    private readonly IMotivoCancelacionRepository _motivoRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public GetMotivoCancelacionByIdQueryHandler(
        IMotivoCancelacionRepository motivoRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _motivoRepo = motivoRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<MotivoCancelacionDto>> Handle(GetMotivoCancelacionByIdQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<MotivoCancelacionDto>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<MotivoCancelacionDto>();

        var motivo = await _motivoRepo.ObtenerPorIdAsync(request.Id, ct);
        if (motivo is null) return Result.NoEncontrado<MotivoCancelacionDto>("MotivoCancelacion", request.Id);

        if (actorDb.Rol != RolTipo.SUPERADMIN && !motivo.EsGlobal && motivo.EmpresaId != actorDb.EmpresaId)
            return Result.NoPermitido<MotivoCancelacionDto>("No tiene acceso a este motivo de cancelación.");

        return Result.Exito(motivo.Adapt<MotivoCancelacionDto>());
    }
}
