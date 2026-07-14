# Fase Actual

## Fase Actual: 7.0 EN CURSO — Hardening y Estabilización RC

**Estado:** Auditoría completada, correcciones aplicadas, bloqueantes identificados.

**Completado:** Auditoría exhaustiva por 8 agentes. 35+ correcciones aplicadas.

**Bloqueantes para UAT/RC (en orden de prioridad):**
1. L001 — Supabase Auth Hook (CRÍTICO — bloquea todo el flujo de auth)
2. 5 endpoints de catálogos (GET /sucursales, /areas, /tipos-servicio, /categorias, GET /usuarios?rol=TECNICO)
3. Protección de rutas por rol en el frontend
4. notifications.store con MOCK_NOTIFICATIONS
5. AppHeader.logout sin authService.logout()
6. SettingsPage ignorando parámetros del backend
7. Estados en_espera/cancelado mapeados incorrectamente

**Porcentaje preparación producción:** 62%
**¿Apto para UAT?** NO
**¿Apto para RC v1.0?** NO

---

## Fase anterior: FASE 6.5 COMPLETADA — Capa API del Backend — QA APROBADO

**Fecha:** 2026-07-11

### Resumen de lo creado

Capa API completa del backend .NET 8 (Clean Architecture). Todos los endpoints REST del sistema implementados con autorización JWT, rate limiting y documentación Swagger.

**Controllers — 13 archivos en `backend/src/PideServicio.Api/Controllers/`:**
- `Common/ApiControllerBase.cs` — Mediator lazy, TraceId, HandleResult, HandleCreated, OkPaged, MapFailure
- `V1/Auth/AuthController.cs` — GET /api/v1/auth/me (ICurrentUserService)
- `V1/Empresas/EmpresasController.cs` — CRUD + ToggleActiva (5 endpoints)
- `V1/Sucursales/SucursalesController.cs` — CRUD + ToggleActiva (5 endpoints)
- `V1/Areas/AreasController.cs` — CRUD + ToggleActiva (5 endpoints)
- `V1/Roles/RolesController.cs` — GET roles, permisos, permisosPorRol (3 endpoints)
- `V1/Configuracion/ConfiguracionController.cs` — GET parametros, PUT por clave (2 endpoints)
- `V1/Usuarios/UsuariosController.cs` — CRUD + CambiarRol + CambiarEstadoLaboral + Activar + Desactivar (9 endpoints)
- `V1/Tickets/TicketsController.cs` — 15 endpoints (CRUD + 10 transiciones de estado)
- `V1/Tickets/ComentariosController.cs` — CRUD comentarios anidados en ticket (4 endpoints)
- `V1/Tickets/EvidenciasController.cs` — GET list + POST multipart + DELETE (3 endpoints)
- `V1/Notificaciones/NotificacionesController.cs` — Listar + Conteo + MarcarLeida + MarcarTodasLeidas (4 endpoints)
- `V1/Auditoria/AuditoriaController.cs` — Listar logs con filtros (1 endpoint)

**Extensions modificados:**
- `Extensions/ServiceCollectionExtensions.cs` — SwaggerGen con JWT Bearer, 5 políticas de autorización (Autenticado / SoloSuperAdmin / AdminOSuperior / SupervisorOSuperior / Tecnico), Rate Limiting 100 req/min por IP (built-in .NET 8 SlidingWindow)
- `Extensions/ApplicationBuilderExtensions.cs` — UseRateLimiter() agregado antes de UseAuthentication

**Patrones implementados:**
- Controllers → MediatR → Application (sin SQL ni lógica en Controllers)
- Respuestas uniformes: `ApiResponse<T>` / `PagedResponse<T>` de `PideServicio.Contracts`
- JWT Auth vía Supabase; policies por claim "rol"
- API Versioning por segmento de URL: /api/v1/

**Documentación creada:**
- `docs/backend/API.md` — referencia completa de endpoints y políticas
- `docs/backend/API_CONVENTIONS.md` — convenciones REST del proyecto

**QA:** APROBADO.

---

## Fase anterior: FASE 6.2 COMPLETADA — Capa Domain del Backend — QA APROBADO

**Fecha:** 2026-07-10

### Resumen de lo creado

Capa Domain completa del backend .NET 8 (Clean Architecture). Total: 22 archivos C# nuevos más documentación.

**Entidades de dominio (20 clases):**
- `Empresa`, `Sucursal`, `Area` — jerarquía organizacional
- `Rol`, `Permiso`, `RolePermission` — RBAC granular
- `TipoServicio`, `Categoria`, `MotivoRechazo`, `MotivoCancelacion`, `Parametro` — catálogos del sistema
- `TecnicoSucursal` — asignación multi-sucursal de técnicos
- `AuditLog`, `Notificacion` — trazabilidad y notificaciones
- `Usuario` (AggregateRoot) — identidad con integración Supabase Auth
- `Ticket` (AggregateRoot) — máquina de estados completa con todas las transiciones del SDD
- `TicketAsignacion`, `TicketHistorialEntrada`, `TicketEvidencia`, `TicketComentario` — entidades del ciclo de vida del ticket

**Value Objects:**
- `Email.cs` — validación con corrección de trailing dot
- `TicketCodigo.cs` — formato PS-XXXXXX

**Documentación creada:**
- `docs/backend/DOMAIN_MODEL.md` — referencia completa del modelo de dominio

**QA:** 2 bloqueantes, 3 moderados y 2 menores detectados y corregidos en la misma sesión. Veredicto final: APROBADO.

---

## Fase anterior: FASE 6.1 COMPLETADA — Arquitectura del Backend — QA APROBADO

**Fecha:** 2026-07-10

### Resumen de lo creado

Solución ASP.NET Core Web API (.NET 8) con Clean Architecture en `backend/`. Total: 72 archivos, 8 proyectos en la solución.

**Proyectos creados (6 en src/):**
- `PideServicio.Domain` — 9 enums, 3 entidades base, 5 excepciones, IDomainEvent (sin dependencias)
- `PideServicio.Contracts` — ApiResponse<T>, ApiError, PagedResponse<T>, PaginacionParams (sin dependencias)
- `PideServicio.Application` — CQRS (ICommand/IQuery/MediatR), Result<T>, CurrentUser, 6 interfaces, 3 behaviors (Logging, Validation, Performance)
- `PideServicio.Infrastructure` — CurrentUserService (lazy JWT claims), DateTimeService, opciones de configuración
- `PideServicio.Persistence` — DbConnectionFactory (Npgsql+Dapper), DatabaseOptions
- `PideServicio.Api` — Program.cs, GlobalExceptionMiddleware, CorrelationIdMiddleware, 3 Extensions

**Tests:** `PideServicio.Architecture.Tests` con 5 pruebas NetArchTest.Rules (dependencias entre capas)

**Documentación:** `docs/backend/` — BACKEND_ARCHITECTURE.md, FOLDER_STRUCTURE.md, CODING_STANDARDS.md

**QA:** 6 correcciones aplicadas post-revisión. Veredicto final: APROBADO.

---

## Fase anterior: 4.10 COMPLETADA — Migración 010: Seguridad, RLS, Storage y Realtime (SQL + Documentación) — QA APROBADO

Tarea completada: **Iteración 4.10 — Migración 010 completa (2026-07-10).** SQL (`supabase/migrations/010_security_storage_rls.sql`, 1977 líneas) + documentación (`docs/database/MIGRATION-010.md`) completados. QA APROBADO: 0 críticos, 4 menores (3 errores tipográficos en RLS_MATRIX.md — no en la migración; 1 bucket email-images sin storage policies, intencional). Migración de infraestructura final: 13 funciones SQL (7 auxiliares RLS SECURITY DEFINER STABLE + 6 trigger negocio), 5 triggers en tickets/ticket_historial, ENABLE ROW LEVEL SECURITY en 21 tablas, FORCE ROW LEVEL SECURITY en 3 tablas (ticket_historial, ticket_asignaciones, audit_logs), 56 políticas RLS en 21 tablas, 4 Storage buckets (tickets-evidence 25MB, avatars 5MB, system-assets público 10MB, email-images 5MB), 6 Storage policies, Realtime en 4 tablas (tickets, ticket_comentarios, ticket_historial, notificaciones), 0 seeds nuevos (330 ya creados en M-002/M-003). Pendiente M-011: sla_configuraciones + feriados + RLS de esas tablas + FK diferida tickets.sla_id.

