namespace PideServicio.Domain.Entities;

using System.Globalization;
using PideServicio.Domain.Common;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Exceptions;

/// <summary>
/// Parámetro de configuración del sistema. empresa_id nullable = parámetro global.
/// Los parámetros no se eliminan lógicamente, solo se actualizan.
/// </summary>
public sealed class Parametro : BaseEntity
{
    public Guid? EmpresaId { get; private set; }
    public string Clave { get; private set; } = string.Empty;
    public string Valor { get; private set; } = string.Empty;
    public TipoDatoParametroTipo TipoDato { get; private set; }
    public string? Descripcion { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public bool EsGlobal => EmpresaId is null;

    private Parametro() { }

    /// <summary>
    /// Crea un nuevo parámetro. Se usa cuando el PUT llega para una clave que aún no existe en BD.
    /// El tipo de dato se infiere del valor: boolean → BOOLEANO, entero → ENTERO,
    /// JSON literal → JSON, cualquier otro → TEXTO.
    /// </summary>
    public static Parametro CrearNuevo(
        string clave,
        string valor,
        Guid? empresaId,
        string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(clave))
            throw new ValidationException("Clave", "La clave del parámetro no puede estar vacía.");
        if (string.IsNullOrWhiteSpace(valor))
            throw new ValidationException("Valor", "El valor del parámetro no puede estar vacío.");

        var now = DateTimeOffset.UtcNow;
        return new Parametro
        {
            Id          = Guid.NewGuid(),
            Clave       = clave.Trim().ToUpperInvariant(),
            Valor       = valor.Trim(),
            TipoDato    = InferirTipoDato(valor.Trim()),
            Descripcion = descripcion,
            EmpresaId   = empresaId,
            CreatedAt   = now,
            UpdatedAt   = now,
            UpdatedBy   = null
        };
    }

    private static TipoDatoParametroTipo InferirTipoDato(string valor)
    {
        if (bool.TryParse(valor, out _))
            return TipoDatoParametroTipo.BOOLEANO;

        if (long.TryParse(valor, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out _))
            return TipoDatoParametroTipo.ENTERO;

        var trimmed = valor.Trim();
        if ((trimmed.StartsWith('{') && trimmed.EndsWith('}')) ||
            (trimmed.StartsWith('[') && trimmed.EndsWith(']')))
            return TipoDatoParametroTipo.JSON;

        return TipoDatoParametroTipo.TEXTO;
    }

    public void ActualizarValor(string nuevoValor, Guid actualizadoPor)
    {
        if (string.IsNullOrWhiteSpace(nuevoValor))
            throw new ValidationException("Valor", $"El valor del parámetro '{Clave}' no puede estar vacío.");

        Valor = nuevoValor.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    public T? ObtenerValorTipado<T>() where T : IConvertible
    {
        try { return (T)Convert.ChangeType(Valor, typeof(T), CultureInfo.InvariantCulture); }
        catch { return default; }
    }
}
