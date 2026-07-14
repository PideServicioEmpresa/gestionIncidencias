namespace PideServicio.Application.Features.Areas.Commands.ToggleAreaActiva;

using PideServicio.Application.Common.CQRS;

public sealed record ToggleAreaActivaCommand(Guid Id) : ICommand<Guid>;
