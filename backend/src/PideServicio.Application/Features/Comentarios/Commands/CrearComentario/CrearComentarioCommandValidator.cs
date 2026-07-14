namespace PideServicio.Application.Features.Comentarios.Commands.CrearComentario;

using FluentValidation;

public sealed class CrearComentarioCommandValidator : AbstractValidator<CrearComentarioCommand>
{
    public CrearComentarioCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El identificador del ticket es requerido.");

        RuleFor(x => x.Cuerpo)
            .NotEmpty().WithMessage("El cuerpo del comentario es requerido.")
            .MaximumLength(2000).WithMessage("El comentario no puede exceder 2000 caracteres.");
    }
}
