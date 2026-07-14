namespace PideServicio.Persistence.UnitOfWork;

using Npgsql;
using PideServicio.Application.Common.Interfaces;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _factory;
    private NpgsqlConnection? _conexion;
    private NpgsqlTransaction? _transaccion;

    public UnitOfWork(IDbConnectionFactory factory) => _factory = factory;

    public async Task IniciarTransaccionAsync(CancellationToken ct = default)
    {
        _conexion = (NpgsqlConnection)await _factory.CrearConexionAsync(ct);
        _transaccion = await _conexion.BeginTransactionAsync(ct);
    }

    public async Task ConfirmarAsync(CancellationToken ct = default)
    {
        if (_transaccion is null)
            throw new InvalidOperationException("No hay transacción activa.");
        await _transaccion.CommitAsync(ct);
    }

    public async Task RevertirAsync(CancellationToken ct = default)
    {
        if (_transaccion is null)
            throw new InvalidOperationException("No hay transacción activa.");
        await _transaccion.RollbackAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaccion is not null)
        {
            await _transaccion.DisposeAsync();
            _transaccion = null;
        }
        if (_conexion is not null)
        {
            await _conexion.DisposeAsync();
            _conexion = null;
        }
    }
}
