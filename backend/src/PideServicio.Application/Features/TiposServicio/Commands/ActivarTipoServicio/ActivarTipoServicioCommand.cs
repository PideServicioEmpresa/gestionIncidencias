namespace PideServicio.Application.Features.TiposServicio.Commands.ActivarTipoServicio;

using PideServicio.Application.Common.CQRS;

public sealed record ActivarTipoServicioCommand(Guid Id) : ICommand<Guid>;
