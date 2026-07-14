namespace PideServicio.Application.Features.Tickets.Commands.SubmitParaValidacion;

using FluentValidation;

public sealed class SubmitParaValidacionCommandValidator : AbstractValidator<SubmitParaValidacionCommand>
{
    public SubmitParaValidacionCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El ID del ticket es obligatorio.");
    }
}
