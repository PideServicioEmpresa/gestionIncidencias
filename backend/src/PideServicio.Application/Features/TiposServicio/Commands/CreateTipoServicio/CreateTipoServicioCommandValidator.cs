namespace PideServicio.Application.Features.TiposServicio.Commands.CreateTipoServicio;

using FluentValidation;

public sealed class CreateTipoServicioCommandValidator : AbstractValidator<CreateTipoServicioCommand>
{
    public CreateTipoServicioCommandValidator()
    {
        RuleFor(x => x.EmpresaId).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Orden).GreaterThan(0);
        RuleFor(x => x.Descripcion).MaximumLength(500).When(x => x.Descripcion is not null);
    }
}
