namespace PideServicio.Domain.Enums;

/// <summary>Espejo del ENUM tipo_evento_historial_tipo definido en PostgreSQL.</summary>
public enum TipoEventoHistorialTipo
{
    CREADO,
    ESTADO_CAMBIADO,
    ASIGNADO,
    REASIGNADO,
    COMENTADO,
    EVIDENCIA_SUBIDA,
    PRIORIDAD_CAMBIADA,
    AREA_CAMBIADA,
    SLA_MODIFICADO_MANUAL,
    DATOS_ACTUALIZADOS
}
