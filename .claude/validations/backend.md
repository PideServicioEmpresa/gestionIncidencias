
# Checklist de Validación — Backend

> Verificar antes de dar por terminada cualquier tarea de Edge Functions / servicios. Responsable: `backend` + `reviewer`.

## Seguridad y permisos
- [ ] Datos nunca modificados directo desde React (vía Service/Edge Function). [BE-001]
- [ ] Permisos validados en Backend, no solo en Frontend. [SEC-001/002]
- [ ] Toda asignación valida permisos. [BE-003]
- [ ] Secretos en variables de entorno; nunca en el repositorio. [SEC-006/007]

## Workflow y datos
- [ ] Cambios de estado validan el workflow; no se saltan estados. [BE-005]
- [ ] Eliminación lógica (`deleted_at`), nunca `DELETE` físico. [BE-004]
- [ ] Cargas de archivos validan MIME, tamaño, extensión e integridad. [BE-006]

## Auditoría y notificaciones
- [ ] Todo cambio importante queda auditado (usuario, fecha, IP, dispositivo, valor anterior/nuevo). [BE-002/008]
- [ ] Correos enviados por cola; el usuario no espera el envío. [BE-007]
- [ ] Fallos de correo no rompen el proceso principal. [NT-003]

## Contrato
- [ ] Respuestas con el formato estándar (`success`, `message`, `data`/`code`/`errors`).
