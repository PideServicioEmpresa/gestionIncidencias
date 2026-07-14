namespace PideServicio.Application.Features.Tickets.Commands.CambiarArea;

using FluentValidation;

public sealed class CambiarAreaCommandValidator : AbstractValidator<CambiarAreaCommand>
{
    public CambiarAreaCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El identificador del ticket es requerido.");

        RuleFor(x => x.NuevaAreaId)
            .NotEmpty().WithMessage("El identificador del área es requerido.");
    }
}
