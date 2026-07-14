namespace PideServicio.Application.Features.Auth.Commands.CambiarContrasena;

using FluentValidation;

public sealed class CambiarContrasenaCommandValidator : AbstractValidator<CambiarContrasenaCommand>
{
    public CambiarContrasenaCommandValidator()
    {
        RuleFor(x => x.NuevaContrasena)
            .NotEmpty().WithMessage("La nueva contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .MaximumLength(72).WithMessage("La contraseña no puede exceder 72 caracteres.")
            .Matches("[A-Z]").WithMessage("La contraseña debe tener al menos una mayúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe tener al menos un número.");

        RuleFor(x => x.ConfirmacionContrasena)
            .NotEmpty().WithMessage("La confirmación de contraseña es requerida.")
            .Equal(x => x.NuevaContrasena).WithMessage("Las contraseñas no coinciden.");
    }
}
