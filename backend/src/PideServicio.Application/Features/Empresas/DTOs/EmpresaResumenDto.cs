namespace PideServicio.Application.Features.Empresas.DTOs;

public sealed record EmpresaResumenDto(
    Guid Id,
    string NombreComercial,
    bool Activa,
    DateTimeOffset CreatedAt);
