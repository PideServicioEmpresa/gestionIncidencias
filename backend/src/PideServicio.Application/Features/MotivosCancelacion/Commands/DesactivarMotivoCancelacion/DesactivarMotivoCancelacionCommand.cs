namespace PideServicio.Application.Features.MotivosCancelacion.Commands.DesactivarMotivoCancelacion;

using PideServicio.Application.Common.CQRS;

public sealed record DesactivarMotivoCancelacionCommand(Guid Id) : ICommand<Guid>;
