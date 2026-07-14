namespace PideServicio.Application.Features.MotivosCancelacion.Commands.UpdateMotivoCancelacion;

using FluentValidation;

public sealed class UpdateMotivoCancelacionCommandValidator : AbstractValidator<UpdateMotivoCancelacionCommand>
{
    public UpdateMotivoCancelacionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Texto).NotEmpty().MaximumLength(300);
    }
}
