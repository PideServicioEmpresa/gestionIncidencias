namespace PideServicio.Application.Features.Usuarios.Commands.DesactivarUsuario;

using FluentValidation;

public sealed class DesactivarUsuarioCommandValidator : AbstractValidator<DesactivarUsuarioCommand>
{
    public DesactivarUsuarioCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("El ID del usuario es obligatorio.");
    }
}
