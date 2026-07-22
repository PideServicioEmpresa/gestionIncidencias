namespace PideServicio.Application.Features.Usuarios.Commands.ActualizarSucursalesUsuario;

using FluentValidation;

public sealed class ActualizarSucursalesUsuarioCommandValidator
    : AbstractValidator<ActualizarSucursalesUsuarioCommand>
{
    public ActualizarSucursalesUsuarioCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("El id del usuario es requerido.");

        RuleFor(x => x.Sucursales)
            .NotEmpty().WithMessage("Debe asignar al menos una sucursal.");

        RuleFor(x => x.Sucursales)
            .Must(s => s != null && s.Count(x => x.EsPrincipal) == 1)
            .WithMessage("Debe marcar exactamente una sucursal como principal.")
            .When(x => x.Sucursales?.Count > 0);

        RuleForEach(x => x.Sucursales).ChildRules(s =>
            s.RuleFor(x => x.SucursalId)
             .NotEmpty().WithMessage("El id de la sucursal no puede estar vacío."));
    }
}
