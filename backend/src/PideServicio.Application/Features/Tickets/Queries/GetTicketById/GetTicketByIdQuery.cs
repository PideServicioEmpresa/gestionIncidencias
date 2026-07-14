namespace PideServicio.Application.Features.Tickets.Queries.GetTicketById;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Features.Tickets.DTOs;

public sealed record GetTicketByIdQuery(Guid Id) : IQuery<TicketDetalleDto>;
