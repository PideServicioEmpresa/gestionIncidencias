namespace PideServicio.Persistence.Options;

public sealed class DatabaseOptions
{
    public const string SeccionNombre = "Database";

    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>Tiempo máximo de espera por comando SQL en segundos.</summary>
    public int TiempoEsperaComando { get; init; } = 30;

    /// <summary>Intentos de reintento ante fallas transitorias de conexión.</summary>
    public int MaxReintentos { get; init; } = 3;
}
