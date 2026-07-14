# Tareas Pendientes

> Se actualiza tras cada tarea y commit. Nunca se elimina trabajo completado; se mueve a `completed.md`.

---

## FASE 6 — MÓDULOS BACKEND

### FASE 6.1 — ARQUITECTURA — COMPLETADA (2026-07-10)

Solución `backend/PideServicio.sln` con 6 proyectos en Clean Architecture (.NET 8). 72 archivos. QA APROBADO.

### FASE 6.2 — CAPA DOMAIN — COMPLETADA (2026-07-10)

20 entidades de dominio + 2 Value Objects + `docs/backend/DOMAIN_MODEL.md`. QA APROBADO (2 bloqueantes, 3 moderados, 2 menores — todos corregidos).

### FASE 6.3 — CAPA APPLICATION — COMPLETADA (2026-07-10)

169 archivos creados en `PideServicio.Application/Features/`. 11 módulos completos (Commands + Queries + DTOs + Mappings). Patrones: CQRS + MediatR 12.2, FluentValidation 11.9, Mapster 7.4, Result<T> discriminado con ResultTipo enum. 19 interfaces de repositorios. QA APROBADO.

### FASE 6.4 — CAPA INFRASTRUCTURE — COMPLETADA (2026-07-11)

19 repositorios (Persistence Layer) + 7 servicios de infraestructura + DI completo. `UnitOfWork` vía `NpgsqlTransaction`. `EntityReconstituter` helper. `DbConnectionFactory` actualizado a `NpgsqlDataSourceBuilder`. Optimistic locking en `UsuarioRepository`. `docs/backend/INFRASTRUCTURE.md` creado. QA APROBADO.

### FASE 6.5 — CAPA API — COMPLETADA (2026-07-11)

13 controllers (ApiControllerBase + 12 route controllers), 5 políticas de autorización JWT, Rate Limiting 100 req/min (built-in .NET 8), SwaggerGen con Bearer, API Versioning /api/v1/. `docs/backend/API.md` + `docs/backend/API_CONVENTIONS.md` creados. QA APROBADO.

### FASE 6.6 — INTEGRACIÓN COMPLETA — COMPLETADA (2026-07-11)

13 páginas frontend migradas de mocks a API real. Backend compilando (0 errores, net10.0). 57/57 handlers verificados. Auth hook skeleton creado. Docs QA: 84 casos de prueba, 9 limitaciones, guía de deployment.

---

## Bloqueantes para que la integración funcione realmente

### CRÍTICO: Auth Hook de Supabase (L001)
- El JWT de Supabase no incluye claims personalizados (usuario_id, rol, empresa_id)
- Sin esto, el endpoint /auth/me retorna 401 aunque el token sea válido
- Implementar en: supabase/functions/auth-hook/index.ts (skeleton disponible)

### Credenciales locales de Supabase
- Ejecutar `supabase start` y copiar valores a .env.local y appsettings.Development.json

### Catálogos pendientes
- Endpoints de tipos_servicio, categorias, motivos_rechazo, motivos_cancelacion no implementados
- CreateTicketPage usa valores estáticos temporalmente

### .NET SDK
- El proyecto fue diseñado para .NET 8 pero se migró a net10.0 (SDK 8 no disponible en la máquina de desarrollo)
- Decisión pendiente: mantener net10.0 o instalar SDK 8.0 LTS para producción

## FASE 7 — Optimización (pendiente)
## FASE 8 — Testing (pendiente)
## FASE 9 — Producción (pendiente)

---

## Arquitectura de Base de Datos — Fase 2 — Modelo Lógico (COMPLETADO — 2026-07-09)

- [x] `docs/database/MODEL-LOGICO.md` creado — 18 entidades persistentes, 2 entidades de solo lectura, 33 relaciones, 12 decisiones ML-DEC-001 a ML-DEC-012.
- [x] DUDA-ARQ-003 resuelta: ConfiguracionSLA es entidad propia con matriz empresa × tipo_servicio × prioridad.

