namespace PideServicio.Domain.Enums;

/// <summary>
/// Espejo del ENUM rol_tipo definido en PostgreSQL.
/// Jerarquía descendente de permisos: SUPERADMIN tiene más privilegios que USUARIO.
/// </summary>
public enum RolTipo
{
    SUPERADMIN,
    ADMIN,
    SUPERVISOR,
    TECNICO,
    TRABAJADOR,
    USUARIO
}
