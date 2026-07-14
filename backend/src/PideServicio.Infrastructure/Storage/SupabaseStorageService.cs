namespace PideServicio.Infrastructure.Storage;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Infrastructure.Authentication;

/// <summary>
/// Implementa IStorageService usando la API REST de Supabase Storage.
/// No contiene lógica de negocio: validación de MIME/tamaño/extensión
/// es responsabilidad de la capa Application antes de invocar este servicio.
/// </summary>
public sealed class SupabaseStorageService : IStorageService
{
    private readonly StorageOptions _storageOptions;
    private readonly SupabaseAuthOptions _supabaseOptions;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SupabaseStorageService> _logger;

    public SupabaseStorageService(
        IOptions<StorageOptions> storageOptions,
        IOptions<SupabaseAuthOptions> supabaseOptions,
        HttpClient httpClient,
        ILogger<SupabaseStorageService> logger)
    {
        _storageOptions = storageOptions.Value;
        _supabaseOptions = supabaseOptions.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> SubirAsync(
        string bucket,
        string ruta,
        Stream contenido,
        string tipoContenido,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_supabaseOptions.Url}/storage/v1/object/{bucket}/{ruta}";

        using var content = new StreamContent(contenido);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(tipoContenido);

        using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        request.Headers.Add("Authorization", $"Bearer {_supabaseOptions.ServiceRoleKey}");
        request.Headers.Add("apikey", _supabaseOptions.AnonKey);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        _logger.LogInformation(
            "Archivo subido correctamente al bucket {Bucket}: {Ruta}",
            bucket, ruta);

        // Retorna la ruta relativa. La URL pública se construye con ObtenerUrlPublica.
        return ruta;
    }

    public async Task EliminarAsync(
        string bucket,
        string ruta,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_supabaseOptions.Url}/storage/v1/object/{bucket}/{ruta}";

        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Add("Authorization", $"Bearer {_supabaseOptions.ServiceRoleKey}");
        request.Headers.Add("apikey", _supabaseOptions.AnonKey);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Error al eliminar archivo {Ruta} del bucket {Bucket}: {Status}",
                    ruta, bucket, response.StatusCode);
            }
            else
            {
                _logger.LogInformation(
                    "Archivo eliminado del bucket {Bucket}: {Ruta}",
                    bucket, ruta);
            }
        }
        catch (Exception ex)
        {
            // Fallo en eliminación no interrumpe el flujo principal (BE-006 equivalente).
            _logger.LogError(
                ex,
                "Excepción al eliminar archivo {Ruta} del bucket {Bucket}",
                ruta, bucket);
        }
    }

    public string ObtenerUrlPublica(string bucket, string ruta)
        => $"{_supabaseOptions.Url}/storage/v1/object/public/{bucket}/{ruta}";
}
