namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Events.Usuarios;
using PideServicio.Domain.Exceptions;
using PideServicio.Domain.ValueObjects;

public sealed class Usuario : AggregateRoot
{
    public Guid AuthId { get; private set; }
    public Guid EmpresaId { get; private set; }
    public Guid SucursalId { get; private set; }
    public Guid? AreaId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public Email Correo { get; private set; } = null!;
    public string NombreUsuario { get; private set; } = string.Empty;
    public string? Telefono { get; private set; }
    public RolTipo Rol { get; private set; }
    public EstadoLaboralTipo EstadoLaboral { get; private set; } = EstadoLaboralTipo.ACTIVO;
    public bool Activo { get; private set; } = true;
    public string? FotoUrl { get; private set; }
    public DateTimeOffset? UltimoAcceso { get; private set; }
    public int Version { get; private set; } = 1;

    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    private Usuario() { }

    public static Usuario Crear(
        Guid authId,
        Guid empresaId,
        Guid sucursalId,
        string nombre,
        string apellido,
        string correo,
        string nombreUsuario,
        RolTipo rol,
        Guid? areaId = null,
        string? telefono = null,
        Guid? creadoPor = null)
    {
        if (authId == Guid.Empty)
            throw new ValidationException("AuthId", "El auth_id del usuario es requerido.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre del usuario es requerido.");
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ValidationException("Apellido", "El apellido del usuario es requerido.");
        if (string.IsNullOrWhiteSpace(nombreUsuario))
            throw new ValidationException("NombreUsuario", "El nombre de usuario es requerido.");

        var emailVo = Email.Crear(correo);
        var ahora = DateTimeOffset.UtcNow;

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            AuthId = authId,
            EmpresaId = empresaId,
            SucursalId = sucursalId,
            AreaId = areaId,
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Correo = emailVo,
            NombreUsuario = nombreUsuario.Trim().ToLowerInvariant(),
            Telefono = telefono?.Trim(),
            Rol = rol,
            EstadoLaboral = EstadoLaboralTipo.ACTIVO,
            Activo = true,
            Version = 1,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor
        };

        usuario.AgregarEvento(new UsuarioCreadoEvent(
            usuario.Id,
            empresaId,
            emailVo.Valor,
            rol,
            ahora));

        return usuario;
    }

    public void ActualizarPerfil(string nombre, string apellido, string? telefono, Guid? areaId, Guid actorId)
    {
        ValidarActivo();
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidationException("Nombre", "El nombre del usuario es requerido.");
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ValidationException("Apellido", "El apellido del usuario es requerido.");

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
        Telefono = telefono?.Trim();
        AreaId = areaId;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actorId;
    }

    public void ActualizarFoto(string? fotoUrl, Guid actorId)
    {
        ValidarActivo();
        FotoUrl = fotoUrl;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actorId;
    }

    public void CambiarRol(RolTipo nuevoRol, Guid actorId)
    {
        ValidarNoEliminado();
        Rol = nuevoRol;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actorId;
    }

    public void CambiarEstadoLaboral(EstadoLaboralTipo nuevoEstado, Guid actorId)
    {
        ValidarActivo();
        EstadoLaboral = nuevoEstado;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actorId;
    }

    public void Desactivar(Guid actorId)
    {
        ValidarNoEliminado();
        Activo = false;
        EstadoLaboral = EstadoLaboralTipo.RETIRADO;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actorId;

        AgregarEvento(new UsuarioDesactivadoEvent(Id, EmpresaId, NombreCompleto, DateTimeOffset.UtcNow));
    }

    public void Activar(Guid actorId)
    {
        ValidarNoEliminado();
        Activo = true;
        EstadoLaboral = EstadoLaboralTipo.ACTIVO;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actorId;
    }

    public void RegistrarAcceso()
    {
        UltimoAcceso = DateTimeOffset.UtcNow;
    }

    private void ValidarActivo()
    {
        if (!Activo)
            throw new UsuarioInactivoException(NombreCompleto);
        ValidarNoEliminado();
    }

    private void ValidarNoEliminado()
    {
        if (IsDeleted)
            throw new ValidationException("Estado", "No se puede modificar un usuario eliminado.");
    }
}
