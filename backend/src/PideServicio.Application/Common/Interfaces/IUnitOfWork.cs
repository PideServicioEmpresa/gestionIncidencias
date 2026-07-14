namespace PideServicio.Application.Common.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    Task IniciarTransaccionAsync(CancellationToken ct = default);
    Task ConfirmarAsync(CancellationToken ct = default);
    Task RevertirAsync(CancellationToken ct = default);
}
