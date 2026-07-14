namespace PideServicio.Application.Features.MotivosRechazo.Commands.UpdateMotivoRechazo;

using PideServicio.Application.Common.CQRS;

public sealed record UpdateMotivoRechazoCommand(
    Guid Id,
    string Nombre,
    string Codigo,
    int Orden,
    string? Descripcion
) : ICommand<Guid>;
