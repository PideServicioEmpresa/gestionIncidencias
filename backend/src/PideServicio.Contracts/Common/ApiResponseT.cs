namespace PideServicio.Contracts.Common;

public sealed class ApiResponse<T>
{
    public bool Exitoso { get; init; }
    public T? Datos { get; init; }
    public string? TraceId { get; init; }
    public ApiError? Error { get; init; }

    public static ApiResponse<T> Ok(T datos, string? traceId = null) =>
        new() { Exitoso = true, Datos = datos, TraceId = traceId };

    public static ApiResponse<T> Fallo(ApiError error, string? traceId = null) =>
        new() { Exitoso = false, Error = error, TraceId = traceId };
}
