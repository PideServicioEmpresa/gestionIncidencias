namespace PideServicio.Application.Features.Auth.Commands.RecuperarContrasena;

using FluentValidation;

public sealed class RecuperarContrasenaCommandValidator : AbstractValidator<RecuperarContrasenaCommand>
{
    public RecuperarContrasenaCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
            .MaximumLength(320).WithMessage("El correo no puede exceder 320 caracteres.");
    }
}
