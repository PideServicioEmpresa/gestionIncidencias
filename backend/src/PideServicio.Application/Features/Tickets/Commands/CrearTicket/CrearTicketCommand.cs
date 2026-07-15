namespace PideServicio.Application.Features.Tickets.Commands.CrearTicket;

using PideServicio.Application.Common.CQRS;
using PideServicio.Domain.Enums;

public sealed record CrearTicketCommand(
    string Titulo,
    string Descripcion,
    Guid SucursalId,
    string AreaNombre,
    Guid TipoServicioId,
    Guid CategoriaId,
    PrioridadTipo Prioridad,
    string? Ubicacion
) : ICommand<Guid>;
