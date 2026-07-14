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