**Dudas Arquitectónicas abiertas (no bloquearon Modelo Lógico, pendientes para Modelo Físico):**
- DUDA-ARQ-001: ¿Qué nivel de multi-tenancy adoptar? (row-level con `empresa_id` vs schema separado)
- DUDA-ARQ-002: ¿UUID v4 o ULID como PK?
- DUDA-ARQ-004: ¿Tabla unificada de historial o tablas separadas por entidad?
- DUDA-ARQ-005: ¿Soft-delete con `deleted_at` en todas las tablas o tablas de archivo separadas?
- DUDA-ARQ-006: ¿Las notificaciones se persisten en BD o solo se despachan en tiempo real?
- DUDA-ARQ-007: ¿El campo `área` en tickets referencia la tabla de áreas o es texto libre?

---

## Arquitectura de Base de Datos — Fase 3 — Modelo Físico (COMPLETADO — 2026-07-09)

- [x] `docs/database/MODEL-FISICO.md` creado — 18 tablas, 9 ENUMs, 2 secuencias, 8 funciones, 9 triggers, 5 vistas, RLS por rol, integración Supabase.
- [x] DUDA-ML-001 resuelta: MotivoRechazo = catálogo configurable (`motivos_rechazo`) + comentario libre. Decisión ML-DEC-013.

**Las tres fases del Modelo de BD están completas:**
- 3.31 Modelo Conceptual — COMPLETADO
- 3.32 Modelo Lógico — COMPLETADO
- 3.33 Modelo Físico — COMPLETADO

---

## Fase 4 — Generación de Migraciones SQL (EN CURSO — Iteración 4.10 completada)

**Estado:** EN CURSO — Migraciones 001-010 completadas (2026-07-10). Próxima: 011.

**Documentación de BD completa (no modificar):**
- `docs/database/MODEL-PHYSICAL.md` — Contrato definitivo (25 tablas, 9 ENUMs). CONGELADO.
- `docs/database/MIGRATION-PLAN.md` — Plan de migraciones (14 fases, 44 migraciones). APROBADO.
- `docs/database/NAMING_CONVENTIONS.md` — Convenciones de nombres (12 categorías). APROBADO.
- `docs/database/INDEX_STRATEGY.md` — Estrategia de índices (50+ índices). APROBADO.
- `docs/database/RLS_MATRIX.md` — Matriz de acceso por rol (23 tablas × 6 roles). APROBADA. NOTA: contiene 2 errores tipográficos detectados por QA en M-010 (tecnico_asignado_id → tecnico_id; empresa_id → filtro via sucursales en audit_logs). Pendiente corrección en tarea separada.

**Migraciones completadas (001-010):**
- [x] `supabase/migrations/001_initial_setup.sql` + `docs/database/MIGRATION-001.md` — APROBADA.
- [x] `supabase/migrations/002_master_catalogs.sql` + `docs/database/MIGRATION-002.md` — APROBADA.
- [x] `supabase/migrations/003_security_identity.sql` + `docs/database/MIGRATION-003.md` — QA APROBADO.
- [x] `supabase/migrations/004_organizational_structure.sql` + `docs/database/MIGRATION-004.md` — QA APROBADO.
- [x] `supabase/migrations/005_users.sql` + `docs/database/MIGRATION-005.md` — QA APROBADO.
- [x] `supabase/migrations/006_tickets.sql` + `docs/database/MIGRATION-006.md` — QA APROBADO (0 críticos).
- [x] `supabase/migrations/007_ticket_history.sql` + `docs/database/MIGRATION-007.md` — QA APROBADO.
- [x] `supabase/migrations/008_comments_evidence.sql` + `docs/database/MIGRATION-008.md` — QA APROBADO.
- [x] `supabase/migrations/009_audit_notifications.sql` + `docs/database/MIGRATION-009.md` — QA APROBADO (0 críticos).
- [x] `supabase/migrations/010_security_storage_rls.sql` + `docs/database/MIGRATION-010.md` — QA APROBADO (0 críticos, 4 menores).

