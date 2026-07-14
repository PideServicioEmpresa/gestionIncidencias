namespace PideServicio.Application.Features.TiposServicio.Commands.UpdateTipoServicio;

using FluentValidation;

public sealed class UpdateTipoServicioCommandValidator : AbstractValidator<UpdateTipoServicioCommand>
{
    public UpdateTipoServicioCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Orden).GreaterThan(0);
        RuleFor(x => x.Descripcion).MaximumLength(500).When(x => x.Descripcion is not null);
    }
}
