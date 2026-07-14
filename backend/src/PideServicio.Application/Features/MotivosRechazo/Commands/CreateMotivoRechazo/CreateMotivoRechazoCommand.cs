namespace PideServicio.Application.Features.MotivosRechazo.Commands.CreateMotivoRechazo;

using PideServicio.Application.Common.CQRS;

public sealed record CreateMotivoRechazoCommand(
    string Codigo,
    string Nombre,
    int Orden,
    bool EsOtro,
    string? Descripcion,
    Guid? EmpresaId
) : ICommand<Guid>;
