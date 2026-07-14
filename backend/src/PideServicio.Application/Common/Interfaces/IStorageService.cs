namespace PideServicio.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> SubirAsync(string bucket, string ruta, Stream contenido, string tipoContenido, CancellationToken cancellationToken = default);
    Task EliminarAsync(string bucket, string ruta, CancellationToken cancellationToken = default);
    string ObtenerUrlPublica(string bucket, string ruta);
}
