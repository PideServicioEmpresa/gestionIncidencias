namespace PideServicio.Application.Features.Usuarios.Commands.CambiarRol;

using FluentValidation;

public sealed class CambiarRolCommandValidator : AbstractValidator<CambiarRolCommand>
{
    public CambiarRolCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("El identificador del usuario es requerido.");

        RuleFor(x => x.NuevoRol)
            .IsInEnum().WithMessage("El rol especificado no es válido.");
    }
}
