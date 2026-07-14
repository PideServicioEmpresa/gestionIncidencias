namespace PideServicio.Application.Common.Interfaces;

public interface IAuditService
{
    Task RegistrarAsync(
        string entidad,
        Guid entidadId,
        string accion,
        object? antes,
        object? despues,
        CancellationToken cancellationToken = default);
}
