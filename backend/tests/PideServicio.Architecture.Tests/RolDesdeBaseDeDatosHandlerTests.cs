namespace PideServicio.Architecture.Tests;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using PideServicio.Api.Authorization;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using Xunit;

/// <summary>
/// El caso que provocó el 403 en Configuración: un SUPERADMIN cuyo token NO trae el
/// claim "rol" enriquecido. Con RequireClaim quedaba fuera; el handler debe dejarlo
/// pasar porque resuelve el rol desde la base de datos.
/// </summary>
public class RolDesdeBaseDeDatosHandlerTests
{
    /// <summary>Simula CurrentUserService cuando el hook de Supabase NO enriqueció el token.</summary>
    private sealed class CurrentUserSinClaimDeRol : ICurrentUserService
    {
        private readonly Guid _authId;
        public CurrentUserSinClaimDeRol(Guid authId) => _authId = authId;

        public bool EstaAutenticado => true;

        // Así queda el usuario en el camino de respaldo: sin Id, rol degradado a USUARIO.
        public CurrentUser? UsuarioActual => new(
            Id: Guid.Empty,
            AuthId: _authId,
            Email: "milagros@inmoveg.pe",
            NombreCompleto: string.Empty,
            Rol: RolTipo.USUARIO,
            EmpresaId: Guid.Empty,
            SucursalIds: [],
            Activo: true);
    }

    private sealed class UsuarioRepositoryFake : IUsuarioRepository
    {
        private readonly Usuario? _usuario;
        public UsuarioRepositoryFake(Usuario? usuario) => _usuario = usuario;

        public Task<Usuario?> ObtenerPorAuthIdAsync(Guid authId, CancellationToken ct = default)
            => Task.FromResult(_usuario);

        // El resto del contrato no interviene en este escenario.
        public Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<Usuario?>(null);
        public Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task<PagedResult<Usuario>> ListarAsync(Guid empresaId, Guid? sucursalId, RolTipo? rol,
            bool? soloActivos, string? busqueda, int pagina, int tamanoPagina, CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task<IReadOnlyList<Usuario>> ListarAdminsActivosPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task<IReadOnlyList<Usuario>> ListarSuperAdminsActivosAsync(CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task<IReadOnlyList<Usuario>> ListarTecnicosActivosPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task<bool> ExisteCorreoAsync(string correo, Guid empresaId, Guid? excluirId = null, CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, Guid? excluirId = null, CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task<Guid> CrearAsync(Usuario usuario, CancellationToken ct = default)
            => throw new NotImplementedException();
        public Task ActualizarAsync(Usuario usuario, CancellationToken ct = default)
            => throw new NotImplementedException();
    }

    private static async Task<bool> EvaluarAsync(Usuario? usuarioEnBd, params RolTipo[] permitidos)
    {
        var authId = Guid.NewGuid();
        var handler = new RolDesdeBaseDeDatosHandler(
            new CurrentUserSinClaimDeRol(authId),
            new UsuarioRepositoryFake(usuarioEnBd),
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<RolDesdeBaseDeDatosHandler>.Instance);

        var requisito = new RolMinimoRequirement(permitidos);
        // Identidad autenticada pero SIN claim "rol": el escenario del bug.
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", authId.ToString())], "Bearer"));
        var contexto = new AuthorizationHandlerContext([requisito], principal, null);

        await handler.HandleAsync(contexto);
        return contexto.HasSucceeded;
    }

    private static Usuario CrearUsuario(RolTipo rol) => Usuario.Crear(
        authId: Guid.NewGuid(),
        empresaId: Guid.NewGuid(),
        sucursalId: Guid.NewGuid(),
        nombre: "Milagros",
        apellido: "Maco",
        correo: "milagros@inmoveg.pe",
        nombreUsuario: "milagros",
        rol: rol);

    [Fact]
    public async Task SuperAdmin_sin_claim_de_rol_es_autorizado_porque_se_resuelve_desde_la_BD()
    {
        var autorizado = await EvaluarAsync(
            CrearUsuario(RolTipo.SUPERADMIN),
            RolTipo.SUPERADMIN, RolTipo.ADMIN);

        Assert.True(autorizado, "Un SUPERADMIN en la BD debe pasar aunque el token no traiga el claim 'rol'.");
    }

    [Fact]
    public async Task Admin_sin_claim_de_rol_tambien_es_autorizado()
    {
        var autorizado = await EvaluarAsync(
            CrearUsuario(RolTipo.ADMIN),
            RolTipo.SUPERADMIN, RolTipo.ADMIN);

        Assert.True(autorizado);
    }

    [Fact]
    public async Task Usuario_sin_rol_suficiente_sigue_siendo_rechazado()
    {
        var autorizado = await EvaluarAsync(
            CrearUsuario(RolTipo.USUARIO),
            RolTipo.SUPERADMIN, RolTipo.ADMIN);

        Assert.False(autorizado, "El fix no debe relajar la autorización para roles sin permiso.");
    }

    [Fact]
    public async Task Usuario_inexistente_en_la_BD_es_rechazado()
    {
        var autorizado = await EvaluarAsync(null, RolTipo.SUPERADMIN, RolTipo.ADMIN);
        Assert.False(autorizado);
    }
}
