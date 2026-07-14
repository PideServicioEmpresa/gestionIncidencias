namespace PideServicio.Application.Features.MotivosRechazo.Commands.ActivarMotivoRechazo;

using PideServicio.Application.Common.CQRS;

public sealed record ActivarMotivoRechazoCommand(Guid Id) : ICommand<Guid>;
