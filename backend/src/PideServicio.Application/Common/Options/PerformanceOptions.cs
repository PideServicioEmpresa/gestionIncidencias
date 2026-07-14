namespace PideServicio.Application.Common.Options;

public sealed class PerformanceOptions
{
    public const string SeccionNombre = "Performance";

    /// <summary>Tiempo máximo en ms antes de loggear una solicitud como lenta. Por defecto 500 ms.</summary>
    public int UmbralAlertaMs { get; init; } = 500;
}
