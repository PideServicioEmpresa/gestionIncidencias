namespace PideServicio.Application.Features.Areas.Commands.DesactivarArea;

using PideServicio.Application.Common.CQRS;

public sealed record DesactivarAreaCommand(Guid Id) : ICommand<Guid>;
