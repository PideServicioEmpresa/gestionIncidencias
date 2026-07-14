namespace PideServicio.Application.Common.Models;

public sealed class ListResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Total => Items.Count;

    public static ListResult<T> Vacio() => new() { Items = [] };

    public static ListResult<T> Crear(IReadOnlyList<T> items) => new() { Items = items };
}
