namespace PideServicio.Application.Features.Usuarios.Queries.ListUsuarios;

using Mapster;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListUsuariosQueryHandler : IQueryHandler<ListUsuariosQuery, PagedResult<UsuarioResumenDto>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;

    public ListUsuariosQueryHandler(
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService)
    {
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<UsuarioResumenDto>>> Handle(ListUsuariosQuery request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<PagedResult<UsuarioResumenDto>>();

        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);
        if (actorDb is null || !actorDb.Activo) return Result.NoAutorizado<PagedResult<UsuarioResumenDto>>();

        Guid empresaId;
        if (request.EmpresaId.HasValue && request.EmpresaId.Value != actorDb.EmpresaId)
        {
            if (actorDb.Rol != RolTipo.SUPERADMIN)
                return Result.NoPermitido<PagedResult<UsuarioResumenDto>>(
                    "No tiene permisos para listar usuarios de otra empresa.");

            empresaId = request.EmpresaId.Value;
        }
        else
        {
            empresaId = actorDb.EmpresaId;
        }

        // Validar paginación
        var pagina = request.Pagina < 1 ? 1 : request.Pagina;
        var tamanoPagina = request.TamanoPagina < 1 ? 20 : Math.Min(request.TamanoPagina, 100);

        var paginado = await _usuarioRepository.ListarAsync(
            empresaId: empresaId,
            sucursalId: request.SucursalId,
            rol: request.Rol,
            soloActivos: request.SoloActivos,
            busqueda: request.Busqueda,
            pagina: pagina,
            tamanoPagina: tamanoPagina,
            ct: ct);

        var itemsDto = paginado.Items
            .Select(u => u.Adapt<UsuarioResumenDto>())
            .ToList();

        var resultado = new PagedResult<UsuarioResumenDto>
        {
            Items = itemsDto,
            Pagina = paginado.Pagina,
            TamanoPagina = paginado.TamanoPagina,
            TotalRegistros = paginado.TotalRegistros
        };

        return Result.Exito<PagedResult<UsuarioResumenDto>>(resultado);
    }
}