### Migración 011 — sla_configuraciones + feriados — PENDIENTE
**Prioridad:** Alta (siguiente en la secuencia — última migración de tablas de negocio)
**Archivo SQL:** `supabase/migrations/011_sla_holidays.sql`
**Contenido:**
- Tabla `sla_configuraciones` (matriz empresa × tipo_servicio × prioridad con SLA_horas y tolerancia)
- Tabla `feriados` (por empresa/país, para cálculo de tiempo laboral real)
- Trigger `trg_sla_unique_check` (unicidad NULL-safe en sla_configuraciones)
- Trigger `trg_tickets_before_insert` (cálculo de SLA y fecha_limite al crear ticket)
- Activación del FK diferido: `tickets.sla_id → sla_configuraciones(id)` (diferido desde M-006)
- Completar protección de `sla_id` y fechas SLA en `trg_fn_tickets_guard_inmutable` (stub en M-010)
- RLS para las 2 nuevas tablas (sla_configuraciones + feriados)
- Seeds de configuración SLA inicial (si se requieren)
**Dependencias:** Migraciones 001-010 completadas

### Tareas restantes de la Fase 4:
- [x] Migraciones 001-010 — COMPLETADAS
- [ ] Migración 011: sla_configuraciones + feriados + FK diferida tickets.sla_id (SQL + Documentación)
- [ ] Conectar cliente Supabase JS al proyecto React (Fase 5).

---

## Módulo Tickets — CERRADO (2026-07-09)

**Módulo completamente especificado y listo para DATABASE.md.** Los 9 documentos fueron actualizados con las 3 decisiones finales del usuario el 2026-07-09.
Ubicación: `docs/modules/tickets/`

### Documentos actualizados y aprobados
- [x] `docs/modules/tickets/README.md` — ACTUALIZADO 2026-07-09
- [x] `docs/modules/tickets/DOMAIN.md` — ACTUALIZADO 2026-07-09 (836 → 890 líneas)
- [x] `docs/modules/tickets/LIFECYCLE.md` — ACTUALIZADO 2026-07-09
- [x] `docs/modules/tickets/BUSINESS-RULES.md` — ACTUALIZADO 2026-07-09 (82 reglas, 3 ciclos QA)
- [x] `docs/modules/tickets/PERMISSIONS.md` — ACTUALIZADO 2026-07-09 (33 acciones, 5 roles, 9 estados, 1415 → 1466 líneas)
- [x] `docs/modules/tickets/EVENTS.md` — ACTUALIZADO 2026-07-09 (21 eventos, 20 registros de auditoría)
- [x] `docs/modules/tickets/VALIDATIONS.md` — ACTUALIZADO 2026-07-09 (39 validaciones, 19 casos especiales, 48 mensajes)
- [x] `docs/modules/tickets/REVIEW.md` — ACTUALIZADO 2026-07-09 — veredicto APROBADO para DATABASE.md
- [x] `docs/modules/tickets/SUMMARY.md` — ACTUALIZADO 2026-07-09 (resumen ejecutivo con Técnico como actor)

### Documentos diferidos (no bloquean DATABASE.md)
- [ ] `docs/modules/tickets/FLOWS.md` — diferido
- [ ] `docs/modules/tickets/FUTURE.md` — diferido

### DUDAs que siguen abiertas (no bloquean DATABASE.md)
DUDA-TK-005, DUDA-TK-008, DUDA-TK-010, DUDA-TK-011, DUDA-PERM-001, DUDA-VAL-001, DUDA-VAL-003, DUDA-VAL-004.

### Próxima tarea del módulo
- [ ] `docs/modules/tickets/DATABASE.md` — **PENDIENTE DE APROBACIÓN EXPRESA DEL USUARIO para iniciar.**

---

## Bloqueantes activos

