namespace PideServicio.Application.Features.Empresas.Commands.DesactivarEmpresa;

using PideServicio.Application.Common.CQRS;

public sealed record DesactivarEmpresaCommand(Guid Id) : ICommand<Guid>;
