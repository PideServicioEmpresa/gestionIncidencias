namespace PideServicio.Application.Features.Usuarios.Commands.ActualizarEspecialidadesUsuario;

using FluentValidation;

public sealed class ActualizarEspecialidadesUsuarioCommandValidator
    : AbstractValidator<ActualizarEspecialidadesUsuarioCommand>
{
    public ActualizarEspecialidadesUsuarioCommandValidator()
    {
        // La lista puede venir vacía: significa "sin especialidades".
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("El id del usuario es requerido.");
    }
}
