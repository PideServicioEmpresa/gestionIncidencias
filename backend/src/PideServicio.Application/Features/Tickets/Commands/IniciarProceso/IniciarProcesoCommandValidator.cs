namespace PideServicio.Application.Features.Tickets.Commands.IniciarProceso;

using FluentValidation;

public sealed class IniciarProcesoCommandValidator : AbstractValidator<IniciarProcesoCommand>
{
    public IniciarProcesoCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El ID del ticket es obligatorio.");
    }
}