Tarea anterior: **Iteración 4.9 — Migración 009 completa (2026-07-10).** SQL (`supabase/migrations/009_audit_notifications.sql`) + documentación (`docs/database/MIGRATION-009.md`) completados. QA APROBADO: 0 críticos, 0 menores.

Tarea anterior: **Iteración 4.8 — Migración 008 completa (2026-07-10).** SQL (`supabase/migrations/008_comments_evidence.sql`) + documentación (`docs/database/MIGRATION-008.md`) completados. QA APROBADO: 0 críticos, 1 menor. 2 tablas: ticket_comentarios (11 cols, parcialmente mutable) y ticket_evidencias (12 cols, append-only para contenido). 9 FKs activas, 0 diferidas, 0 triggers.

Tarea anterior: **Iteración 4.6 — Migración 006: tipos_servicio + tickets (2026-07-10).** SQL y documentación completos. Archivos: `supabase/migrations/006_tickets.sql` + `docs/database/MIGRATION-006.md`. Tablas: tipos_servicio (12 cols, sin version, empresa_id NOT NULL, 4 índices, 2 UNIQUE partiales) y tickets (36 cols, version optimistic locking, código PS-XXXXXX por DEFAULT expression, prioridad_efectiva física sin DEFAULT en BD). 0 funciones trigger nuevas, 2 triggers, 12 FKs activas, 4 FKs diferidas (3 tipos_servicio→usuarios + tickets.sla_id→sla_configuraciones). QA: APROBADO — 0 críticos, 4 menores de documentación (MODEL-PHYSICAL.md congelado — sin modificar).
Tarea anterior: **Iteración 4.5 (Documentación) — MIGRATION-005.md: Usuarios, Preferencias y Alcance Territorial (2026-07-10).** Documentación técnica completa de la migración 005 creada. 11 secciones, 13 queries QA, rollback en 3 niveles. Tablas documentadas: usuarios (22 cols, version, auth_id inmutable), preferencias_notificacion (17 cols, trigger auto-creación), tecnico_sucursales (9 cols, guard de es_principal). 3 funciones trigger nuevas, 6 triggers, 7 índices explícitos, 13 FKs activas (incluyendo cross-schema a auth.users), 0 seeds, 0 FKs diferidas. Pendiente: generar `supabase/migrations/005_usuarios.sql`.

Tarea anterior: **Iteración 4.4 — Migración 004: Estructura Organizacional (2026-07-10).** Cuarta migración SQL real del sistema creada y documentada. Contenido: 3 tablas (empresas, sucursales, areas), 3 triggers (trg_fn_set_updated_at_only), 7 índices explícitos, 2 FKs activas (empresa→sucursal→area ON DELETE RESTRICT), 11 FKs a usuarios diferidas → M-0031, 0 seeds. Estado: COMPLETADA — QA APROBADO CON OBSERVACIONES (observación menor resuelta).

Tarea anterior: **Iteración 4.3 — Migración 003: Seguridad e Identidad (2026-07-10).** Tercera migración SQL real del sistema creada y documentada. Contenido: tabla role_permissions (junction RBAC roles×permisos con override opcional por empresa), 4 índices (idx_role_perm_rol, idx_role_perm_permiso, idx_role_perm_empresa, uq_role_perm NULL-safe), FKs activas a roles(codigo) y permisos(id), FKs diferidas para empresas y usuarios (M-0031), 234 filas seed RBAC: SUPERADMIN(63) + ADMIN(60) + SUPERVISOR(60) + TECNICO(19) + TRABAJADOR(17) + USUARIO(15). Estado: COMPLETADA — QA APROBADO.

Tarea anterior: **Iteración 4.2 — Migración 002: Catálogos Maestros del Sistema (2026-07-10).** Segunda migración SQL real del sistema creada y documentada. Contenido: función auxiliar trg_fn_set_updated_at_only, 9 ENUMs, secuencia ticket_codigo_seq, 6 tablas de catálogos maestros, 6 triggers BEFORE UPDATE, 12 índices y 96 filas seed. Estado: COMPLETADA.

Tarea anterior: **Iteración 4.1 — Migración 001: Configuración Base (2026-07-10).** Primera migración SQL real del sistema creada y documentada. Estado: APROBADA, lista para ejecutar en Supabase.

### Documentación de BD completada (CONGELADA — no modificar)
- 3.31 Modelo Conceptual: COMPLETADO — `docs/database/MODEL-CONCEPTUAL.md`
- 3.32 Modelo Lógico: COMPLETADO — `docs/database/MODEL-LOGICO.md`
- 3.33 Modelo Físico: COMPLETADO — `docs/database/MODEL-FISICO.md` (2026-07-09)
- **3.34 Documento Contrato Definitivo: COMPLETADO — `docs/database/MODEL-PHYSICAL.md`** (2026-07-10)
- **3.35 Resolución 7 Dudas Arquitectónicas + Validación Final MODEL-PHYSICAL.md: COMPLETADO** (2026-07-10)
- **3.36 Plan de Migraciones: COMPLETADO — `docs/database/MIGRATION-PLAN.md`** (2026-07-10) — APROBADO
- **3.37 Documentos Técnicos Pre-Migración: COMPLETADO** (2026-07-10) — APROBADO — MODELO CONGELADO
  - `docs/database/NAMING_CONVENTIONS.md` — convenciones de nombres (12 categorías)
  - `docs/database/INDEX_STRATEGY.md` — estrategia de índices (50+ índices con justificación)
  - `docs/database/RLS_MATRIX.md` — matriz de acceso (23 tablas × 6 roles × 4 operaciones)

### Módulos completados (documentación aprobada)
- Módulo Empresas: Aprobado
- Módulo Sucursales: Aprobado
- Módulo Roles y Permisos: Aprobado (parcial — faltan BUSINESS-RULES.md y EVENTS.md)
- Módulo Áreas: Aprobado (8/8 documentos) — 2026-07-08
- Módulo Tipos de Servicio: Aprobado (8/8 documentos) — 2026-07-08
- Módulo Catálogos del Sistema: Aprobado (8/8 documentos) — 2026-07-08 (`docs/modules/system-catalogs/`)
- **Módulo Tickets — Fase 1 (Dominio): APROBADO — 2026-07-08**
- **Módulo Tickets — Fase 2 (completa): BUSINESS-RULES.md, PERMISSIONS.md, EVENTS.md y VALIDATIONS.md APROBADOS — 2026-07-09**
- **Módulo Tickets — Fase 7 (Consolidación Final): COMPLETADA — 2026-07-09**
- **Módulo Tickets — CERRADO: Propagación de 3 decisiones finales + actualización de 9 documentos — 2026-07-09**

### Última tarea completada
**Documentos Técnicos Pre-Migración + Congelamiento del Modelo (Iteración 3.37)** — 2026-07-10

Tres documentos técnicos creados y aprobados: NAMING_CONVENTIONS.md, INDEX_STRATEGY.md y RLS_MATRIX.md. El modelo físico fue congelado por instrucción del usuario: sin nuevas entidades, renombres, relaciones ni estados hasta indicación contraria.

**Tarea anterior (Iteración 3.36):**
**Plan de Migraciones SQL — `docs/database/MIGRATION-PLAN.md`** — 2026-07-10

Plan completo de implementación de la base de datos: 14 fases de migración (F-0 a F-13), 44 migraciones planificadas, 25 tablas mapeadas con dependencias, 9 ENUMs + 1 secuencia, 12 funciones, 12 triggers, estrategia de seeds, RLS, Storage (4 buckets), Realtime (4 tablas), Rollback en 3 niveles, 6 grupos de riesgos con mitigaciones, validación de escalabilidad y checklist pre-implementación. Estado: APROBADO.

**Tarea anterior (Iteración 3.35):**

`docs/database/MODEL-PHYSICAL.md` actualizado: decisiones D-011 a D-017 incorporadas, 3 nuevas tablas añadidas (total: 25 tablas), validación QA final completada, veredicto APROBADO.

Decisiones resueltas:
- DUDA-ARQ-001: Supervisor sin implementación funcional en MVP; permanece en ENUM.
- DUDA-ARQ-002: Técnico multi-sucursal vía tabla puente `tecnico_sucursales`.
- DUDA-ARQ-008: RBAC completo — tablas `roles`, `permisos`, `role_permissions`.
- DUDA-ARQ-009: `audit_logs` sin particionamiento en MVP.
- DUDA-ARQ-010: `prioridad_efectiva` como columna física en `tickets`.
- DUDA-ARQ-011: Solo español en MVP.
- DUDA-ARQ-012: SLA en tiempo laboral real con nueva tabla `feriados`.

