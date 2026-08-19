namespace PideServicio.Application.Features.Especialidades.Commands.UpdateEspecialidad;

using FluentValidation;

public sealed class UpdateEspecialidadCommandValidator : AbstractValidator<UpdateEspecialidadCommand>
{
    public UpdateEspecialidadCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).MaximumLength(500).When(x => x.Descripcion is not null);
    }
}
