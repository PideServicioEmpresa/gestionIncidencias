namespace PideServicio.Application.Features.Configuracion.Commands.UpdateParametro;

using FluentValidation;

public sealed class UpdateParametroCommandValidator : AbstractValidator<UpdateParametroCommand>
{
    public UpdateParametroCommandValidator()
    {
        RuleFor(x => x.Clave)
            .NotEmpty().WithMessage("La clave del parámetro es obligatoria.")
            .MaximumLength(200).WithMessage("La clave no puede superar los 200 caracteres.");

        RuleFor(x => x.NuevoValor)
            .NotEmpty().WithMessage("El nuevo valor del parámetro es obligatorio.")
            .MaximumLength(2000).WithMessage("El valor no puede superar los 2000 caracteres.");
    }
}
