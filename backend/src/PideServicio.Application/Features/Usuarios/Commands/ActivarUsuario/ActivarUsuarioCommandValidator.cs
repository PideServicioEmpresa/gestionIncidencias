namespace PideServicio.Application.Features.Usuarios.Commands.ActivarUsuario;

using FluentValidation;

public sealed class ActivarUsuarioCommandValidator : AbstractValidator<ActivarUsuarioCommand>
{
    public ActivarUsuarioCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("El ID del usuario es obligatorio.");
    }
}
