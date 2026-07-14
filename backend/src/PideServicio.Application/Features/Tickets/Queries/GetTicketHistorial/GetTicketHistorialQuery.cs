namespace PideServicio.Application.Features.Tickets.Queries.GetTicketHistorial;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Tickets.DTOs;

public sealed record GetTicketHistorialQuery(Guid TicketId) : IQuery<ListResult<TicketHistorialDto>>;
