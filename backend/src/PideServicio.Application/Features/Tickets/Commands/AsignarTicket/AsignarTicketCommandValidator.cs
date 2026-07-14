namespace PideServicio.Application.Features.Tickets.Commands.AsignarTicket;

using FluentValidation;

public sealed class AsignarTicketCommandValidator : AbstractValidator<AsignarTicketCommand>
{
    public AsignarTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El identificador del ticket es requerido.");

        RuleFor(x => x.TecnicoId)
            .NotEmpty().WithMessage("El identificador del técnico es requerido.");
    }
}
