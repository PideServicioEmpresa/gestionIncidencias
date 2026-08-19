namespace PideServicio.Application.Features.Usuarios.Queries.ListUsuarios;

using Mapster;
using Microsoft.Extensions.Logging;
using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Enums;

public sealed class ListUsuariosQueryHandler : IQueryHandler<ListUsuariosQuery, PagedResult<UsuarioResumenDto>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioEspecialidadRepository _usuarioEspecialidadRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ListUsuariosQueryHandler> _logger;

    public ListUsuariosQueryHandler(
        IUsuarioRepository usuarioRepository,
        IUsuarioEspecialidadRepository usuarioEspecialidadRepository,
        ICurrentUserService currentUserService,
        ILogger<ListUsuariosQueryHandler> logger)
    {
        _usuarioRepository = usuarioRepository;
        _usuarioEspecialidadRepository = usuarioEspecialidadRepository;
        _currentUserService = currentUserService;
        _logger = logger;
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

        // Especialidades de toda la página en UNA consulta agregada (no una por usuario).
        // Es un dato meramente informativo: si falla (por ejemplo, porque la migración
        // de usuario_especialidades aún no se aplicó), el listado debe seguir funcionando.
        IReadOnlyDictionary<Guid, IReadOnlyList<string>> especialidadesPorUsuario;
        try
        {
            var idsPagina = paginado.Items.Select(u => u.Id).ToList();
            especialidadesPorUsuario =
                await _usuarioEspecialidadRepository.ListarNombresPorUsuariosAsync(idsPagina, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudieron cargar las especialidades del listado de usuarios.");
            especialidadesPorUsuario = new Dictionary<Guid, IReadOnlyList<string>>();
        }

        var itemsDto = paginado.Items
            .Select(u => u.Adapt<UsuarioResumenDto>() with
            {
                Especialidades = especialidadesPorUsuario.TryGetValue(u.Id, out var nombres)
                    ? nombres
                    : [],
            })
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
