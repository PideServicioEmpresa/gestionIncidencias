namespace PideServicio.Application.Features.Sucursales.Queries.ListSucursales;

using FluentValidation;

public sealed class ListSucursalesQueryValidator : AbstractValidator<ListSucursalesQuery>
{
    public ListSucursalesQueryValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("La página debe ser mayor o igual a 1.");

        RuleFor(x => x.TamanoPagina)
            .InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100.");
    }
}
