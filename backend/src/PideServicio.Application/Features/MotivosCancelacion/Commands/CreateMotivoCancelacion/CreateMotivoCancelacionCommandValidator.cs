namespace PideServicio.Application.Features.MotivosCancelacion.Commands.CreateMotivoCancelacion;

using FluentValidation;

public sealed class CreateMotivoCancelacionCommandValidator : AbstractValidator<CreateMotivoCancelacionCommand>
{
    public CreateMotivoCancelacionCommandValidator()
    {
        RuleFor(x => x.Texto).NotEmpty().MaximumLength(300);
    }
}
