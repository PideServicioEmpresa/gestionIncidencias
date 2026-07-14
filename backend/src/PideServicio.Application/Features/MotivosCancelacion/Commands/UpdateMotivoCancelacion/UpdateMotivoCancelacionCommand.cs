namespace PideServicio.Application.Features.MotivosCancelacion.Commands.UpdateMotivoCancelacion;

using PideServicio.Application.Common.CQRS;

public sealed record UpdateMotivoCancelacionCommand(
    Guid Id,
    string Texto
) : ICommand<Guid>;
