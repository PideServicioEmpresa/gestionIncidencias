namespace PideServicio.Application.Features.Tickets.Commands.ReanudarDesdeEspera;

using FluentValidation;

public sealed class ReanudarDesdeEsperaCommandValidator : AbstractValidator<ReanudarDesdeEsperaCommand>
{
    public ReanudarDesdeEsperaCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El ID del ticket es obligatorio.");
    }
}
