namespace PideServicio.Application.Features.Tickets.Commands.CerrarTicket;

using FluentValidation;

public sealed class CerrarTicketCommandValidator : AbstractValidator<CerrarTicketCommand>
{
    public CerrarTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El identificador del ticket es requerido.");

        When(x => x.Valoracion.HasValue, () =>
        {
            RuleFor(x => x.Valoracion!.Value)
                .InclusiveBetween((byte)1, (byte)5)
                .WithMessage("La valoración debe estar entre 1 y 5.");
        });
    }
}
