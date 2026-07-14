namespace PideServicio.Application.Features.Usuarios.Commands.CambiarEstadoLaboral;

using FluentValidation;

public sealed class CambiarEstadoLaboralCommandValidator : AbstractValidator<CambiarEstadoLaboralCommand>
{
    public CambiarEstadoLaboralCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("El identificador del usuario es requerido.");

        RuleFor(x => x.NuevoEstado)
            .IsInEnum().WithMessage("El estado laboral especificado no es válido.");
    }
}