**Referencia anterior (Iteración 3.34):** `docs/database/MODEL-PHYSICAL.md` creado — 21 tablas, 9 ENUMs, 1 secuencia, 40+ índices, checklist pre-migración de 20 puntos.

### Tarea en progreso
Ninguna. Iteración 4.4 COMPLETADA. Próxima: Iteración 4.5 — Migración 005 Usuarios e Identidad.

### Siguiente tarea
**Iteración 4.5 — Migración 005: Usuarios e Identidad** — tabla `usuarios` (vinculada a auth.users), tabla `tecnico_sucursales` (junction técnicos multi-sucursal), tabla `preferencias_notificacion`. Incluye `tipos_servicio` si depende de empresas.

Documentación de referencia (no modificar):
- Contrato definitivo: `docs/database/MODEL-PHYSICAL.md` (25 tablas, 9 ENUMs, 1 secuencia)
- Plan de implementación: `docs/database/MIGRATION-PLAN.md` (14 fases, 44 migraciones)
- Convenciones: `docs/database/NAMING_CONVENTIONS.md`
- Índices: `docs/database/INDEX_STRATEGY.md`
- RLS: `docs/database/RLS_MATRIX.md`

Próximos pasos de la Fase 4:
1. Iteración 4.4 — `supabase/migrations/004_company_hierarchy.sql` (empresas + sucursales + areas).
2. Continuar migraciones SQL siguiendo MIGRATION-PLAN.md y NAMING_CONVENTIONS.md.
3. Configurar RLS por tabla y rol (siguiendo RLS_MATRIX.md).
4. Crear índices (siguiendo INDEX_STRATEGY.md).
5. Seeds de datos de prueba reales.
6. Conectar cliente Supabase JS al proyecto React.

### Bloqueantes activos
- **COMMIT PENDIENTE DE AUTORIZACIÓN:** Iteraciones 3.5–3.15 en working tree sin commitear. NO ejecutar sin autorización expresa del usuario.
- DUDA-TS-001: Bloquea DATABASE.md y API.md del módulo Tipos de Servicio.
- DUDA-AR-001: Bloquea DATABASE.md y API.md del módulo Áreas.
- DUDA-SC-001: Bloquea DATABASE.md y API.md del módulo Catálogos del Sistema.

**BLOQUEANTE RESUELTO:** Las 7 Dudas Arquitectónicas de MODEL-PHYSICAL.md §13 fueron resueltas en Iteración 3.35. Fase 4 DESBLOQUEADA.

### Próxima acción
Iniciar Fase 4 — planificación y generación de migraciones SQL en Supabase, tomando `docs/database/MODEL-PHYSICAL.md` como contrato definitivo.

---

## Cambios implementados en Iteración 3.18 — Documentación de Negocio del Módulo Empresas

- **docs/modules/companies/README.md** — Visión general del módulo: propósito, alcance, entidades principales (Empresa, ConfiguraciónEmpresa, IdentidadVisual), dependencias con otros módulos y punto de entrada a la documentación.
- **docs/modules/companies/DOMAIN.md** — Entidad Empresa con todos sus atributos, ConfiguraciónEmpresa, IdentidadVisual; relaciones con sucursales, usuarios, tickets y auditoría; glosario del dominio. Dudas pendientes DUDA-CO-001 (mono-tenant vs multi-tenant) y DUDA-CO-002 documentadas.
- **docs/modules/companies/BUSINESS-RULES.md** — 15 reglas de negocio BR-CO-001 a BR-CO-015. Referencia corregida en BR-CO-013: "BE-008" reemplazado por "SEC-004" (corrección post-QA de referencia rota).
- **docs/modules/companies/PERMISSIONS.md** — Matriz de permisos por rol para todas las operaciones del módulo. DUD-CO-001 documentada en sección de dudas pendientes (¿puede Administrador editar nombre, logo y colores de su empresa?).
- **docs/modules/companies/VALIDATIONS.md** — Validaciones funcionales en lenguaje de negocio sin código. 3 secciones: creación, edición, configuración. Contradicción sobre obligatoriedad de País/Zona horaria entre DOMAIN.md y VALIDATIONS.md resuelta: alineado a DOMAIN.md (campos opcionales con fallback al sistema).
- **docs/modules/companies/EVENTS.md** — 6 eventos de dominio: company.created, company.updated, company.deactivated, company.reactivated, company.config_updated, company.deleted. Evento company.deleted añadido post-QA (C-001 corregido).
- **docs/modules/companies/FLOWS.md** — 6 flujos FL-CO-001 a FL-CO-006. FL-CO-006 (borrado lógico de empresa) añadido post-QA (C-001 corregido). Dudas pendientes DUD-CO-004 a DUD-CO-006 documentadas.
- **docs/modules/companies/FUTURE.md** — 8 funcionalidades futuras FUT-CO-001 a FUT-CO-008.

**QA:** Ciclo inicial rechazado con 3 críticos — C-001 (faltaba FL-CO-006 y evento company.deleted), C-002 (contradicción sobre obligatoriedad de País/Zona horaria), C-003 (referencia rota "BE-008" en BR-CO-013). Los 3 críticos fueron corregidos. Estado final: aprobado con observaciones menores pendientes.

**Dudas abiertas (requieren decisión del usuario):**
- **DUDA-CO-001 (crítica — bloqueante para DATABASE.md):** ¿El sistema es mono-tenant o multi-tenant? El SDD menciona "una empresa" pero la arquitectura de permisos usa `empresa_id` multi-tenant. Afecta DATABASE.md y API.md de todos los módulos.
- **DUD-CO-001:** ¿Puede un Administrador editar el nombre, logo y colores de su empresa, o solo el SuperAdministrador?
- **DUD-CO-004:** ¿Los tickets activos bloquean la desactivación de empresa o solo generan advertencia con confirmación opcional?
- **QA-DUD-001:** ¿Se notifica a los administradores de la empresa cuando esta es desactivada?
- **QA-DUD-002:** ¿Los cambios de identidad visual (logo, colores) generan `company.updated` o `company.config_updated`?

---

## Cambios implementados en Iteración 3.17 — Documentación de Negocio del Módulo Usuarios

- **docs/modules/users/README.md** — Visión general del módulo: alcance, propósito, dependencias y punto de entrada a la documentación.
- **docs/modules/users/DOMAIN.md** — Entidad Usuario, enumerados Rol y EstadoLaboral, relaciones con otras entidades y glosario del dominio.
- **docs/modules/users/BUSINESS-RULES.md** — 11 reglas de negocio BR-US-001 a BR-US-011. BR-US-011 añadida: usuario Inactivo o Suspendido no puede recibir tickets nuevos.
- **docs/modules/users/PERMISSIONS.md** — Matriz de permisos por rol para todas las operaciones. Fila de borrado lógico añadida. Duda CRIT-02 documentada (¿puede Admin restablecer contraseñas?).
- **docs/modules/users/VALIDATIONS.md** — Validaciones de negocio reescritas sin código Zod. FL-US-005 separada en FL-US-005A/005B. Requisito de contraseña en minúsculas per SDD §16.3.
- **docs/modules/users/EVENTS.md** — 5 eventos de dominio con payloads y efectos en otros módulos. OBS-04 corregida.
- **docs/modules/users/FLOWS.md** — 5 flujos FL-US-001 a FL-US-005. CRIT-01 resuelto (desactivación ≠ borrado lógico). CRIT-02 documentado como duda pendiente.
- **docs/modules/users/FUTURE.md** — 6 funcionalidades planificadas FUT-US-001 a FUT-US-006.

**QA:** CRIT-01, CRIT-03, CRIT-04 resueltos. CRIT-02 pendiente decisión del usuario. OBS-01 a OBS-08 gestionadas. DATABASE.md y API.md no creados (pendiente aprobación de negocio).

**Duda pendiente CRIT-02:** ¿Puede un Administrador restablecer contraseñas de otros usuarios? Contradicción entre PERMISSIONS.md (sí) y docs/architecture/permissions/README.md (solo SuperAdministrador). Requiere decisión explícita del usuario.

---

