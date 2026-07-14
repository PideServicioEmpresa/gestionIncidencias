namespace PideServicio.Application.Features.MotivosCancelacion.Commands.ActivarMotivoCancelacion;

using PideServicio.Application.Common.CQRS;

public sealed record ActivarMotivoCancelacionCommand(Guid Id) : ICommand<Guid>;