**1. COMMIT PENDIENTE DE AUTORIZACIÓN:**
Los cambios de las iteraciones 3.5, 3.6, 3.7, 3.8, 3.9, 3.10, 3.11, 3.12, 3.13, 3.14 y 3.15 están implementados en el árbol de trabajo pero NO commiteados.
**NO ejecutar git add, git commit ni git push sin autorización expresa del usuario.**
El usuario debe indicar explícitamente "haz el commit" o "autorizo el commit" para proceder.
La iteración 3.16 (documentación) no requiere commit propio.

**2. METODOLOGÍA DOCUMENT-FIRST OBLIGATORIA (vigente desde iteración 3.16):**
Toda implementación de módulo requiere completar y aprobar la documentación previa en `docs/modules/<modulo>/`.
El orden es: Dominio → Reglas → Flujos → Permisos → Eventos → Validaciones → Aprobación → DB → API → Backend.
**NO escribir código de implementación sin documentación aprobada del módulo.**

**BLOQUEANTE RESUELTO (Iteración 3.35):** Las 7 Dudas Arquitectónicas de MODEL-PHYSICAL.md §13 fueron resueltas. Fase 4 está DESBLOQUEADA.

---

## Módulo Catálogos del Sistema — Pendiente decisión del usuario (BLOQUEANTE)

**Documentación de negocio aprobada QA (2026-07-08). Bloqueada en 6 DUDAs.**
Ubicación: `docs/modules/system-catalogs/`

- [ ] Usuario resuelve **DUDA-SC-001 (crítica — bloqueante para DATABASE.md y API.md):** ¿Los catálogos son globales al sistema (todos los módulos y empresas comparten el mismo conjunto de catálogos) o son específicos por empresa (cada empresa gestiona los suyos)? Afecta el esquema de tablas, RLS y todas las consultas del módulo.
- [ ] Usuario decide sobre **DUDA-SC-002:** ¿Puede un Administrador crear nuevos Tipos de Catálogo (añadir categorías de catálogo que no existían), o solo el SuperAdministrador puede hacerlo?
- [ ] Usuario decide sobre **DUDA-SC-003:** ¿Los estados del Ticket (Nuevo, Sin Asignar, Asignado, etc.) son configurables desde este módulo o son fijos en el código? Si son configurables, este módulo los gestiona. Si son fijos, no deben aparecer como catálogo editable.
- [ ] Usuario decide sobre **DUDA-SC-004:** ¿Puede el Administrador modificar o desactivar Elementos que tengan el campo "Valor de sistema" marcado como verdadero (elementos críticos para el funcionamiento del sistema)?
- [ ] Usuario decide sobre **DUDA-SC-005:** ¿Este módulo absorbe el módulo Tipos de Servicio ya documentado (pasando a ser un catálogo más dentro de Catálogos del Sistema) o ambos módulos coexisten de forma independiente?
- [ ] Usuario decide sobre **DUDA-SC-006:** ¿El reordenamiento de elementos dentro de un catálogo es por posición absoluta (el usuario especifica el número de posición exacto) o por posición relativa (solo se puede subir o bajar un lugar a la vez)?

**Una vez resueltas las DUDAs bloqueantes:**
- [ ] Crear `docs/modules/system-catalogs/DATABASE.md` — esquema de tablas, índices, RLS, triggers y seeds del módulo.
- [ ] Crear `docs/modules/system-catalogs/API.md` — contratos de Edge Functions y endpoints del módulo.
- [ ] **NO iniciar DATABASE.md ni API.md sin resolver DUDA-SC-001 como mínimo.**

---

## Módulo Tipos de Servicio — Pendiente decisión del usuario (BLOQUEANTE)

**Documentación de negocio aprobada QA (2026-07-08). Bloqueada en DUDA-TS-001.**

