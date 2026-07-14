namespace PideServicio.Application.Features.Comentarios.Commands.EditarComentario;

using PideServicio.Application.Common.CQRS;

public sealed record EditarComentarioCommand(
    Guid ComentarioId,
    string NuevoCuerpo) : ICommand<Guid>;
