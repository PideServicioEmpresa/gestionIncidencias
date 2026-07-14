namespace PideServicio.Domain.ValueObjects;

using PideServicio.Domain.Exceptions;

/// <summary>
/// Código único de ticket generado por trigger de BD. Máximo 10 caracteres.
/// El dominio lo protege como valor inmutable.
/// </summary>
public sealed record TicketCodigo
{
    public string Valor { get; }

    private TicketCodigo(string valor) => Valor = valor;

    public static TicketCodigo Crear(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ValidationException("TicketCodigo", "El código de ticket es requerido.");

        codigo = codigo.Trim().ToUpperInvariant();

        if (codigo.Length > 10)
            throw new ValidationException("TicketCodigo", "El código de ticket no puede exceder 10 caracteres.");

        return new TicketCodigo(codigo);
    }

    public static implicit operator string(TicketCodigo codigo) => codigo.Valor;
    public override string ToString() => Valor;
}
