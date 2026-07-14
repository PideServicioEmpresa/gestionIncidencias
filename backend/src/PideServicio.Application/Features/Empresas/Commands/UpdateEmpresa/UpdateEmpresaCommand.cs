namespace PideServicio.Application.Features.Empresas.Commands.UpdateEmpresa;

using PideServicio.Application.Common.CQRS;

public sealed record UpdateEmpresaCommand(
    Guid Id,
    string NombreComercial,
    string RazonSocial,
    string ZonaHoraria,
    string? LogoUrl,
    string? ColorPrimario,
    string? ColorSecundario
) : ICommand<Guid>;
