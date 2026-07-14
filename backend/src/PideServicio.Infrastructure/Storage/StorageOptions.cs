namespace PideServicio.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SeccionNombre = "Storage";

    public string UrlBase { get; init; } = string.Empty;

    /// <summary>Tamaño máximo por archivo en bytes. Por defecto 10 MB.</summary>
    public long TamanoMaximoBytes { get; init; } = 10 * 1024 * 1024;

    public string[] TiposImagenPermitidos { get; init; } =
        ["image/jpeg", "image/png", "image/webp"];

    public string[] TiposDocumentoPermitidos { get; init; } =
        ["application/pdf"];

    /// <summary>Buckets definidos en M-010. No modificar sin crear nueva migración.</summary>
    public static class Buckets
    {
        public const string Evidencias = "tickets-evidence";
        public const string Avatares = "avatars";
        public const string RecursosDelSistema = "system-assets";
        public const string ImagenesEmail = "email-images";
    }
}
