namespace PideServicio.Application.Features.Empresas.DTOs;

public sealed record EmpresaDto(
    Guid Id,
    string NombreComercial,
    string RazonSocial,
    string IdentificacionFiscal,
    string? LogoUrl,
    string? ColorPrimario,
    string? ColorSecundario,
    string ZonaHoraria,
    bool Activa,
    DateTimeOffset CreatedAt);
