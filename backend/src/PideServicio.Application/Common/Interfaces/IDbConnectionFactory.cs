namespace PideServicio.Application.Common.Interfaces;

using System.Data;

public interface IDbConnectionFactory
{
    IDbConnection CrearConexion();
    Task<IDbConnection> CrearConexionAsync(CancellationToken cancellationToken = default);
}
