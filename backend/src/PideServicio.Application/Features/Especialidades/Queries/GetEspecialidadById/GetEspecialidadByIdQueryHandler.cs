namespace PideServicio.Application.Features.Especialidades.Queries.GetEspecialidadById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Especialidades.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetEspecialidadByIdQueryHandler
    : IQueryHandler<GetEspecialidadByIdQuery, EspecialidadDto>
{
    private readonly IEspecialidadRepository _especialidadRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public GetEspecialidadByIdQueryHandler(
        IEspecialidadRepository especialidadRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _especialidadRepo = especialidadRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<EspecialidadDto>> Handle(GetEspecialidadByIdQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<EspecialidadDto>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<EspecialidadDto>();

        var esp = await _especialidadRepo.ObtenerPorIdAsync(request.Id, ct);
        if (esp is null) return Result.NoEncontrado<EspecialidadDto>("Especialidad", request.Id);

        // Las globales son visibles para todos; las de empresa solo para su empresa o SuperAdmin.
        if (!esp.EsGlobal && actorDb.Rol != RolTipo.SUPERADMIN && actorDb.EmpresaId != esp.EmpresaId)
            return Result.NoPermitido<EspecialidadDto>("No tiene acceso a esta especialidad.");

        return Result.Exito(esp.Adapt<EspecialidadDto>());
    }
}
