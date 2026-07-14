namespace PideServicio.Application.Features.Empresas.Commands.UpdateEmpresa;

using FluentValidation;

public sealed class UpdateEmpresaCommandValidator : AbstractValidator<UpdateEmpresaCommand>
{
    public UpdateEmpresaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador de la empresa es requerido.");

        RuleFor(x => x.NombreComercial)
            .NotEmpty().WithMessage("El nombre comercial es requerido.")
            .MaximumLength(200).WithMessage("El nombre comercial no puede exceder 200 caracteres.");

        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("La razón social es requerida.")
            .MaximumLength(200).WithMessage("La razón social no puede exceder 200 caracteres.");

        RuleFor(x => x.ZonaHoraria)
            .NotEmpty().WithMessage("La zona horaria es requerida.")
            .MaximumLength(100).WithMessage("La zona horaria no puede exceder 100 caracteres.");

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500).WithMessage("La URL del logo no puede exceder 500 caracteres.")
            .When(x => x.LogoUrl is not null);

        RuleFor(x => x.ColorPrimario)
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("El color primario debe tener formato hexadecimal #RRGGBB.")
            .When(x => x.ColorPrimario is not null);

        RuleFor(x => x.ColorSecundario)
            .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("El color secundario debe tener formato hexadecimal #RRGGBB.")
            .When(x => x.ColorSecundario is not null);
    }
}
