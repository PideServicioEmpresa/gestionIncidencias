namespace PideServicio.Application.Features.Usuarios.Commands.CreateUsuario;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

public sealed class CreateUsuarioCommandHandler : ICommandHandler<CreateUsuarioCommand, Guid>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISupabaseAuthService _supabaseAuth;
    private readonly IAuditService _auditService;

    public CreateUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        ICurrentUserService currentUserService,
        ISupabaseAuthService supabaseAuth,
        IAuditService auditService)
    {
        _usuarioRepository = usuarioRepository;
        _currentUserService = currentUserService;
        _supabaseAuth = supabaseAuth;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(CreateUsuarioCommand request, CancellationToken ct)
    {
        var claims = _currentUserService.UsuarioActual;
        if (claims is null) return Result.NoAutorizado<Guid>();

        // Resolver el actor real desde la BD (necesario cuando el hook de Supabase no está activo)
        var actorDb = claims.Id != Guid.Empty
            ? await _usuarioRepository.ObtenerPorIdAsync(claims.Id, ct)
            : await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, ct);

        if (actorDb is null || !actorDb.Activo)
            return Result.NoAutorizado<Guid>();

        if (actorDb.Rol is not (RolTipo.ADMIN or RolTipo.SUPERADMIN))
            return Result.NoPermitido<Guid>("Solo administradores pueden crear usuarios.");

        if (request.Rol == RolTipo.SUPERADMIN && actorDb.Rol != RolTipo.SUPERADMIN)
            return Result.NoPermitido<Guid>("Solo un SuperAdministrador puede asignar el rol SUPERADMIN.");

        var empresaId = actorDb.EmpresaId;

        try
        {
            var correoExiste = await _usuarioRepository.ExisteCorreoAsync(
                request.Correo, empresaId, excluirId: null, ct);
            if (correoExiste)
                return Result.ErrorValidacion<Guid>("Correo", "Ya existe un usuario con ese correo en la empresa.");

            var nombreUsuarioExiste = await _usuarioRepository.ExisteNombreUsuarioAsync(
                request.NombreUsuario, excluirId: null, ct);
            if (nombreUsuarioExiste)
                return Result.ErrorValidacion<Guid>("NombreUsuario", "El nombre de usuario ya está en uso.");

            // 1. Crear usuario en Supabase Auth → obtener authId
            Guid authId;
            try
            {
                authId = await _supabaseAuth.CrearUsuarioEnAuthAsync(request.Correo, request.Contrasena, ct);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Fallo<Guid>($"Error al registrar usuario en el sistema de autenticación: {ex.Message}");
            }

            // 2. Crear el registro en la BD; si falla, revertir en Supabase Auth
            Guid nuevoId;
            try
            {
                var usuario = Usuario.Crear(
                    authId: authId,
                    empresaId: empresaId,
                    sucursalId: request.SucursalId,
                    nombre: request.Nombre,
                    apellido: request.Apellido,
                    correo: request.Correo,
                    nombreUsuario: request.NombreUsuario,
                    rol: request.Rol,
                    areaId: request.AreaId,
                    telefono: request.Telefono,
                    creadoPor: actorDb.Id);

                nuevoId = await _usuarioRepository.CrearAsync(usuario, ct);

                await _auditService.RegistrarAsync(
                    entidad: "Usuario",
                    entidadId: nuevoId,
                    accion: "Crear",
                    antes: null,
                    despues: new
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Correo = usuario.Correo.Valor,
                        Rol = usuario.Rol.ToString(),
                        EmpresaId = empresaId,
                        SucursalId = usuario.SucursalId
                    },
                    cancellationToken: ct);
            }
            catch
            {
                // Revertir la creación en Supabase Auth para evitar usuarios huérfanos
                await _supabaseAuth.EliminarUsuarioDeAuthAsync(authId, ct);
                throw;
            }

            return Result.Exito<Guid>(nuevoId);
        }
        catch (ValidationException ex)
        {
            return Result.ErrorValidacion<Guid>(ex.Errors);
        }
        catch (NotFoundException ex)
        {
            return Result.NoEncontrado<Guid>(ex.Message);
        }
        catch (ForbiddenException ex)
        {
            return Result.NoPermitido<Guid>(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.Fallo<Guid>(ex.Message);
        }
    }
}
