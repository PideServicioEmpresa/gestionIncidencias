namespace PideServicio.Application.Features.Empresas.Queries.GetEmpresaById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Empresas.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetEmpresaByIdQueryHandler : IQueryHandler<GetEmpresaByIdQuery, EmpresaDto>
{
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUser;

    public GetEmpresaByIdQueryHandler(
        IEmpresaRepository empresaRepo,
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUser)
    {
        _empresaRepo = empresaRepo;
        _usuarioRepository = usuarioRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<EmpresaDto>> Handle(GetEmpresaByIdQuery request, CancellationToken ct)
    {
        var claims = _currentUser.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<EmpresaDto>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<EmpresaDto>();

        var empresa = await _empresaRepo.ObtenerPorIdAsync(request.Id, ct);
        if (empresa is null)
            return Result.NoEncontrado<EmpresaDto>("Empresa", request.Id);

        if (actorDb.Rol != RolTipo.SUPERADMIN && actorDb.EmpresaId != empresa.Id)
            return Result.NoPermitido<EmpresaDto>("No tiene acceso a esta empresa.");

        var dto = empresa.Adapt<EmpresaDto>();
        return Result.Exito(dto);
    }
}