- [ ] Usuario resuelve **DUDA-TS-001 (crítica — bloqueante para DATABASE.md y API.md):** ¿Los tipos de servicio son globales al sistema (todos los módulos/empresas comparten el catálogo) o son específicos por empresa (cada empresa gestiona los suyos)? Afecta el esquema de tablas, RLS y todas las consultas del módulo.
- [ ] Usuario decide sobre **DUDA-TS-002:** ¿Puede un Administrador gestionar (crear, editar, activar/desactivar) tipos de servicio, o solo el SuperAdministrador?
- [ ] Usuario decide sobre **DUDA-TS-003:** ¿El tipo de servicio de un ticket es inmutable una vez creado el ticket, o puede modificarse?
- [ ] Usuario decide sobre **DUDA-TS-004:** ¿Qué ocurre al hacer borrado lógico de un tipo de servicio que tiene tickets activos asociados? ¿Bloqueo o advertencia con confirmación?
- [ ] Usuario decide sobre **DUDA-TS-005:** ¿El reordenamiento de tipos de servicio es absoluto (el usuario especifica la posición exacta) o relativo (solo se puede subir/bajar uno)?
- [ ] Usuario decide sobre **DUDA-TS-006:** ¿Un nuevo tipo de servicio puede crearse en estado Inactivo, o siempre comienza Activo?

**Una vez resuelta DUDA-TS-001:**
- [ ] Crear `docs/modules/service-types/DATABASE.md` — esquema de tablas, índices, RLS, triggers y seeds del módulo.
- [ ] Crear `docs/modules/service-types/API.md` — contratos de Edge Functions y endpoints del módulo.
- [ ] **NO iniciar DATABASE.md ni API.md sin resolver DUDA-TS-001.**

---

## Iteración 3.18 — Pendiente revisión y decisión del usuario

**Documentación de negocio del Módulo Empresas — pendiente aprobación:**
- [ ] Usuario revisa y aprueba los 8 archivos en `docs/modules/companies/` (README, DOMAIN, BUSINESS-RULES, PERMISSIONS, VALIDATIONS, EVENTS, FLOWS, FUTURE).
- [ ] Usuario resuelve **DUDA-CO-001 (crítica — bloqueante para DATABASE.md de todos los módulos):** ¿El sistema es mono-tenant (una instancia por empresa) o multi-tenant (múltiples empresas en una instancia)? El SDD menciona "una empresa" pero la arquitectura de permisos usa `empresa_id`. Afecta el diseño de DATABASE.md y API.md de todos los módulos.
- [ ] Usuario decide sobre **DUD-CO-001:** ¿Puede un Administrador editar el nombre, logo y colores de su empresa, o solo el SuperAdministrador?
- [ ] Usuario decide sobre **DUD-CO-004:** ¿Los tickets activos bloquean la desactivación de empresa o solo generan advertencia con confirmación opcional?
- [ ] Usuario decide sobre **QA-DUD-001:** ¿Se notifica a los administradores de la empresa cuando esta es desactivada?
- [ ] Usuario decide sobre **QA-DUD-002:** ¿Los cambios de identidad visual (logo, colores) generan `company.updated` o `company.config_updated`?

**Una vez aprobados los documentos de negocio y resuelta DUDA-CO-001:**
- [ ] Crear `docs/modules/companies/DATABASE.md` — esquema de tablas, índices, RLS, triggers y seeds del módulo Empresas.
- [ ] Crear `docs/modules/companies/API.md` — contratos de Edge Functions y endpoints del módulo Empresas.
- [ ] **NO iniciar DATABASE.md ni API.md sin aprobación explícita y sin resolver DUDA-CO-001.**

---

## Iteración 3.17 — Pendiente revisión y decisión del usuario

**Documentación de negocio del Módulo Usuarios — pendiente aprobación:**
- [ ] Usuario revisa y aprueba los 8 archivos en `docs/modules/users/` (README, DOMAIN, BUSINESS-RULES, PERMISSIONS, VALIDATIONS, EVENTS, FLOWS, FUTURE).
- [ ] Usuario decide sobre **CRIT-02:** ¿Puede un Administrador restablecer la contraseña de otro usuario? Contradicción documentada: PERMISSIONS.md dice sí; docs/architecture/permissions/README.md dice solo SuperAdministrador.