## Iteración 3.16 — Nueva Estructura Documental Modular · `completada` | Sin commit (no requerido)

> **Cambio de metodología aprobado:** A partir de esta iteración, toda implementación de módulo requiere documentación previa completa. El orden obligatorio es: Dominio → Reglas → Flujos → Permisos → Eventos → Validaciones → Aprobación → DB → API → Backend.

---

## Cambios implementados en Iteración 3.16 — Nueva Estructura Documental Modular

- **docs/README.md** (nuevo) — Índice maestro de toda la documentación del proyecto. Punto de entrada único para navegar la estructura documental.
- **docs/METHODOLOGY.md** (nuevo) — Define la metodología Document-First obligatoria, el orden de trabajo por módulo y las reglas de aprobación antes de implementar.
- **docs/architecture/frontend/README.md** (nuevo) — Arquitectura del frontend: estructura de features, patrones, convenciones de componentes.
- **docs/architecture/backend/README.md** (nuevo) — Arquitectura del backend: Edge Functions, servicios, contratos de API.
- **docs/architecture/database/README.md** (nuevo) — Arquitectura de base de datos: esquema general, convenciones de tablas, migraciones.
- **docs/architecture/security/README.md** (nuevo) — Modelo de seguridad: autenticación, autorización, RLS, validaciones.
- **docs/architecture/notifications/README.md** (nuevo) — Arquitectura del sistema de notificaciones multi-canal.
- **docs/architecture/events/README.md** (nuevo) — Sistema de eventos: EventBus, contratos, flujo de publicación/suscripción.
- **docs/architecture/permissions/README.md** (nuevo) — Modelo de permisos por rol: matriz completa de accesos.
- **docs/architecture/integrations/README.md** (nuevo) — Integraciones externas: Supabase, Web Push, canales de notificación.
- **docs/modules/tickets/** (11 archivos) — Documentación completa del módulo de tickets: dominio, reglas, flujos, permisos, eventos, validaciones, DB, API, backend, UI, tests.
- **docs/modules/users/** (11 archivos) — Documentación completa del módulo de usuarios.
- **docs/modules/notifications/** (11 archivos) — Documentación completa del módulo de notificaciones.
- **docs/modules/dashboard/** (11 archivos) — Documentación completa del módulo de dashboard.
- **docs/modules/audit/** (11 archivos) — Documentación completa del módulo de auditoría.
- **docs/modules/reports/** (11 archivos) — Documentación completa del módulo de reportes.
- **docs/modules/settings/** (11 archivos) — Documentación completa del módulo de configuración.
- **docs/modules/companies/** (11 archivos) — Documentación completa del módulo de empresas.
- **docs/modules/branches/** (11 archivos) — Documentación completa del módulo de sucursales.
- **docs/modules/areas/** (11 archivos) — Documentación completa del módulo de áreas.
- **docs/modules/comments/** (11 archivos) — Documentación completa del módulo de comentarios.
- **docs/modules/attachments/** (11 archivos) — Documentación completa del módulo de adjuntos/evidencias.
- **docs/modules/assignments/** (11 archivos) — Documentación completa del módulo de asignaciones.
- **docs/modules/service-types/** (11 archivos) — Documentación completa del módulo de tipos de servicio.
- **docs/modules/permissions/** (11 archivos) — Documentación completa del módulo de permisos.
- **docs/decisions/ADR-001 a ADR-009** — Architecture Decision Records: decisiones arquitectónicas documentadas con contexto, alternativas consideradas y consecuencias.
- **Total de documentos creados:** ~185 archivos markdown.

---

## Metodología Document-First (vigente desde 3.16)

Antes de implementar cualquier módulo, se deben completar y aprobar los siguientes documentos en orden:

1. **Dominio** — Entidades, atributos, relaciones y glosario del módulo.
2. **Reglas** — Reglas de negocio, restricciones e invariantes.
3. **Flujos** — Diagramas de flujo de los procesos del módulo.
4. **Permisos** — Matriz de accesos por rol para cada operación.
5. **Eventos** — Eventos que emite y consume el módulo.
6. **Validaciones** — Reglas de validación de datos en frontend y backend.
7. **Aprobación** — Revisión y aprobación del usuario antes de continuar.
8. **DB** — Migraciones SQL, índices, RLS, triggers y seeds.
9. **API** — Contratos de Edge Functions y endpoints.
10. **Backend** — Implementación de servicios, lógica de negocio e integración.

---

## Próximo paso — Iteraciones 3.17 y 3.18 (pendientes revisión del usuario)

1. **Revisión y aprobación del módulo Usuarios (3.17):** Usuario revisa los 8 archivos en `docs/modules/users/` y decide sobre CRIT-02 (¿puede Administrador restablecer contraseñas de otros usuarios?).
2. **Revisión y aprobación del módulo Empresas (3.18):** Usuario revisa los 8 archivos en `docs/modules/companies/` y resuelve las 5 dudas abiertas documentadas.
3. **DUDA-CO-001 (bloqueante crítica para todos los módulos):** Definir si el sistema es mono-tenant o multi-tenant antes de crear DATABASE.md de cualquier módulo.
4. **Una vez aprobados los documentos de negocio y resuelta DUDA-CO-001:** crear `DATABASE.md` y `API.md` para cada módulo aprobado.
5. **NO iniciar DATABASE.md ni API.md sin aprobación explícita de los documentos de negocio.**
6. **Commit de 3.5→3.15:** Requiere autorización explícita del usuario. Las iteraciones 3.16, 3.17 y 3.18 no requieren commit propio (solo documentación).
7. **NO iniciar Fase 4 (Base de Datos / Supabase) sin aprobación visual final del usuario.**

---

## Iteración 3.15 — Arquitectura de Notificaciones Multi-Canal · `completada` | Pendiente: commit y aprobación del usuario

> **Modo activo: Frontend First** — Aprobado por usuario el 2026-06-29.
> Orden modificado: Design System → UI Mock Data → DB → Backend → Integración real.
> La serie de subfases 3.x continua (3.0 → 3.15). El frontend tiene datos mock, flujo interactivo
> completo, UX premium, nomenclatura unificada, componentes transversales, consistencia visual auditada,
> design system refactorizado, vistas dedicadas para el CRUD de usuarios, sheet lateral completo
> para modificar tickets, branding completo con logo en favicon, trazabilidad de asignación visible
> en tarjetas mobile y tabla desktop, restricción de Configuración por rol, gestión interactiva de
> empresas y sucursales, filtros y 2 nuevos gráficos en Reportes, restructuración de ProfilePage,
> y arquitectura escalable de notificaciones multi-canal desacoplada por eventos.

> **REGLA DE NO COMMIT:** Los cambios de las iteraciones 3.5 a 3.15 están implementados pero
> **NO commiteados**. El commit requiere autorización explícita del usuario antes de ejecutarse.

---

## Cambios implementados en Iteración 3.15 — Arquitectura de Notificaciones Multi-Canal

- **NotificationEventBus** (`src/features/notifications/core/`) — Bus de eventos desacoplado para publicar y suscribir eventos de notificación entre módulos del sistema sin acoplamiento directo.
- **NotificationService** (`src/features/notifications/core/`) — Servicio central que coordina el ciclo de vida de cada notificación: creación, enrutamiento al canal correcto, persistencia y marcado de estado.
- **NotificationDispatcher** (`src/features/notifications/core/`) — Orquesta el despacho de notificaciones a uno o múltiples canales en función de las preferencias del usuario y la disponibilidad del canal.
- **NotificationFactory** (`src/features/notifications/core/`) — Fábrica para construir objetos `Notification` con tipado estricto a partir de payloads crudos de eventos.
- **NotificationRegistry** (`src/features/notifications/core/`) — Registro central de tipos de notificación soportados, metadatos de canal y prioridades de entrega.
- **NotificationQueue** (`src/features/notifications/core/`) — Cola de notificaciones pendientes con soporte de reintentos y control de concurrencia por canal.
- **NotificationTemplates** (`src/features/notifications/core/`) — Plantillas core reutilizables para generar el cuerpo de notificaciones en distintos formatos (texto plano, HTML, markdown).
- **Canales (10)** (`src/features/notifications/channels/`) — Internal (funcional), Push (listo para solicitar permiso), Email / SMS / WhatsApp / Teams / Slack / Discord / Telegram / Webhook (stubs con interfaz implementada). Cada canal exporta una clase que implementa `INotificationChannel`.
- **preferences.store.ts** (`src/features/notifications/store/`) — Store Zustand para las preferencias de notificación del usuario: canales habilitados, horarios de no molestar, frecuencia de resumen.
- **notifications.store.ts** (`src/features/notifications/store/`) — Store Zustand para el estado de las notificaciones en tiempo real: lista, contadores no leídos, estado de cada ítem.
- **useNotificationService** (`src/features/notifications/hooks/`) — Hook que expone el `NotificationService` a componentes React con lazy initialization y limpieza automática de suscripciones.
- **useNotificationPreferences** (`src/features/notifications/hooks/`) — Hook para leer y actualizar las preferencias de notificación desde `preferences.store.ts`.
- **useNotificationPermission** (`src/features/notifications/hooks/`) — Hook que gestiona el estado del permiso de notificaciones push: `default | granted | denied`, solicitud al usuario y persistencia.
- **PushPermissionBanner** (`src/features/notifications/components/`) — Componente que muestra un banner contextual para solicitar permiso de notificaciones push. Se oculta si el permiso ya fue concedido o denegado.
- **NotificationPreferences** (`src/features/notifications/components/`) — Componente de UI para configurar preferencias de notificación por canal, renderizado en la SettingsPage.
- **index.ts (barrel)** (`src/features/notifications/`) — Re-exporta todos los elementos públicos de la feature: core, canales, store, hooks y componentes.
- **service-worker/README.md** (`src/features/notifications/service-worker/`) — Documentación de la estructura y responsabilidades del Service Worker para notificaciones push futuras.
- **SettingsPage.tsx** (`src/features/settings/pages/`) — Nueva sección "Preferencias de Notificaciones" integrada usando el componente `NotificationPreferences`.
- **notification-architecture.md** (`.claude/`) — Documento interno de referencia de la arquitectura de notificaciones: decisiones de diseño, contratos de interfaces y guía de extensión de canales.
- **TypeScript:** 0 errores. Arquitectura desacoplada, extensible y lista para integración real con Supabase Realtime y Web Push API en Fase 4.

---

## Cambios implementados en Iteración 3.14 — UserEditPage, AppHeader, TicketDetail, UsersPage, ReportsPage, ProfilePage

- **UserEditPage.tsx — ancho completo y grids de campos** — `mx-auto max-w-xl` removido del Card principal: el formulario ocupa todo el ancho disponible. Correo + Teléfono agrupados en grid `sm:grid-cols-2`. Empresa + Sucursal agrupadas en grid `sm:grid-cols-2`. Props `error={!!errors.name}`, `error={!!errors.apellido}`, `error={!!errors.correo}` añadidos para mostrar borde rojo en campos inválidos al enviar el formulario.
- **AppHeader.tsx — Configuración restringida a admin/superadmin** — Variable `isAdmin` calculada como `user?.rol === 'admin' || user?.rol === 'superadmin'`. El item "Configuración" en el dropdown del header ahora se renderiza dentro de `{isAdmin && (...)}`. Consistente con AppSidebar.tsx donde el ítem ya estaba en `adminNav`.
- **TicketDetailPage.tsx — modal "Cambiar estado" rediseñado** — DialogContent recibe clase `ps-glow-modal` y tamaño `max-w-sm`. La descripción muestra el estado actual mediante `<StatusBadge />`. Cada opción de estado tiene borde azul + `bg-primary/10` cuando está seleccionada y ícono `<Check />` al final de la fila. Botón "Confirmar cambio" deshabilitado mientras `pendingStatus === localStatus`. Import `Check` añadido desde lucide-react.
- **TicketDetailPage.tsx — confirmación antes de cancelar ticket** — Nuevo estado local `cancelDialog: boolean`. El botón "Cancelar ticket" ya no navega directamente: ahora abre un `ConfirmDialog` destructivo con mensaje explícito de confirmación. La navegación ocurre solo si el usuario confirma.
- **UsersPage.tsx — gestión interactiva de empresas y sucursales** — Sección "Empresas": botón "Nueva empresa", ícono `Building2` por fila, badge activa/inactiva, dropdown con Editar y toggle de activación. Sección "Sucursales": selector de empresa para filtrar, botón "Nueva sucursal", dropdown por fila con estado activa/inactiva. Estados locales añadidos: `localSucursales`, `localAreas`, `selectedEmpresaFilter`, `toggleEmpresaTarget`, `toggleAreaTarget`. Handlers: `handleToggleEmpresa()` y `handleToggleArea()`. 2 nuevos ConfirmDialogs para confirmar el toggling de activación. Imports añadidos: `Pencil`, `Power`, `Building2`, `MapPin`.
- **ReportsPage.tsx — filtros y 2 nuevos gráficos** — Sección de filtros añadida: campos Desde/Hasta (type date), Empresa (select), Estado (select), botones "Aplicar filtros" y "Limpiar". Botón "Ver todo"/"Ver toda" (variante ghost con ícono `ArrowRight`) en el header de cada gráfico existente. Nuevo gráfico "Tendencia semanal": AreaChart con gradiente azul, datos desde `byWeek`. Nuevo gráfico "Por estado": PieChart donut con colores semánticos por estado usando datos `byStatus`. Nuevos imports: `AreaChart`, `Area`, `Filter`, `X`, `ArrowRight`, `Input`, selects.
- **ProfilePage.tsx — restructuración de layout** — Enlace "Cambiar foto" (texto bajo el avatar) eliminado. Campo "Usuario" añadido en columna izquierda debajo de empresa/sucursal, renderizado condicionalmente si `user.usuario` tiene valor. Card "Apariencia" movida de columna derecha a columna izquierda (debajo del card de perfil, con `mt-4`). Sección de solo lectura (Empresa/Sucursal/Usuario) eliminada del card "Información personal" junto con el Separator que la precedía. Correo + Teléfono agrupados en grid `sm:grid-cols-2`.
- **TypeScript:** 0 errores. Preview verificado en todas las páginas modificadas.

---

## Cambios implementados en Iteración 3.13 — Favicon, trazabilidad de asignación y renombre de columna

- **Favicon → logo.jpeg** — `index.html`: el atributo `href` del `<link rel="icon">` cambió de `favicon.svg` a `/logo.jpeg`. El ícono visible en la pestaña del navegador ahora muestra el logo real del producto.
- **Helper `getAssignedBy(ticket)`** — `MyTicketsPage.tsx`: función auxiliar añadida que extrae el nombre de quien realizó la asignación consultando `ticket.history.find(h => h.action === 'Ticket asignado')?.author`. Retorna el nombre o `null` si no existe entrada de historial.
- **Tarjeta mobile — línea "Por:"** — En la sección "Asignado a" de la tarjeta mobile de cada ticket, se añade una segunda línea `"Por: [nombre]"` (quién asignó) debajo de `"Asignado a: [nombre]"`. Si no hay autor de asignación, la línea no se renderiza.
- **Tabla desktop — columna "F. creación"** — La cabecera de columna "Fecha" fue renombrada a "F. creación" para mayor precisión semántica. La misma columna ahora incluye una sub-línea `"Por: [nombre]"` dentro de la celda de asignación en desktop.
- **TypeScript:** 0 errores. Preview verificado: mobile 375px (tarjeta con "Por:") y desktop 1280px (columna "F. creación", sub-línea "Por:" en celda de asignación).

---

## Cambios implementados en Iteración 3.12 — Branding, badge, bordes y menú de acciones

- **Nombre de marca "Pidde Servicio"** — Corregido en todos los puntos de branding del sistema:
  - `index.html`: `<title>` y `apple-mobile-web-app-title`.
  - `AppSidebar.tsx`: atributo `alt` de la imagen de logo y `<span>` de nombre visible.
  - `AuthLayout.tsx`: texto del logo en el panel de marca (desktop) y en el logo mobile.
  - `AppLayout.tsx`: título fallback en la barra de aplicación.
- **Badge de notificaciones — rojo y centrado** — Actualizado en `AppSidebar.tsx`, `MobileNav.tsx` y `AppHeader.tsx`:
  - Color: `bg-primary` (azul) → `bg-destructive` (rojo).
  - Número centrado: añadido `flex items-center justify-center` al contenedor.
  - Cap: `9+` → `99+` (muestra "99+" cuando count supera 99).
- **Bordes de cards azules** — `src/shared/ui/card.tsx`: clase CSS del borde actualizada de `border` a `border border-primary/25`. Todas las cards del sistema ahora muestran un borde azul semitransparente por defecto.
- **Glow de cards intensificado** — `src/styles/globals.css`: opacidades de la clase `ps-glow-card` subidas de `0.25/0.15` a `0.45/0.3`. El efecto fosforescente/azul es más visible.
- **Filtros móviles en Tickets** — `MyTicketsPage.tsx`: layout de filtros reorganizado para mobile:
  - Fila 1: campo de búsqueda a ancho completo.
  - Fila 2: grid de 2 columnas — Estado+Prioridad | Desde+Hasta (inputs de fecha `w-full` para evitar corte del icono de calendario).
  - Fila 3: botón Limpiar a ancho completo.
  - Desktop: todos los filtros en una sola fila sin cambios.
  - Técnica CSS: `grid grid-cols-2 gap-2 lg:contents` dentro de un `flex flex-col gap-2 lg:flex-row`.
- **Menú de acciones en Tickets** — `MyTicketsPage.tsx`: reestructurado en mobile y desktop:
  - Removidos del menú: "Cambiar estado", "Cambiar prioridad", "Ver historial" (son parte del detalle del ticket).
  - "Modificar ticket" y "Asignar responsable": visibles solo para admin/superadmin (antes "Modificar ticket" era visible para todos).
  - "Ver detalle": mantiene el ícono `Eye`.
  - Añadido "Ver historial de cambios" (visible para todos los roles) con ícono `History` → abre un Dialog con historial mock de modificaciones del ticket.
  - Nuevo estado `historyTicketId` para controlar la apertura del Dialog de historial.
  - Nuevos imports: `Eye` y `History` de lucide-react.
- **TypeScript:** 0 errores. Preview verificado: badge rojo centrado, filtros 2 columnas mobile, `lg:contents` activo en desktop, menú de acciones diferenciado por rol.

---

## Cambios implementados en Iteración 3.10/3.11 — Modificar Ticket como Sheet lateral completo

- **EditTicketSheet** — `MyTicketsPage.tsx`: reemplazado el `EditTicketDialog` (Dialog con 4 campos limitados) por un Sheet lateral que edita todos los campos del ticket.
  - Campos editables: tipo de servicio, título, empresa, sucursal, prioridad, descripción, estado, asignado a.
  - Sección primaria con fondo `bg-primary/5` y label "INFORMACIÓN PRINCIPAL".
  - Sección secundaria con label "CAMPOS ADICIONALES".
  - Botones visuales de prioridad (Baja/Media/Alta/Crítica) con colores semánticos y estado seleccionado marcado visualmente.
  - Dependencia Empresa→Sucursal: sucursal deshabilitada hasta seleccionar empresa; se resetea al cambiar empresa.
  - Validaciones en español: tipo de servicio, título, empresa y sucursal son obligatorios.
  - Footer fijo con acciones Cancelar y Guardar cambios.
  - Nuevos imports: `Sheet`, `SheetContent`, `SheetHeader`, `SheetTitle` (desde `@shared/ui/sheet`); `cn` (desde `@lib/utils`); `MOCK_SUCURSALES`, `MOCK_AREAS`, `TICKET_TYPES` (desde `@mocks/data`).
  - Import removido: `DialogDescription`.
  - TypeScript: 0 errores. Prueba visual confirmada.

---

## Cambios implementados en Iteración 3.9 — Vistas dedicadas de usuarios, logo Pidde, breadcrumb y sorting

- **Logo Pidde** — `/logo.jpeg` integrado en `AppSidebar` y `AuthLayout`. Nombre "Pidde" visible en sidebar y en el panel de marca del login.
- **Breadcrumb — separador `›`** — `PageBreadcrumb` actualizado: el separador cambió de `/` a `›` en todas las páginas que usan el componente.
- **UsersPage — modales eliminados** — Los modales de Crear, Ver y Editar usuario fueron removidos. Las acciones navegan a páginas dedicadas con `react-router-dom navigate()`. La página queda con tabla + filtros solamente.
- **UserNewPage** (`src/features/users/pages/UserNewPage.tsx`) — Nueva página para crear usuario. Formulario completo con validación Zod, campos rol/empresa/sucursal/estado. Breadcrumb con retorno a `/usuarios`. Ruta: `/usuarios/nuevo`.
- **UserDetailPage** (`src/features/users/pages/UserDetailPage.tsx`) — Nueva página para ver perfil de usuario en modo lectura. Breadcrumb con retorno a `/usuarios`. Ruta: `/usuarios/:id`.
- **UserEditPage** (`src/features/users/pages/UserEditPage.tsx`) — Nueva página para editar usuario. Formulario pre-poblado desde el mock. Breadcrumb con retorno a `/usuarios/:id`. Ruta: `/usuarios/:id/editar`.
- **Router** — Tres nuevas rutas bajo `RequireAuth`: `/usuarios/nuevo`, `/usuarios/:id`, `/usuarios/:id/editar`.
- **Sort cronológico descendente** — Implementado en `MyTicketsPage`, `NotificationsPage` y `DashboardPage` (tickets recientes). Más reciente primero como criterio por defecto.
- **SettingsPage — layout PC con navegación lateral** — Panel lateral (tabs verticales) + contenido de sección activa en PC. Mobile sin cambios.
- **ProfilePage — layout PC de 2 columnas** — Columna izquierda: datos personales + foto. Columna derecha: seguridad + preferencias. Mobile: una columna.

### Nuevas páginas (Iteración 3.9)

| Página | Ruta | Archivo |
|---|---|---|
| `UserNewPage` | `/usuarios/nuevo` | `src/features/users/pages/UserNewPage.tsx` |
| `UserDetailPage` | `/usuarios/:id` | `src/features/users/pages/UserDetailPage.tsx` |
| `UserEditPage` | `/usuarios/:id/editar` | `src/features/users/pages/UserEditPage.tsx` |

---

## Cambios implementados en Iteración 3.8 — Design System Refactor

- **Input** — nuevo prop `error`: borde rojo + glow rojo sutil cuando se pasa un string de error. Estado visual de validacion integrado.
- **Textarea** — mismo patron que Input: prop `error` con borde rojo + glow rojo sutil.
- **FormField** (`src/shared/components/FormField.tsx`) — nuevo componente base para formularios. Encapsula Label + campo + mensaje de error + hint. Elimina duplicacion del patron en cada pagina.
- **SearchInput** (`src/shared/components/SearchInput.tsx`) — nuevo componente: Input con icono `Search` integrado. Unifica la implementacion del buscador en todas las paginas.
- **Pagination** (`src/shared/components/Pagination.tsx`) — nuevo componente base reutilizable para paginacion. Reemplaza implementaciones ad-hoc.
- **ChartContainer** (`src/shared/components/ChartContainer.tsx`) — nuevo componente extraido de DashboardPage. Wrapper visual unificado (Card + header + acciones) para todos los graficos.
- **globals.css** — `ps-glow-card` y `ps-glow-form` corregidos: glow como borde luminoso (box-shadow exterior), no como fondo. Comportamiento visual correcto para el design system oscuro.
- **globals.css** — nueva clase `ps-glow-modal`: glow especifico para modales y drawers.
- **MyTicketsPage** — boton "Asignar" inline reemplazado por `DropdownMenu` contextual.
- **NotificationsPage** — botones "X" y "Marcar todo" reemplazados por `DropdownMenu` contextual. Nueva accion `onMarkUnread`.
- **LoginPage, CreateTicketPage, ProfilePage, SettingsPage, UsersPage** — todos los formularios migrados a `FormField`.

---

## Nuevos componentes (Iteracion 3.8)

| Componente | Ruta | Proposito |
|---|---|---|
| `FormField` | `src/shared/components/FormField.tsx` | Base para todos los formularios (Label + campo + error + hint) |
| `SearchInput` | `src/shared/components/SearchInput.tsx` | Input con icono Search integrado |
| `Pagination` | `src/shared/components/Pagination.tsx` | Paginacion base reutilizable |
| `ChartContainer` | `src/shared/components/ChartContainer.tsx` | Wrapper de graficos extraido de DashboardPage |

---

## Cambios implementados en Iteración 3.7 — Gap Audit

- **DashboardPage** — `ps-glow-card` aplicado en todas las chart cards (antes solo en KPI cards). Consistencia visual completa entre secciones.
- **DashboardPage** — Botón "Ver todos" en todas las secciones de gráficos, no solo en tickets recientes.
- **globals.css** — `ps-glow-card` mejorado: borde sólido 1px + difuminado externo (box-shadow) más pronunciado. Glow visible como borde y como brillo ambiental.
- **MobileNav** — Migrado de `flex-1` a `grid-cols-5`. Centrado matemáticamente perfecto de los 5 tabs sin depender del reparto flexible.
- **Labels obligatorio/opcional** — Revisión y completado en LoginPage, SettingsPage, UsersPage (modales Crear y Editar) y todos los formularios restantes.
- **ProfilePage** — Badges de rol con colores semánticos por rol: SuperAdministrador (destructivo/rojo), Administrador (primario/azul), Trabajador (warning/amarillo), Usuario (secundario). Mayor contraste y diferenciación visual.
- **Theme validation** — Sistema dark-only verificado como consistente en todos los componentes. Sin valores hardcodeados que quiebren el tema oscuro.

---

## Cambios implementados en Iteración 3.5 — UX Premium y mejoras de interacción

- **AppLayout.tsx** — ScrollToTop al cambiar de ruta (evita que las páginas nuevas abran con scroll a la mitad).
- **MobileNav** — Fix de centrado de íconos y etiquetas en la barra de navegación inferior.
- **globals.css** — Animaciones CSS `glow` para bordes y sombras (`@keyframes glow`, clases utilitarias).
- **LoginPage** — Fondo animado con partículas/gradiente. Glow border en el card. Selector de empresa y sucursal antes de login. Modal "Olvidé mi contraseña" con formulario de recuperación.
- **DashboardPage** — Glow cards en KPI. Botones "Ver todos" en secciones de tickets recientes. Sparklines en vista PC (mini-gráficos en línea dentro de las stat cards). Filtros de empresa/sucursal para admins. Opción "General" (todas las sucursales) en el selector.
- **MyTicketsPage** — Ordenamiento por fecha (más reciente primero). Filtros de rango de fecha. UI de asignación mejorada (avatar + nombre del responsable). Tabla en desktop en lugar de cards apiladas.
- **TicketDetailPage** — History tracking automático: cada cambio de estado/prioridad/asignado genera una entrada en el historial sin intervención manual. Layout de ancho completo en PC (sin sidebar derecho vacío).
- **CreateTicketPage** — Opción "Otros" condicional en el selector de tipo de servicio (muestra campo de texto libre cuando se selecciona). Evidencias mejoradas: preview de imagen antes de subir, progreso simulado de carga. Layout 2 columnas en PC.
- **NotificationsPage** — Patrón Undo para acciones destructivas (eliminar notificación muestra toast con botón "Deshacer" por 5 segundos). Estilos tipo Gmail para notificaciones no leídas (fondo diferenciado, punto azul). Layout 2 columnas en PC.
- **ProfilePage** — Foto de perfil clickeable con preview inmediato via FileReader (sin esperar subida). Campos de contraseña con botón ojo (mostrar/ocultar). Nombres de secciones actualizados. Agrupación en PC (datos personales | seguridad | preferencias).
- **SettingsPage** — Tooltips de ayuda en opciones avanzadas (icono `?` con Popover). Layout 2 columnas en PC.
- **UsersPage** — Campo Estado visible en la tabla y en el modal de Editar Usuario. Nomenclatura actualizada en columnas y etiquetas.

---

## Cambios implementados en Iteración 3.6 — Nomenclatura global y componentes transversales

- **Nomenclatura global unificada:**
  - `Sucursales` → `Empresa` (nivel superior de organización)
  - `Área` → `Sucursal` (unidad dentro de la empresa)
  - `Tipo de Solicitud` → `Tipo de Servicio`
  - Cambio aplicado en: mocks, constantes, todos los formularios, tablas, filtros, labels y textos de UI.
- **PageBreadcrumb** — Nuevo componente de breadcrumb (ruta: `src/shared/components/PageBreadcrumb.tsx`). Integrado en todas las páginas interiores. Muestra la ruta de navegación actual con separadores y enlace de regreso.
- **CommandMenu** — Nuevo componente de búsqueda global (ruta: `src/shared/components/CommandMenu.tsx`). Activado con `Ctrl+K` (o `Cmd+K` en Mac). Permite navegar entre páginas, buscar tickets y acceder a acciones frecuentes desde cualquier pantalla.
- **PageSkeletons** — Nuevo archivo de esqueletos de carga por página (ruta: `src/shared/components/PageSkeletons.tsx`). Skeletons específicos para: Dashboard, MyTickets, TicketDetail, CreateTicket, Users, Notifications, Profile, Settings, Reports, Audit. Reemplazan el spinner genérico durante la carga inicial.
- **Labels obligatorio/opcional** — Todos los formularios del sistema marcan con `*` los campos requeridos y con `(Opcional)` los opcionales. Aplicado en: CreateTicketPage, ProfilePage, SettingsPage, modal Crear/Editar Usuario.

---

## Pantallas implementadas (15 rutas)

- [x] `/login` — Login premium con fondo animado, glow border, selector empresa/sucursal, modal "Olvidé mi contraseña"
- [x] `/dashboard` — Dashboard con glow cards, sparklines PC, filtros empresa/sucursal, botones "Ver todos", sort cronológico
- [x] `/tickets` — Lista con sort cronológico descendente, filtros de fecha, tabla PC, UI de asignación mejorada
- [x] `/tickets/nuevo` — Formulario 2 columnas PC, "Otros" condicional, evidencias con preview, labels obligatorio/opcional
- [x] `/tickets/:id` — History tracking automático, PC ancho completo, breadcrumb, modal Cambiar Estado con glow+StatusBadge+Check, ConfirmDialog cancelar ticket
- [x] `/usuarios` — Tabla + filtros, acciones navegan a vistas dedicadas, breadcrumb, gestión de empresas y sucursales con toggles e íconos
- [x] `/usuarios/nuevo` — Formulario crear usuario con validación, breadcrumb
- [x] `/usuarios/:id` — Vista de perfil de usuario en modo lectura, breadcrumb
- [x] `/usuarios/:id/editar` — Formulario ancho completo, grids Correo+Teléfono y Empresa+Sucursal, props error en Inputs, breadcrumb
- [x] `/notificaciones` — Sort cronológico descendente, patrón Undo, estilos Gmail no leídas, 2 columnas PC
- [x] `/perfil` — Layout 2 columnas PC (Apariencia en col. izquierda), foto clickeable FileReader, ojo en contraseñas, campo Usuario condicional, grid Correo+Teléfono, breadcrumb
- [x] `/reportes` — Filtros Desde/Hasta/Empresa/Estado, "Ver todo" en gráficos, AreaChart tendencia semanal, PieChart donut por estado, descarga simulada toast.promise, breadcrumb
- [x] `/auditoria` — Exportar con toast.promise, breadcrumb
- [x] `/configuracion` — Layout PC con navegación lateral, tooltips ayuda, breadcrumb (solo visible para admin/superadmin en AppHeader)
- [x] `/ui` — Design System Showcase (Fase 2, mantenida)

---

## Nuevos componentes transversales (Fase 3.6)

| Componente | Ruta | Propósito |
|---|---|---|
| `PageBreadcrumb` | `src/shared/components/PageBreadcrumb.tsx` | Navegación jerárquica en todas las páginas |
| `CommandMenu` | `src/shared/components/CommandMenu.tsx` | Búsqueda global `Ctrl+K` |
| `PageSkeletons` | `src/shared/components/PageSkeletons.tsx` | Skeletons de carga específicos por página |

---

## Próximo paso (resumen de bloqueos activos)

1. **Commit de 3.5→3.15:** Requiere autorización explícita del usuario. Las iteraciones 3.16 y 3.17 no requieren commit.
2. **NO iniciar Fase 4 (Base de Datos / Supabase) sin aprobación visual final del usuario.**
3. **Iteración 3.17 pendiente:** Usuario debe revisar y aprobar `docs/modules/users/` + decidir CRIT-02.
4. **NO iniciar DATABASE.md ni API.md sin aprobación de los documentos de negocio del módulo.**

---

## Fases anteriores completadas

### Iteración 3.17 — Documentación de Negocio del Módulo Usuarios · `completada`
Sin commit requerido (solo documentación). 8 archivos markdown en `docs/modules/users/`: README, DOMAIN, BUSINESS-RULES (11 reglas BR-US-001 a BR-US-011, BR-US-011 añadida por QA), PERMISSIONS (duda CRIT-02 documentada), VALIDATIONS (FL-US-005A/005B, requisito de minúsculas per SDD §16.3), EVENTS (5 eventos con payloads), FLOWS (5 flujos FL-US-001 a FL-US-005, CRIT-01 resuelto), FUTURE (6 funcionalidades). Ciclo QA: CRIT-01/03/04 resueltos; CRIT-02 pendiente decisión del usuario (¿puede Admin restablecer contraseñas?). DATABASE.md y API.md no creados: pendiente aprobación.

### Iteración 3.16 — Nueva Estructura Documental Modular · `completada`
Sin commit requerido (solo documentación). Cambio de metodología aprobado: Document-First obligatoria. Creación de ~185 archivos markdown: `docs/README.md`, `docs/METHODOLOGY.md`, `docs/architecture/` (8 READMEs), `docs/modules/` (15 módulos × 11 archivos), `docs/decisions/ADR-001 a ADR-009`. La metodología Document-First pasa a ser parte de la Definición de Terminado de cada módulo.

### Iteración 3.15 — Arquitectura de Notificaciones Multi-Canal · `completada`
Sin commit propio — acumulada junto a 3.5→3.14. Creación de arquitectura escalable de eventos desacoplados para sistema de notificaciones. NotificationEventBus, NotificationService, NotificationDispatcher, NotificationFactory, NotificationRegistry, NotificationQueue, NotificationTemplates (core). 10 canales: Internal (funcional), Push (listo para permiso), Email/SMS/WhatsApp/Teams/Slack/Discord/Telegram/Webhook (stubs). Stores Zustand: preferences.store.ts + notifications.store.ts. Hooks: useNotificationService, useNotificationPreferences, useNotificationPermission. Componentes: PushPermissionBanner + NotificationPreferences. SettingsPage actualizado con nueva sección "Preferencias de Notificaciones". TypeScript: 0 errores.

### Iteración 3.14 — UserEditPage ancho completo, Configuración restringida, modal Cambiar Estado, gestión empresas/sucursales, filtros+gráficos en Reportes, restructuración ProfilePage · `completada`
Sin commit propio — acumulada junto a 3.5→3.13. `UserEditPage.tsx`: ancho completo, grids Correo+Teléfono y Empresa+Sucursal, props error. `AppHeader.tsx`: isAdmin condicional en item Configuración. `TicketDetailPage.tsx`: modal Cambiar Estado con ps-glow-modal, StatusBadge, Check y botón deshabilitado sin cambio; ConfirmDialog para cancelar ticket. `UsersPage.tsx`: gestión de empresas y sucursales con Building2/MapPin, toggles, ConfirmDialogs. `ReportsPage.tsx`: filtros Desde/Hasta/Empresa/Estado, "Ver todo" en gráficos, AreaChart tendencia semanal, PieChart donut por estado. `ProfilePage.tsx`: "Cambiar foto" eliminado, campo Usuario condicional en col. izquierda, Apariencia movida a col. izquierda, sección solo lectura eliminada, grid Correo+Teléfono. TypeScript: 0 errores. Preview verificado en todas las páginas.

### Iteración 3.13 — Favicon logo.jpeg, trazabilidad de asignación y columna "F. creación" · `completada`
Sin commit propio — acumulada junto a 3.5→3.12. `index.html`: favicon cambiado a `/logo.jpeg`. `MyTicketsPage.tsx`: helper `getAssignedBy` añadido; tarjeta mobile muestra "Asignado a: [nombre]" + "Por: [nombre]"; tabla desktop renombra "Fecha" a "F. creación" y muestra "Por: [nombre]" en la celda de asignación. TypeScript: 0 errores. Preview verificado en mobile y desktop.

### Iteración 3.12 — Branding, badge, bordes y menú de acciones · `completada`
Sin commit propio — acumulada junto a 3.5→3.11. Nombre "Pidde Servicio" corregido en 4 archivos de branding. Badge de notificaciones rojo (`bg-destructive`), centrado y con cap `99+` en los 3 componentes que lo usan. Bordes de cards azules (`border-primary/25`). Glow intensificado en `ps-glow-card`. Filtros mobile de Tickets reorganizados en 3 filas con grid 2 columnas. Menú de acciones de tickets reestructurado por rol con historial de cambios Dialog.

### Iteración 3.10/3.11 — Modificar Ticket como Sheet lateral · `completada`
Sin commit propio — acumulada junto a 3.5→3.9. `EditTicketDialog` reemplazado por `EditTicketSheet` en `MyTicketsPage`. Sheet lateral con todos los campos del ticket, botones visuales de prioridad, dependencia Empresa→Sucursal, validaciones en español y footer fijo.

### Fase 3.8 — Design System Refactor: componentes base y contextuales · `completada`
Sin commit propio — acumulada junto a 3.5, 3.6 y 3.7. Nuevos componentes: FormField, SearchInput, Pagination, ChartContainer. Props `error` en Input y Textarea. Clases `ps-glow-card`, `ps-glow-form`, `ps-glow-modal` corregidas. Formularios de LoginPage, CreateTicketPage, ProfilePage, SettingsPage y UsersPage migrados a FormField. DropdownMenus contextuales en MyTicketsPage y NotificationsPage.

### Fase 3.7 — Gap Audit: refinamiento visual y consistencia transversal · `completada`
Sin commit propio — acumulada junto a 3.5 y 3.6. DashboardPage con glow en todas las chart cards, botones "Ver todos" en todas las secciones, MobileNav con grid-cols-5, labels obligatorio/opcional completos, badges de rol con colores semánticos en ProfilePage, validación de consistencia dark-only.

### Fase 3.6 — Nomenclatura global y componentes transversales · `completada`
Sin commit propio — acumulada junto a 3.5. Nomenclatura Empresa/Sucursal/Tipo de Servicio unificada en todo el sistema. Nuevos: PageBreadcrumb, CommandMenu, PageSkeletons. Labels obligatorio/opcional en todos los formularios.

### Fase 3.5 — UX Premium y mejoras de interacción · `completada`
Sin commit propio — pendiente de autorización. ScrollToTop, animaciones glow, LoginPage premium, DashboardPage glow cards + sparklines, MyTicketsPage tabla PC, TicketDetailPage history tracking, CreateTicketPage 2 columnas + preview evidencias, NotificationsPage patrón Undo + estilos Gmail.

### Fase 3.4 — Flujo interactivo completo (modales y acciones) · `completada`
Commit dc4c1b8. Modales Crear/Editar/Ver/Eliminar en UsersPage. Estado mutable en TicketDetailPage. Guardado con validación en ProfilePage. Switches reactivos en SettingsPage. Notificaciones con marcar/eliminar. Descargas simuladas con toast.promise.

### Fase 3.3 — Consistencia visual, scroll y tipografía · `completada`
22 agentes, 641k tokens. Escala tipográfica canónica en globals.css, fix scroll horizontal en TicketDetailPage, padding normalizado en todas las páginas. Commit: `ed91387`.

### Fase 3.2 — Design System Oscuro + Dashboard Graph-First · `completada`
Paleta `#0B0C10/#111113/#26262B`, fuente Geist, 9 widgets de gráfico, D&D con @dnd-kit, click-to-filter. Commits: `5ebd756`, `d5f9385`.

### Fase 3.1 — Refinamiento Visual y UX · `completada`
14 subagentes. Escala tipográfica reducida, Mobile First compacto, AreaChart con gradiente. Commit: `49a91df`.

### Fase 3.0 — Pantallas completas con Mock Data · `completada`
12 rutas, layouts, router con guards, mocks. Commits: `333cfbe`, `7998db1`, `79e43a0`, `2746042`.

### Fase 2 — Design System · `completada`
Paleta semántica, tokens CSS, shadcn/ui (32 componentes), Spinner, EmptyState, StatusBadge, PriorityBadge, ConfirmDialog, página `/ui`.

### Fase 1 — Inicialización · `completada`
React 19 + Vite 5 + TypeScript strict + ESLint + Prettier + Tailwind + shadcn/ui setup + React Router + TanStack Query + Zustand + Supabase + Vitest + Playwright + Husky.

### Fase 0 — Infraestructura · `completada`
Estructura `.claude/`, 11 subagentes, skill control-de-commits, reglas de commits.
