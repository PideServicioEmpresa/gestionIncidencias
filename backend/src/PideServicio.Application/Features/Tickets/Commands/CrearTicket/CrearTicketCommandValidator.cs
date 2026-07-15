namespace PideServicio.Application.Features.Tickets.Commands.CrearTicket;

using FluentValidation;

public sealed class CrearTicketCommandValidator : AbstractValidator<CrearTicketCommand>
{
    public CrearTicketCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título es requerido.")
            .MaximumLength(300).WithMessage("El título no puede exceder 300 caracteres.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es requerida.");

        RuleFor(x => x.SucursalId)
            .NotEmpty().WithMessage("La sucursal es requerida.");

        RuleFor(x => x.AreaNombre)
            .NotEmpty().WithMessage("El área es requerida.")
            .MaximumLength(150).WithMessage("El nombre del área no puede exceder 150 caracteres.");

        RuleFor(x => x.TipoServicioId)
            .NotEmpty().WithMessage("El tipo de servicio es requerido.");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("La categoría es requerida.");
    }
}
