namespace PideServicio.Contracts.Common;

public sealed record ApiError(
    string Codigo,
    string Mensaje,
    IReadOnlyDictionary<string, string[]>? ErroresValidacion = null
);
