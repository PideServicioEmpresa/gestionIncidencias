namespace PideServicio.Application.Features.Empresas.Commands.ToggleEmpresaActiva;

using PideServicio.Application.Common.CQRS;

public sealed record ToggleEmpresaActivaCommand(Guid Id) : ICommand<Guid>;
