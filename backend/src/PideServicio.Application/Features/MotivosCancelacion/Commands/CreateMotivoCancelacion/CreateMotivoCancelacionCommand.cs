namespace PideServicio.Application.Features.MotivosCancelacion.Commands.CreateMotivoCancelacion;

using PideServicio.Application.Common.CQRS;

public sealed record CreateMotivoCancelacionCommand(
    string Texto,
    Guid? EmpresaId
) : ICommand<Guid>;
