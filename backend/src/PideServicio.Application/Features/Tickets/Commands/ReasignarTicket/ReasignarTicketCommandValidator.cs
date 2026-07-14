namespace PideServicio.Application.Features.Tickets.Commands.ReasignarTicket;

using FluentValidation;

public sealed class ReasignarTicketCommandValidator : AbstractValidator<ReasignarTicketCommand>
{
    public ReasignarTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El identificador del ticket es requerido.");

        RuleFor(x => x.NuevoTecnicoId)
            .NotEmpty().WithMessage("El identificador del nuevo técnico es requerido.");
    }
}
