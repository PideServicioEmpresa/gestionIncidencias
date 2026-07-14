namespace PideServicio.Application.Features.Comentarios.Commands.CrearComentario;

using PideServicio.Application.Common.CQRS;

public sealed record CrearComentarioCommand(
    Guid TicketId,
    string Cuerpo,
    bool EsInterno = false) : ICommand<Guid>;
