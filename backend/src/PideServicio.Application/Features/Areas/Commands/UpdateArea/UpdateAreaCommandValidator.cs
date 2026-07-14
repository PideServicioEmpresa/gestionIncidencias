namespace PideServicio.Application.Features.Areas.Commands.UpdateArea;

using FluentValidation;

public sealed class UpdateAreaCommandValidator : AbstractValidator<UpdateAreaCommand>
{
    public UpdateAreaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del área es requerido.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del área es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres.")
            .When(x => x.Descripcion is not null);
    }
}
