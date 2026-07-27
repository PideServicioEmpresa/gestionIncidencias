namespace PideServicio.Application.Features.Auth.Queries.GetMe;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Auth.DTOs;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetMeQueryHandler : IQueryHandler<GetMeQuery, PerfilUsuarioDto>
{
    private static readonly RolTipo[] _rolesMultiSucursal = [RolTipo.TECNICO, RolTipo.TRABAJADOR, RolTipo.USUARIO];

    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarios;
    private readonly IUsuarioSucursalRepository _usuarioSucursales;

    public GetMeQueryHandler(
        ICurrentUserService currentUser,
        IUsuarioRepository usuarios,
        IUsuarioSucursalRepository usuarioSucursales)
    {
        _currentUser = currentUser;
        _usuarios = usuarios;
        _usuarioSucursales = usuarioSucursales;
    }

    public async Task<Result<PerfilUsuarioDto>> Handle(GetMeQuery request, CancellationToken ct)
    {
        var actor = _currentUser.UsuarioActual;
        if (actor is null)
            return Result.NoAutorizado<PerfilUsuarioDto>();

        var usuario = actor.Id != Guid.Empty
            ? await _usuarios.ObtenerPorIdAsync(actor.Id, ct)
            : await _usuarios.ObtenerPorAuthIdAsync(actor.AuthId, ct);
        if (usuario is null)
            return Result.NoEncontrado<PerfilUsuarioDto>(
                "No se encontró el perfil del usuario. Contacte al administrador.");

        // Los roles multi-sucursal tienen asignaciones en usuario_sucursales;
        // Admin y SuperAdmin tienen la lista vacía (sus sucursales se limpian al cambiar de rol).
        IReadOnlyList<SucursalAsignacionDto> sucursales = _rolesMultiSucursal.Contains(usuario.Rol)
            ? await _usuarioSucursales.ListarPorUsuarioAsync(usuario.Id, ct)
            : [];

        return Result.Exito(new PerfilUsuarioDto(
            Id: usuario.Id,
            AuthId: usuario.AuthId,
            EmpresaId: usuario.EmpresaId,
            SucursalId: usuario.SucursalId,
            AreaId: usuario.AreaId,
            Nombre: usuario.Nombre,
            Apellido: usuario.Apellido,
            NombreCompleto: usuario.NombreCompleto,
            Correo: usuario.Correo.Valor,
            NombreUsuario: usuario.NombreUsuario,
            Telefono: usuario.Telefono,
            FotoUrl: usuario.FotoUrl,
            Rol: usuario.Rol.ToString(),
            EstadoLaboral: usuario.EstadoLaboral.ToString(),
            Activo: usuario.Activo,
            UltimoAcceso: usuario.UltimoAcceso,
            Permisos: BuildPermisos(usuario.Rol),
            Sucursales: sucursales));
    }

    private static PermisosDto BuildPermisos(RolTipo rol) => new(
        EsSuperAdmin: rol == RolTipo.SUPERADMIN,
        EsAdmin: rol == RolTipo.ADMIN,
        EsSupervisor: rol == RolTipo.SUPERVISOR,
        EsTecnico: rol == RolTipo.TECNICO,
        EsTrabajador: rol == RolTipo.TRABAJADOR,
        EsUsuario: rol == RolTipo.USUARIO,
        TieneAccesoAdministrativo: rol is RolTipo.SUPERADMIN or RolTipo.ADMIN or RolTipo.SUPERVISOR,
        PuedeManejarTickets: rol is RolTipo.SUPERADMIN or RolTipo.ADMIN or RolTipo.SUPERVISOR or RolTipo.TECNICO);
}
