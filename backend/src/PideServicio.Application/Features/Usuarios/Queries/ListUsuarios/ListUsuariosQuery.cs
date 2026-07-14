namespace PideServicio.Application.Features.Usuarios.Queries.ListUsuarios;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Enums;

public sealed record ListUsuariosQuery(
    Guid? EmpresaId,
    Guid? SucursalId,
    RolTipo? Rol,
    bool? SoloActivos,
    string? Busqueda,
    int Pagina = 1,
    int TamanoPagina = 20) : IQuery<PagedResult<UsuarioResumenDto>>;
