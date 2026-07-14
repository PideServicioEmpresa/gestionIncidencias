namespace PideServicio.Application.Features.Empresas.Commands.CreateEmpresa;

using PideServicio.Application.Common.CQRS;

public sealed record CreateEmpresaCommand(
    string NombreComercial,
    string RazonSocial,
    string IdentificacionFiscal,
    string ZonaHoraria,
    string? LogoUrl,
    string? ColorPrimario,
    string? ColorSecundario
) : ICommand<Guid>;
