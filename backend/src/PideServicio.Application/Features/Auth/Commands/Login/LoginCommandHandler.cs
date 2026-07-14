namespace PideServicio.Application.Features.Auth.Commands.Login;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Auth.DTOs;
using PideServicio.Domain.Enums;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponseDto>
{
    private readonly ISupabaseAuthService _supabaseAuth;
    private readonly IUsuarioRepository _usuarios;

    public LoginCommandHandler(ISupabaseAuthService supabaseAuth, IUsuarioRepository usuarios)
    {
        _supabaseAuth = supabaseAuth;
        _usuarios = usuarios;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        SupabaseAuthResult authResult;
        try
        {
            authResult = await _supabaseAuth.LoginAsync(request.Email, request.Password, ct);
        }
        catch (InvalidOperationException ex)
        {
            return Result.NoAutorizado<LoginResponseDto>(ex.Message);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            return Result.Fallo<LoginResponseDto>($"Error al autenticar: {ex.Message}");
        }

        var usuario = await _usuarios.ObtenerPorAuthIdAsync(authResult.SupabaseUserId, ct);
        if (usuario is null)
            return Result.NoEncontrado<LoginResponseDto>(
                "No existe un perfil de usuario asociado a esta cuenta. Contacte al administrador.");

        if (!usuario.Activo)
            return Result.NoPermitido<LoginResponseDto>(
                "Su cuenta está desactivada. Contacte al administrador.");

        var perfil = MapearPerfil(usuario);

        return Result.Exito(new LoginResponseDto(
            AccessToken: authResult.AccessToken,
            RefreshToken: authResult.RefreshToken,
            ExpiresIn: authResult.ExpiresIn,
            TipoToken: "bearer",
            Perfil: perfil));
    }

    private static PerfilUsuarioDto MapearPerfil(Domain.Entities.Usuario u) => new(
        Id: u.Id,
        AuthId: u.AuthId,
        EmpresaId: u.EmpresaId,
        SucursalId: u.SucursalId,
        AreaId: u.AreaId,
        Nombre: u.Nombre,
        Apellido: u.Apellido,
        NombreCompleto: u.NombreCompleto,
        Correo: u.Correo.Valor,
        NombreUsuario: u.NombreUsuario,
        Telefono: u.Telefono,
        FotoUrl: u.FotoUrl,
        Rol: u.Rol.ToString(),
        EstadoLaboral: u.EstadoLaboral.ToString(),
        Activo: u.Activo,
        UltimoAcceso: u.UltimoAcceso,
        Permisos: BuildPermisos(u.Rol));

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
