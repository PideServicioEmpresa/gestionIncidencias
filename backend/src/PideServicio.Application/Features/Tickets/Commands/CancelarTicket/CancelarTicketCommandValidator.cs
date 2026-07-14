namespace PideServicio.Application.Features.Tickets.Commands.CancelarTicket;

using FluentValidation;

public sealed class CancelarTicketCommandValidator : AbstractValidator<CancelarTicketCommand>
{
    public CancelarTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El identificador del ticket es requerido.");

        RuleFor(x => x.MotivoCancelacionId)
            .NotEmpty().WithMessage("El motivo de cancelación es requerido.");
    }
}