**Una vez aprobados los documentos de negocio:**
- [ ] Crear `docs/modules/users/DATABASE.md` — esquema de tablas, índices, RLS, triggers y seeds del módulo Usuarios.
- [ ] Crear `docs/modules/users/API.md` — contratos de Edge Functions y endpoints del módulo Usuarios.
- [ ] **NO iniciar DATABASE.md ni API.md sin aprobación explícita del usuario.**

**Después de los módulos Usuarios y Empresas completos:**
- [ ] Planificar y documentar el siguiente módulo (a definir por el usuario).

---

## Bloqueado — pendiente de decisión del usuario
- **DUDA-AR-001**: ¿Un área pertenece directamente a una Sucursal (Opción A) o puede pertenecer a una Empresa directamente sin sucursal (Opción B)?
  - Impacto: DATABASE.md y API.md del módulo Áreas no pueden crearse sin esta decisión.
  - Todos los documentos actuales asumen Opción A como supuesto de trabajo.

## Próximo módulo disponible
- Los módulos sin dependencia de DUDA-AR-001 pueden comenzarse si el usuario lo autoriza.
- El orden recomendado desde el SDD: Usuarios → Tickets → Dashboard → Notificaciones → Auditoría → Reportes.

---

## Iteraciones 3.5 → 3.15 — Implementadas, pendientes de commit
## Iteración 3.16 — Completada, sin commit requerido (solo documentación)
## Iteración 3.17 — Completada, sin commit requerido (solo documentación)
## Iteración 3.18 — Completada, sin commit requerido (solo documentación)
## Iteración 3.21 — Completada, sin commit requerido (solo documentación — módulo Áreas aprobado QA)

**Archivos creados en iteración 3.16:**
- `docs/README.md` — índice maestro de documentación
- `docs/METHODOLOGY.md` — metodología Document-First
- `docs/architecture/` (8 READMEs: frontend, backend, database, security, notifications, events, permissions, integrations)
- `docs/modules/` (15 módulos × 11 archivos = 165 archivos): tickets, users, notifications, dashboard, audit, reports, settings, companies, branches, areas, comments, attachments, assignments, service-types, permissions
- `docs/decisions/ADR-001 a ADR-009` — Architecture Decision Records

---

## Iteraciones 3.5 → 3.15 — Implementadas, pendientes de commit (detalle)

Los cambios están en el árbol de trabajo (working tree). Archivos modificados incluyen:

