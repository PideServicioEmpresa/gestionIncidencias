namespace PideServicio.Application.Features.Especialidades.Commands.CreateEspecialidad;

using FluentValidation;

public sealed class CreateEspecialidadCommandValidator : AbstractValidator<CreateEspecialidadCommand>
{
    public CreateEspecialidadCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descripcion).MaximumLength(500).When(x => x.Descripcion is not null);
    }
}
