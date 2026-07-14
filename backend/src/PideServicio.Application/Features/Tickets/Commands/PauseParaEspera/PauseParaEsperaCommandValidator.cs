namespace PideServicio.Application.Features.Tickets.Commands.PauseParaEspera;

using FluentValidation;

public sealed class PauseParaEsperaCommandValidator : AbstractValidator<PauseParaEsperaCommand>
{
    public PauseParaEsperaCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El ID del ticket es obligatorio.");
    }
}
