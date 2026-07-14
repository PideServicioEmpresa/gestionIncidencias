namespace PideServicio.Contracts.Common;

public sealed class ApiResponse
{
    public bool Exitoso { get; init; }
    public string? TraceId { get; init; }
    public ApiError? Error { get; init; }

    public static ApiResponse Ok(string? traceId = null) =>
        new() { Exitoso = true, TraceId = traceId };

    public static ApiResponse Fallo(ApiError error, string? traceId = null) =>
        new() { Exitoso = false, Error = error, TraceId = traceId };
}
