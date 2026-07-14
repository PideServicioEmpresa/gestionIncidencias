namespace PideServicio.Application.Features.Auditoria.DTOs;

public sealed record AuditLogDto(
    Guid Id,
    Guid? EmpresaId,
    string Tabla,
    Guid RegistroId,
    string Accion,
    Guid? UsuarioId,
    string? ValoresAnteriores,
    string? ValoresNuevos,
    string? IpAddress,
    DateTimeOffset CreatedAt);
