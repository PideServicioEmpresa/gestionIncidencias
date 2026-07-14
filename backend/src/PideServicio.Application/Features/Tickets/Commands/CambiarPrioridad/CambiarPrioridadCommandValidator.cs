namespace PideServicio.Application.Features.Tickets.Commands.CambiarPrioridad;

using FluentValidation;

public sealed class CambiarPrioridadCommandValidator : AbstractValidator<CambiarPrioridadCommand>
{
    public CambiarPrioridadCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El identificador del ticket es requerido.");
    }
}
