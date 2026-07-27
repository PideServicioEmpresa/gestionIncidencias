namespace PideServicio.Application.Features.Usuarios.Commands.CreateUsuario;

using FluentValidation;
using PideServicio.Domain.Enums;

public sealed class CreateUsuarioCommandValidator : AbstractValidator<CreateUsuarioCommand>
{
    private static readonly RolTipo[] _rolesMultiSucursal = [RolTipo.TECNICO, RolTipo.TRABAJADOR, RolTipo.USUARIO];

    public CreateUsuarioCommandValidator()
    {
        RuleFor(x => x.SucursalId)
            .NotEmpty().WithMessage("La sucursal es requerida.")
            .When(x => _rolesMultiSucursal.Contains(x.Rol));

        RuleFor(x => x.EmpresaId)
            .NotEmpty().WithMessage("La empresa es requerida para el rol Administrador.")
            .When(x => x.Rol == RolTipo.ADMIN);

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.Apellido)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres.");

        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
            .MaximumLength(320).WithMessage("El correo electrónico no puede exceder 320 caracteres.");

        RuleFor(x => x.NombreUsuario)
            .NotEmpty().WithMessage("El nombre de usuario es requerido.")
            .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder 50 caracteres.")
            .Matches(@"^[a-z0-9_]{3,50}$")
            .WithMessage("El nombre de usuario solo puede contener letras minúsculas, números y guion bajo.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.");

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres.")
            .When(x => x.Telefono is not null);

        RuleFor(x => x.Rol)
            .IsInEnum().WithMessage("El rol especificado no es válido.");

        When(x => x.Sucursales is { Count: > 0 }, () =>
        {
            RuleFor(x => x.Sucursales)
                .Must(s => s!.Count(x => x.EsPrincipal) == 1)
                .WithMessage("Si especifica sucursales, debe marcar exactamente una como principal.");

            RuleForEach(x => x.Sucursales).ChildRules(s =>
                s.RuleFor(x => x.SucursalId)
                 .NotEmpty().WithMessage("El id de la sucursal no puede estar vacío."));
        });
    }
}