- `src/app/layouts/AppLayout.tsx` — ScrollToTop + título fallback "Pidde Servicio"
- `src/app/layouts/AuthLayout.tsx` — nombre de marca "Pidde Servicio" en panel desktop y mobile
- `src/app/components/AppSidebar.tsx` — nombre "Pidde Servicio", badge notif rojo bg-destructive, cap 99+
- `src/app/components/MobileNav.tsx` — migrado a grid-cols-5 (centrado perfecto), badge notif rojo bg-destructive, cap 99+
- `src/app/components/AppHeader.tsx` — badge notif rojo bg-destructive, cap 99+; Configuración restringida a isAdmin
- `src/styles/globals.css` — glow animations + ps-glow-card intensificado (0.45/0.3)
- `src/shared/ui/card.tsx` — borde azul semitransparente por defecto (border-primary/25)
- `src/features/auth/pages/LoginPage.tsx` — login premium + labels obligatorio/opcional completados
- `src/features/dashboard/pages/DashboardPage.tsx` — glow cards en todas las chart cards, "Ver todos" en todas las secciones
- `src/features/tickets/pages/MyTicketsPage.tsx` — sort fecha, filtros fecha, tabla PC, EditTicketSheet lateral, filtros mobile 2 cols, menú de acciones reestructurado por rol, historial de cambios Dialog, helper getAssignedBy, "Por:" en tarjeta mobile y tabla desktop, columna "F. creación"
- `src/features/tickets/pages/TicketDetailPage.tsx` — history tracking, PC layout, modal Cambiar Estado rediseñado con ps-glow-modal + StatusBadge + Check, ConfirmDialog cancelar ticket
- `src/features/tickets/pages/CreateTicketPage.tsx` — "Otros" condicional, evidencias, 2 cols PC
- `src/features/notifications/pages/NotificationsPage.tsx` — Undo, Gmail styles, 2 cols PC
- `src/features/profile/pages/ProfilePage.tsx` — foto FileReader, ojo passwords, grupos PC, badges de rol con colores semánticos, "Cambiar foto" eliminado, campo Usuario en col. izquierda, Apariencia movida a col. izquierda, Correo+Teléfono en grid
- `src/features/reports/pages/ReportsPage.tsx` — filtros (Desde/Hasta/Empresa/Estado), "Ver todo" en gráficos, AreaChart tendencia semanal, PieChart donut por estado
- `src/features/settings/pages/SettingsPage.tsx` — tooltips, 2 cols PC, labels obligatorio/opcional completados
- `src/features/users/pages/UsersPage.tsx` — campo Estado, nomenclatura, labels en modales, gestión de empresas y sucursales con toggles de activación y ConfirmDialogs
- `src/features/users/pages/UserEditPage.tsx` — ancho completo (sin max-w-xl), grids Correo+Teléfono y Empresa+Sucursal, props error en Inputs
- `src/features/notifications/core/` — 8 archivos nuevos (EventBus, Service, Dispatcher, Factory, Registry, Queue, Templates + index)
- `src/features/notifications/channels/` — 10 canales (Internal, Push, Email, SMS, WhatsApp, Teams, Slack, Discord, Telegram, Webhook) + index
- `src/features/notifications/store/` — preferences.store.ts + notifications.store.ts (nuevos)
- `src/features/notifications/hooks/` — useNotificationService, useNotificationPreferences, useNotificationPermission (nuevos)
- `src/features/notifications/components/` — PushPermissionBanner + NotificationPreferences (nuevos)
- `src/features/notifications/index.ts` — barrel re-exportando toda la feature (nuevo)
- `src/features/notifications/service-worker/README.md` — documentación del SW de push (nuevo)
- `src/features/settings/pages/SettingsPage.tsx` — sección "Preferencias de Notificaciones" integrada
- `.claude/notification-architecture.md` — documento de referencia de arquitectura (nuevo)
- `src/mocks/data.ts` — nomenclatura Empresa/Sucursal/Tipo de Servicio
- `src/constants/index.ts` — nomenclatura actualizada
- `src/shared/components/PageBreadcrumb.tsx` — NUEVO
- `src/shared/components/CommandMenu.tsx` — NUEVO
- `src/shared/components/PageSkeletons.tsx` — NUEVO
- `index.html` — title y apple-mobile-web-app-title actualizados a "Pidde Servicio"; favicon cambiado a /logo.jpeg

---

## Fase 4 — Base de Datos con Supabase (bloqueada — requiere aprobación explícita del usuario)

NO iniciar hasta:
1. Commit de iteraciones 3.5 → 3.15 autorizado y ejecutado.
2. Aprobación visual final del usuario.

### Trabajo a realizar
- [ ] Migraciones SQL: tablas usuarios, empresas, sucursales, tickets, comentarios, evidencias, notificaciones, auditoria
- [ ] Índices y claves foráneas
- [ ] Row Level Security (RLS) por rol
- [ ] Funciones y triggers PostgreSQL (historial, auditoría, contadores)
- [ ] Seeds de datos de prueba reales
- [ ] Supabase conectado al proyecto React

---

## Mejoras opcionales implementadas en 3.5/3.6 (ya resueltas)

- [x] Vista desktop del Ticket Detail (columna derecha con metadata + acciones) → ancho completo implementado
- [x] Filtro por sucursal en el Dashboard Admin → filtros empresa/sucursal implementados
- [x] Tabla en lugar de cards para la lista de tickets en desktop → tabla PC implementada en MyTicketsPage
- [x] Animaciones de transición entre rutas → ScrollToTop implementado
- [x] Sidebar colapsable en desktop → pendiente (no solicitado aún)
