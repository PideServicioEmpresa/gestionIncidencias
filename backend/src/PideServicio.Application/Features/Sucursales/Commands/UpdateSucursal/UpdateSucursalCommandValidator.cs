namespace PideServicio.Application.Features.Sucursales.Commands.UpdateSucursal;

using FluentValidation;

public sealed class UpdateSucursalCommandValidator : AbstractValidator<UpdateSucursalCommand>
{
    public UpdateSucursalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador de la sucursal es requerido.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la sucursal es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres.")
            .When(x => x.Descripcion is not null);

        RuleFor(x => x.Direccion)
            .MaximumLength(500).WithMessage("La dirección no puede exceder 500 caracteres.")
            .When(x => x.Direccion is not null);
    }
}
