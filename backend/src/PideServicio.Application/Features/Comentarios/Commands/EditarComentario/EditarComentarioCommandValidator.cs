namespace PideServicio.Application.Features.Comentarios.Commands.EditarComentario;

using FluentValidation;

public sealed class EditarComentarioCommandValidator : AbstractValidator<EditarComentarioCommand>
{
    public EditarComentarioCommandValidator()
    {
        RuleFor(x => x.ComentarioId)
            .NotEmpty().WithMessage("El identificador del comentario es requerido.");

        RuleFor(x => x.NuevoCuerpo)
            .NotEmpty().WithMessage("El nuevo cuerpo del comentario es requerido.")
            .MaximumLength(2000).WithMessage("El comentario no puede exceder 2000 caracteres.");
    }
}
