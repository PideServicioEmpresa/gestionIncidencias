namespace PideServicio.Infrastructure.Services;

using Microsoft.AspNetCore.Http;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Enums;

public sealed class SucursalActivaService : ISucursalActivaService
{
    private const string HeaderName = "X-Sucursal-Activa";
    private static readonly RolTipo[] _rolesSinSucursal = [RolTipo.ADMIN, RolTipo.SUPERADMIN];

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsuarioSucursalRepository _usuarioSucursalRepository;

    // Cache dentro del scope: una petición HTTP = una instancia Scoped.
    // Evita múltiples queries a BD si el servicio se llama varias veces en el mismo request.
    private bool _calculado;
    private Guid? _resultado;

    public SucursalActivaService(
        IHttpContextAccessor httpContextAccessor,
        IUsuarioSucursalRepository usuarioSucursalRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _usuarioSucursalRepository = usuarioSucursalRepository;
    }

    public async Task<Guid?> ObtenerAsync(Guid usuarioId, Guid sucursalPrincipal, RolTipo rol, CancellationToken ct = default)
    {
        // Admin y SuperAdmin no operan sobre sucursales individuales
        if (_rolesSinSucursal.Contains(rol))
            return null;

        // Resultado cacheado para esta petición
        if (_calculado)
            return _resultado;

        var headerValue = _httpContextAccessor.HttpContext?
            .Request.Headers[HeaderName]
            .FirstOrDefault();

        if (!Guid.TryParse(headerValue, out var sucursalSolicitada))
        {
            // Header ausente o con formato inválido → usar la sucursal principal
            _calculado = true;
            _resultado = sucursalPrincipal;
            return _resultado;
        }

        // Validar en BD que la sucursal del header pertenece a este usuario
        var asignadas = await _usuarioSucursalRepository.ListarPorUsuarioAsync(usuarioId, ct);
        var perteneceAlUsuario = asignadas.Any(s => s.SucursalId == sucursalSolicitada && s.Activo);

        _calculado = true;
        _resultado = perteneceAlUsuario ? sucursalSolicitada : sucursalPrincipal;
        return _resultado;
    }
}
