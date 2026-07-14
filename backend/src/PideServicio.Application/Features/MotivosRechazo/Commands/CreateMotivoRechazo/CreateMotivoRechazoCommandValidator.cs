namespace PideServicio.Application.Features.MotivosRechazo.Commands.CreateMotivoRechazo;

using FluentValidation;

public sealed class CreateMotivoRechazoCommandValidator : AbstractValidator<CreateMotivoRechazoCommand>
{
    public CreateMotivoRechazoCommandValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Orden).GreaterThan(0);
        RuleFor(x => x.Descripcion).MaximumLength(500).When(x => x.Descripcion is not null);
    }
}
