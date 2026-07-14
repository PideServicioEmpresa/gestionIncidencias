namespace PideServicio.Application.Features.Auth.Queries.GetMe;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Auth.DTOs;
using PideServicio.Domain.Enums;

public sealed class GetMeQueryHandler : IQueryHandler<GetMeQuery, PerfilUsuarioDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarios;

    public GetMeQueryHandler(ICurrentUserService currentUser, IUsuarioRepository usuarios)
    {
        _currentUser = currentUser;
        _usuarios = usuarios;
    }

    public async Task<Result<PerfilUsuarioDto>> Handle(GetMeQuery request, CancellationToken ct)
    {
        var actor = _currentUser.UsuarioActual;
        if (actor is null)
            return Result.NoAutorizado<PerfilUsuarioDto>();

        // Si los claims del hook están presentes busca por Id interno;
        // si solo está el sub estándar (sin hook activo) busca por auth_id.
        var usuario = actor.Id != Guid.Empty
            ? await _usuarios.ObtenerPorIdAsync(actor.Id, ct)
            : await _usuarios.ObtenerPorAuthIdAsync(actor.AuthId, ct);
        if (usuario is null)
            return Result.NoEncontrado<PerfilUsuarioDto>(
                "No se encontró el perfil del usuario. Contacte al administrador.");

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
            Permisos: BuildPermisos(usuario.Rol)));
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
