namespace PideServicio.Contracts.Common;

public sealed record PaginacionParams
{
    public int Pagina { get; init; }
    public int TamanoPagina { get; init; }
    public int Desplazamiento => (Pagina - 1) * TamanoPagina;

    public PaginacionParams(int pagina = 1, int tamanoPagina = 20)
    {
        Pagina = Math.Max(1, pagina);
        TamanoPagina = Math.Clamp(tamanoPagina, 1, 100);
    }
}
