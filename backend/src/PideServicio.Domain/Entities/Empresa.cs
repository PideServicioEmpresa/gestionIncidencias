namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Exceptions;

public sealed class Empresa : SoftDeletableEntity
{
    public string NombreComercial { get; private set; } = string.Empty;
    public string RazonSocial { get; private set; } = string.Empty;
    public string IdentificacionFiscal { get; private set; } = string.Empty;
    public string? LogoUrl { get; private set; }
    public string? ColorPrimario { get; private set; }
    public string? ColorSecundario { get; private set; }
    public string ZonaHoraria { get; private set; } = "America/Mexico_City";
    public bool Activa { get; private set; } = true;

    private Empresa() { }

    public static Empresa Crear(
        string nombreComercial,
        string razonSocial,
        string identificacionFiscal,
        string? zonaHoraria = null,
        Guid? creadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(nombreComercial))
            throw new ValidationException("NombreComercial", "El nombre comercial de la empresa es requerido.");
        if (string.IsNullOrWhiteSpace(razonSocial))
            throw new ValidationException("RazonSocial", "La razón social de la empresa es requerida.");
        if (string.IsNullOrWhiteSpace(identificacionFiscal))
            throw new ValidationException("IdentificacionFiscal", "La identificación fiscal de la empresa es requerida.");

        var ahora = DateTimeOffset.UtcNow;
        return new Empresa
        {
            Id = Guid.NewGuid(),
            NombreComercial = nombreComercial.Trim(),
            RazonSocial = razonSocial.Trim(),
            IdentificacionFiscal = identificacionFiscal.Trim().ToUpperInvariant(),
            ZonaHoraria = string.IsNullOrWhiteSpace(zonaHoraria) ? "America/Mexico_City" : zonaHoraria,
            Activa = true,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor
        };
    }

    public void Actualizar(
        string nombreComercial,
        string razonSocial,
        string zonaHoraria,
        string? logoUrl,
        string? colorPrimario,
        string? colorSecundario,
        Guid actualizadoPor)
    {
        ValidarNoEliminada();
        if (string.IsNullOrWhiteSpace(nombreComercial))
            throw new ValidationException("NombreComercial", "El nombre comercial es requerido.");
        if (string.IsNullOrWhiteSpace(razonSocial))
            throw new ValidationException("RazonSocial", "La razón social es requerida.");
        if (string.IsNullOrWhiteSpace(zonaHoraria))
            throw new ValidationException("ZonaHoraria", "La zona horaria es requerida.");
        ValidarColorHex(colorPrimario, nameof(ColorPrimario));
        ValidarColorHex(colorSecundario, nameof(ColorSecundario));

        NombreComercial = nombreComercial.Trim();
        RazonSocial = razonSocial.Trim();
        ZonaHoraria = zonaHoraria;
        LogoUrl = logoUrl;
        ColorPrimario = colorPrimario?.ToUpperInvariant();
        ColorSecundario = colorSecundario?.ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    public void ActualizarDatosVisuales(string? logoUrl, string? colorPrimario, string? colorSecundario, Guid actualizadoPor)
    {
        ValidarNoEliminada();
        ValidarColorHex(colorPrimario, nameof(ColorPrimario));
        ValidarColorHex(colorSecundario, nameof(ColorSecundario));

        LogoUrl = logoUrl;
        ColorPrimario = colorPrimario?.ToUpperInvariant();
        ColorSecundario = colorSecundario?.ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    public void Activar(Guid actualizadoPor)
    {
        ValidarNoEliminada();
        Activa = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    /// <summary>
    /// Solo SuperAdmin puede desactivar. La Application layer valida tickets activos antes de llamar este método.
    /// </summary>
    public void Desactivar(Guid actualizadoPor)
    {
        ValidarNoEliminada();
        Activa = false;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actualizadoPor;
    }

    private void ValidarNoEliminada()
    {
        if (IsDeleted)
            throw new ValidationException("Estado", "No se puede modificar una empresa eliminada.");
    }

    private static void ValidarColorHex(string? color, string campo)
    {
        if (color is null) return;
        if (color.Length != 7 || color[0] != '#')
            throw new ValidationException(campo, $"El color '{color}' debe tener formato hexadecimal #RRGGBB.");
    }
}
