namespace PideServicio.Domain.ValueObjects;

using PideServicio.Domain.Exceptions;

public sealed record Email
{
    public string Valor { get; }

    private Email(string valor) => Valor = valor;

    public static Email Crear(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
            throw new ValidationException("Email", "El correo electrónico es requerido.");

        correo = correo.Trim().ToLowerInvariant();

        if (correo.Length > 320)
            throw new ValidationException("Email", "El correo electrónico no puede exceder 320 caracteres.");

        var posicionArroba = correo.IndexOf('@');
        var ultimoPunto = correo.LastIndexOf('.');
        if (posicionArroba <= 0
            || posicionArroba == correo.Length - 1
            || ultimoPunto <= posicionArroba
            || ultimoPunto == correo.Length - 1)
            throw new ValidationException("Email", $"'{correo}' no es un correo electrónico válido.");

        return new Email(correo);
    }

    public static implicit operator string(Email email) => email.Valor;
    public override string ToString() => Valor;
}
