namespace PideServicio.Infrastructure.Caching;

/// <summary>
/// Opciones de caché leídas desde appsettings.json sección "Cache".
/// Redis está preparado estructuralmente pero no habilitado en MVP.
/// </summary>
public sealed class CacheOptions
{
    public const string SeccionNombre = "Cache";

    /// <summary>Activa IMemoryCache en proceso. Por defecto habilitado.</summary>
    public bool MemoriaCacheHabilitada { get; init; } = true;

    /// <summary>Activa Redis distribuido. Requiere RedisConnectionString. MVP: false.</summary>
    public bool RedisCacheHabilitado { get; init; } = false;

    /// <summary>Connection string de Redis. Solo se usa si RedisCacheHabilitado = true.</summary>
    public string? RedisConnectionString { get; init; }

    /// <summary>Duración predeterminada de entradas en caché (segundos). Por defecto 5 minutos.</summary>
    public int DuracionDefaultSegundos { get; init; } = 300;
}
