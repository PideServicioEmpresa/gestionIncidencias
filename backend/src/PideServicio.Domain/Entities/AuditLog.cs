namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;

/// <summary>
/// Registro de auditoría inmutable. Se inserta, nunca se modifica ni elimina.
/// BD almacena los valores como jsonb; en dominio se trabajan como strings serializados.
/// </summary>
public sealed class AuditLog : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Tabla { get; private set; } = string.Empty;
    public Guid RegistroId { get; private set; }
    public string Accion { get; private set; } = string.Empty;
    public Guid? UsuarioId { get; private set; }
    public string? ValoresAnteriores { get; private set; }
    public string? ValoresNuevos { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private AuditLog() { }

    public static AuditLog Crear(
        Guid empresaId,
        string tabla,
        Guid registroId,
        string accion,
        Guid? usuarioId,
        string? valoresAnteriores = null,
        string? valoresNuevos = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            Tabla = tabla,
            RegistroId = registroId,
            Accion = accion.ToUpperInvariant(),
            UsuarioId = usuarioId,
            ValoresAnteriores = valoresAnteriores,
            ValoresNuevos = valoresNuevos,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
