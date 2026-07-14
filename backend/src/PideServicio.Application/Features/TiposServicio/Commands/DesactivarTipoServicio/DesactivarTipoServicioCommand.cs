namespace PideServicio.Application.Features.TiposServicio.Commands.DesactivarTipoServicio;

using PideServicio.Application.Common.CQRS;

public sealed record DesactivarTipoServicioCommand(Guid Id) : ICommand<Guid>;
