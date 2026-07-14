namespace PideServicio.Application.Features.Usuarios.Commands.UpdateUsuarioPerfil;

using FluentValidation;

public sealed class UpdateUsuarioPerfilCommandValidator : AbstractValidator<UpdateUsuarioPerfilCommand>
{
    public UpdateUsuarioPerfilCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("El identificador del usuario es requerido.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.Apellido)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres.");

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres.")
            .When(x => x.Telefono is not null);

        RuleFor(x => x.FotoUrl)
            .MaximumLength(500).WithMessage("La URL de la foto no puede exceder 500 caracteres.")
            .When(x => x.FotoUrl is not null);
    }
}
