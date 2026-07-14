namespace PideServicio.Application.Features.Empresas.Queries.ListEmpresas;

using FluentValidation;

public sealed class ListEmpresasQueryValidator : AbstractValidator<ListEmpresasQuery>
{
    public ListEmpresasQueryValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("La página debe ser mayor o igual a 1.");

        RuleFor(x => x.TamanoPagina)
            .InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100.");
    }
}
