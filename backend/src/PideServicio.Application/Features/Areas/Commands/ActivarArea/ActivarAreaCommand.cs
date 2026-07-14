namespace PideServicio.Application.Features.Areas.Commands.ActivarArea;

using PideServicio.Application.Common.CQRS;

public sealed record ActivarAreaCommand(Guid Id) : ICommand<Guid>;
