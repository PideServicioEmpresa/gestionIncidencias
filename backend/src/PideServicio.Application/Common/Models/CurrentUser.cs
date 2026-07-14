namespace PideServicio.Application.Common.Models;

using PideServicio.Domain.Enums;

public sealed record CurrentUser(
    Guid Id,
    Guid AuthId,
    string Email,
    string NombreCompleto,
    RolTipo Rol,
    Guid EmpresaId,
    IReadOnlyList<Guid> SucursalIds,
    bool Activo
)
{
    public bool EsSuperAdmin => Rol == RolTipo.SUPERADMIN;
    public bool EsAdmin => Rol == RolTipo.ADMIN;
    public bool EsSupervisor => Rol == RolTipo.SUPERVISOR;
    public bool EsTecnico => Rol == RolTipo.TECNICO;
    public bool EsTrabajador => Rol == RolTipo.TRABAJADOR;
    public bool EsUsuario => Rol == RolTipo.USUARIO;

    public bool TieneAccesoAdministrativo =>
        Rol is RolTipo.SUPERADMIN or RolTipo.ADMIN or RolTipo.SUPERVISOR;

    public bool PuedeManejarTickets =>
        Rol is RolTipo.TECNICO or RolTipo.TRABAJADOR;

    public bool PerteneceASucursal(Guid sucursalId) =>
        SucursalIds.Contains(sucursalId);
}
