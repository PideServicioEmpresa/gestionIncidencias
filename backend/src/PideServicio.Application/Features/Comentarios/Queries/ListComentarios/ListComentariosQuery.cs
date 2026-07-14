namespace PideServicio.Application.Features.Comentarios.Queries.ListComentarios;

using PideServicio.Application.Common.CQRS;
using PideServicio.Application.Common.Models;
using PideServicio.Application.Features.Comentarios.DTOs;

public sealed record ListComentariosQuery(Guid TicketId)
    : IQuery<ListResult<ComentarioDto>>;
