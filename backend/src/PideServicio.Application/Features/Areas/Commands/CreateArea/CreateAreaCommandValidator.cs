namespace PideServicio.Application.Features.Areas.Commands.CreateArea;

using FluentValidation;

public sealed class CreateAreaCommandValidator : AbstractValidator<CreateAreaCommand>
{
    public CreateAreaCommandValidator()
    {
        RuleFor(x => x.SucursalId)
            .NotEmpty().WithMessage("El identificador de la sucursal es requerido.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del área es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres.")
            .When(x => x.Descripcion is not null);
    }
}
