namespace PideServicio.Application.Features.Comentarios.Commands.EliminarComentario;

using PideServicio.Application.Common.CQRS;

public sealed record EliminarComentarioCommand(Guid ComentarioId) : ICommand;
