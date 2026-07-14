namespace PideServicio.Application.Features.TiposServicio.Commands.CreateTipoServicio;

using PideServicio.Application.Common.CQRS;

public sealed record CreateTipoServicioCommand(
    Guid EmpresaId,
    string Nombre,
    int Orden,
    string? Descripcion
) : ICommand<Guid>;
