namespace PideServicio.Application.Features.Tickets.Commands.ReabrirTicket;

using FluentValidation;

public sealed class ReabrirTicketCommandValidator : AbstractValidator<ReabrirTicketCommand>
{
    public ReabrirTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El identificador del ticket es requerido.");

        RuleFor(x => x.MotivoRechazoId)
            .NotEmpty().WithMessage("El motivo de rechazo es requerido.");
    }
}
