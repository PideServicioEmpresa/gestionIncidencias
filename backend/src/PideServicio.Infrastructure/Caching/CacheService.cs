namespace PideServicio.Infrastructure.Caching;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

/// <summary>
/// Wrapper preparatorio sobre IMemoryCache. Centraliza la política de expiración
/// y facilita la sustitución por Redis distribuido en fases futuras.
/// Registrado como Singleton ya que IMemoryCache es thread-safe.
/// </summary>
public sealed class CacheService
{
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _options;

    public CacheService(IMemoryCache cache, IOptions<CacheOptions> options)
    {
        _cache = cache;
        _options = options.Value;
    }

    /// <summary>
    /// Obtiene un valor de caché. Retorna default(T) si no existe o expiró.
    /// </summary>
    public T? Obtener<T>(string clave) =>
        _cache.TryGetValue(clave, out T? valor) ? valor : default;

    /// <summary>
    /// Establece un valor en caché con duración opcional. Si no se especifica, usa DuracionDefaultSegundos.
    /// </summary>
    public void Establecer<T>(T valor, string clave, TimeSpan? duracion = null)
    {
        var expiracion = duracion ?? TimeSpan.FromSeconds(_options.DuracionDefaultSegundos);
        _cache.Set(clave, valor, expiracion);
    }

    /// <summary>
    /// Elimina una entrada de caché por clave.
    /// </summary>
    public void Remover(string clave) => _cache.Remove(clave);
}
