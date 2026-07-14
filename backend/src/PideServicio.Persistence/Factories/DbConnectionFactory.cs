namespace PideServicio.Persistence.Factories;

using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Persistence.Options;

/// <summary>
/// Fábrica de conexiones a PostgreSQL basada en NpgsqlDataSource (pool de conexiones de Npgsql v8).
/// Se registra como Singleton para que el DataSource sea compartido por toda la aplicación.
/// </summary>
public sealed class DbConnectionFactory : IDbConnectionFactory, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public DbConnectionFactory(IOptions<DatabaseOptions> opciones)
    {
        var builder = new NpgsqlDataSourceBuilder(opciones.Value.ConnectionString);
        // Los repositorios convierten enums a string en INSERT/UPDATE
        // y leen columnas de enum como string en SELECT mediante alias explícitos.
        _dataSource = builder.Build();
    }

    public IDbConnection CrearConexion()
        => _dataSource.OpenConnection();

    public async Task<IDbConnection> CrearConexionAsync(CancellationToken cancellationToken = default)
        => await _dataSource.OpenConnectionAsync(cancellationToken);

    public async ValueTask DisposeAsync()
        => await _dataSource.DisposeAsync();
}
