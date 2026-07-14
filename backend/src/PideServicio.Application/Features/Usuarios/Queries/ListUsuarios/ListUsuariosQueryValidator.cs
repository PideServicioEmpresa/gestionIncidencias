namespace PideServicio.Application.Features.Usuarios.Queries.ListUsuarios;

using FluentValidation;

public sealed class ListUsuariosQueryValidator : AbstractValidator<ListUsuariosQuery>
{
    public ListUsuariosQueryValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("El número de página debe ser mayor o igual a 1.");

        RuleFor(x => x.TamanoPagina)
            .InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100.");

        RuleFor(x => x.Rol)
            .IsInEnum().WithMessage("El rol especificado no es válido.")
            .When(x => x.Rol.HasValue);
    }
}
