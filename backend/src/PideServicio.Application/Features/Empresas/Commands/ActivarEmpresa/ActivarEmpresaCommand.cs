namespace PideServicio.Application.Features.Empresas.Commands.ActivarEmpresa;

using PideServicio.Application.Common.CQRS;

public sealed record ActivarEmpresaCommand(Guid Id) : ICommand<Guid>;
