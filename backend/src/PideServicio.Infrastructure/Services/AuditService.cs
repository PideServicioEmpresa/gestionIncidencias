namespace PideServicio.Infrastructure.Services;

using System.Text.Json;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Domain.Entities;

public sealed class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUsuarioRepository _usuarioRepository;

    public AuditService(
        IAuditLogRepository auditRepo,
        ICurrentUserService currentUser,
        IUsuarioRepository usuarioRepository)
    {
        _auditRepo = auditRepo;
        _currentUser = currentUser;
        _usuarioRepository = usuarioRepository;
    }

    public async Task RegistrarAsync(
        string entidad,
        Guid entidadId,
        string accion,
        object? antes,
        object? despues,
        CancellationToken cancellationToken = default)
    {
        var claims = _currentUser.UsuarioActual;

        Guid? actorId = null;
        Guid empresaId = Guid.Empty;

        if (claims is not null)
        {
            if (claims.Id != Guid.Empty)
            {
                actorId = claims.Id;
                empresaId = claims.EmpresaId;
            }
            else if (claims.AuthId != Guid.Empty)
            {
                // Fallback: hook no activo, resolver desde BD
                var actorDb = await _usuarioRepository.ObtenerPorAuthIdAsync(claims.AuthId, cancellationToken);
                if (actorDb is not null)
                {
                    actorId = actorDb.Id;
                    empresaId = actorDb.EmpresaId;
                }
            }
        }

        var log = AuditLog.Crear(
            empresaId: empresaId,
            tabla: entidad,
            registroId: entidadId,
            accion: accion,
            usuarioId: actorId,
            valoresAnteriores: antes is null ? null : JsonSerializer.Serialize(antes),
            valoresNuevos: despues is null ? null : JsonSerializer.Serialize(despues));

        await _auditRepo.CrearAsync(log, cancellationToken);
    }
}
