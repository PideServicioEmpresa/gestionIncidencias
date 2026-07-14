namespace PideServicio.Domain.Enums;

/// <summary>
/// Espejo del ENUM ticket_estado_tipo definido en PostgreSQL.
/// Flujo principal: NUEVO → SIN_ASIGNAR → ASIGNADO → EN_PROCESO → PENDIENTE_VALIDACION → CERRADO.
/// Flujo de reapertura: PENDIENTE_VALIDACION → REABIERTO → ASIGNADO.
/// </summary>
public enum TicketEstadoTipo
{
    NUEVO,
    SIN_ASIGNAR,
    ASIGNADO,
    EN_PROCESO,
    EN_ESPERA,
    PENDIENTE_VALIDACION,
    REABIERTO,
    CERRADO,
    CANCELADO
}
