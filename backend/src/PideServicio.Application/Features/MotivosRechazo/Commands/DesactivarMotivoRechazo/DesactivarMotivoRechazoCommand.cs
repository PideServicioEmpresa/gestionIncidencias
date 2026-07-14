namespace PideServicio.Application.Features.MotivosRechazo.Commands.DesactivarMotivoRechazo;

using PideServicio.Application.Common.CQRS;

public sealed record DesactivarMotivoRechazoCommand(Guid Id) : ICommand<Guid>;
