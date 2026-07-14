namespace PideServicio.Application.Features.Usuarios.Queries.GetUsuarioById;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetUsuarioByIdQueryHandler : IQueryHandler<GetUsuarioByIdQuery, UsuarioDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUsuarioByIdQueryHandler(
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService)
    {
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UsuarioDto>> Handle(GetUsuarioByIdQuery request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<UsuarioDto>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<UsuarioDto>();

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.Id, ct);
        if (usuario is null)
            return Result.NoEncontrado<UsuarioDto>("Usuario", request.Id);

        var puedeVer = actorDb.Rol == RolTipo.SUPERADMIN
            || (actorDb.Rol is (RolTipo.ADMIN or RolTipo.SUPERVISOR) && usuario.EmpresaId == actorDb.EmpresaId)
            || usuario.Id == actorDb.Id;

        if (!puedeVer)
            return Result.NoPermitido<UsuarioDto>("No tiene permisos para ver este usuario.");

        var dto = usuario.Adapt<UsuarioDto>();
        return Result.Exito<UsuarioDto>(dto);
    }
}
