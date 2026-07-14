# Tareas Completadas

> Registro acumulativo. Se añade al final; no se borra.

## FASE 7.0 — Hardening y Estabilización RC (2026-07-11)

### Auditoría completada por 8 agentes especializados

**Correcciones aplicadas (Frontend):**
- StatusBadge: estados en_espera y cancelado agregados (crash silencioso corregido)
- ProfilePage, mocks/data.ts: roles 'worker'/'user' → 'trabajador'/'usuario' (16 ocurrencias)
- Lazy loading en 11 páginas de features (React.lazy + Suspense)
- manualChunks en Vite (vendor, query, forms, charts, supabase, dnd, motion, dates)
- apiClient: 401 → clearAuth() + redirect a /login
- QueryCache.onError global (Sonner toast para errores de red)
- ErrorBoundary creado y aplicado globalmente
- AppHeader/AppSidebar/MobileNav: badge de notificaciones → Zustand store (eliminado mock)
- Debounce 300ms en búsqueda de tickets
- staleTime: 0 en useConteoNotificaciones
- WCAG AA: FormField.htmlFor, skip-to-content, ARIA roles tabs, button aria-labels, spinner role="status"
- ReportsPage h1→h2, UsersPage UserListSkeleton, AuditPage EmptyState
- 30+ textos con diacríticos corregidos en español (Configuración, Teléfono, contraseña, etc.)
- NotificationsPage: false "Todo al día" durante carga corregido
- TicketDetailPage: handleReopenTicket/handleCancelTicket con GUID vacío → toast.error preventivo

**Correcciones aplicadas (Backend):**
- LoggingBehavior: LogWarning para resultados fallidos de business logic
- 6 validators faltantes creados (IniciarProceso, Pausar, Reanudar, SubmitValidacion, ActivarUsuario, DesactivarUsuario)
- GlobalExceptionMiddleware: LogWarning para 4xx, LogError solo para 5xx
- CORS: AllowAnyMethod/AllowAnyHeader restringidos a métodos y headers necesarios
- [Authorize(Policy = "Autenticado")] agregado a AuthController.ObtenerPerfil
- File upload: whitelist MIME (14 tipos) + límite 25MB + sanitización de nombre
- Security headers middleware (X-Content-Type-Options, X-Frame-Options, etc.)
- Response compression: Brotli + Gzip habilitados

**Bloqueantes activos para RC:**
- L001 (CRÍTICO): Supabase Auth Hook no implementado → JWT sin claims → sistema no puede autenticarse
- 5 endpoints de catálogos faltantes (sucursales, áreas, tipos-servicio, categorías, técnicos)
- Sin protección de rutas por rol (M-2)
- notifications.store inicializa desde MOCK_NOTIFICATIONS (M-003)
- AppHeader.logout sin authService.logout() → sesión Supabase no se destruye (M-004)
- SettingsPage ignora parámetros del backend (void parametros)
- Estado en_espera/cancelado mapeados visualmente incorrecto

---

## FASE 6.6 — Integración Completa del Sistema (2026-07-11)

### Frontend
- Eliminados todos los mocks de datos en 13 páginas
- Creados servicios HTTP: ticketService, usuarioService, notificacionService, configuracionService, auditoriaService
- Creados hooks TanStack Query: useTickets, useUsuarios, useNotificaciones
- Páginas conectadas: MyTickets, TicketDetail, CreateTicket, UsersPage, UserDetail, UserEdit, UserNew, Dashboard, Notifications, Profile, Settings, Audit
- ROL_COLORS/ROL_LABELS actualizados con los 6 roles reales (supervisor, tecnico, trabajador, usuario)
- Infraestructura: apiClient.ts, auth.store con JWT, authService, SessionRestorer, proxy Vite

### Backend
- Compilación exitosa (0 errores) — migrado a net10.0
- 57/57 handlers Application verificados
- Auth hook de Supabase: skeleton creado en supabase/functions/auth-hook/

### QA Docs
- INTEGRATION_TESTS.md (84 casos), KNOWN_LIMITATIONS.md (9 limitaciones), DEPLOYMENT_GUIDE.md

---

## FASE 6.5 — CAPA API DEL BACKEND [2026-07-11] — QA APROBADO

**Proyecto:** `PideServicio.Api` (Clean Architecture, .NET 8)

### Controllers implementados — 13 archivos

**Base:**
- `Controllers/Common/ApiControllerBase.cs` — Mediator lazy, TraceId, HandleResult, HandleCreated, OkPaged, MapFailure

**Controllers V1 (12):**
- `Controllers/V1/Auth/AuthController.cs` — GET /api/v1/auth/me (perfil del usuario autenticado vía ICurrentUserService)
- `Controllers/V1/Empresas/EmpresasController.cs` — CRUD + ToggleActiva (5 endpoints)
- `Controllers/V1/Sucursales/SucursalesController.cs` — CRUD + ToggleActiva (5 endpoints)
- `Controllers/V1/Areas/AreasController.cs` — CRUD + ToggleActiva (5 endpoints)
- `Controllers/V1/Roles/RolesController.cs` — GET roles, GET permisos, GET permisosPorRol (3 endpoints)
- `Controllers/V1/Configuracion/ConfiguracionController.cs` — GET parametros, PUT por clave (2 endpoints)
- `Controllers/V1/Usuarios/UsuariosController.cs` — CRUD + CambiarRol + CambiarEstadoLaboral + Activar + Desactivar (9 endpoints)
- `Controllers/V1/Tickets/TicketsController.cs` — 15 endpoints: CRUD + 10 transiciones de estado (Asignar, Reasignar, IniciarProceso, PausarEspera, ReanudarEspera, SubmitValidacion, Cerrar, Reabrir, Cancelar, CambiarPrioridad, CambiarArea)
- `Controllers/V1/Tickets/ComentariosController.cs` — CRUD comentarios anidados bajo ticket (4 endpoints)
- `Controllers/V1/Tickets/EvidenciasController.cs` — GET list + POST multipart + DELETE evidencias (3 endpoints)
- `Controllers/V1/Notificaciones/NotificacionesController.cs` — Listar + Conteo + MarcarLeida + MarcarTodasLeidas (4 endpoints)
- `Controllers/V1/Auditoria/AuditoriaController.cs` — Listar logs con filtros (1 endpoint)

### Extensions modificados

- `Extensions/ServiceCollectionExtensions.cs` — SwaggerGen con JWT Bearer, 5 políticas de autorización (Autenticado / SoloSuperAdmin / AdminOSuperior / SupervisorOSuperior / Tecnico), Rate Limiting 100 req/min por IP (built-in .NET 8)
- `Extensions/ApplicationBuilderExtensions.cs` — UseRateLimiter() agregado antes de UseAuthentication

### Patrones implementados

- Clean Architecture: Controllers → MediatR → Application (nunca directo a Infrastructure o Persistence)
- Sin SQL ni lógica de negocio en Controllers
- Todas las respuestas via `ApiResponse<T>` / `PagedResponse<T>` de `PideServicio.Contracts`
- JWT Auth vía Supabase; policies por claim "rol"
- Rate Limiting built-in .NET 8 (SlidingWindow 100 req/min)
- API Versioning por segmento de URL: /api/v1/

### Documentación creada

- `docs/backend/API.md` — referencia de todos los endpoints, políticas de autorización y convenciones
- `docs/backend/API_CONVENTIONS.md` — guía de convenciones REST para el proyecto

**QA:** APROBADO.

---

## FASE 6.4 — CAPA INFRASTRUCTURE DEL BACKEND [2026-07-11] — QA APROBADO

**Proyectos:** `PideServicio.Persistence` + `PideServicio.Infrastructure` (Clean Architecture, .NET 8)

### Persistence Layer — 19 repositorios implementados

- `EmpresaRepository`, `SucursalRepository`, `AreaRepository`
- `RolRepository`, `PermisoRepository`
- `TipoServicioRepository`, `CategoriaRepository`, `MotivoCancelacionRepository`, `MotivoRechazoRepository`, `ParametroRepository`
- `TecnicoSucursalRepository`, `UsuarioRepository`
- `TicketRepository`, `TicketHistorialRepository`, `TicketComentarioRepository`, `TicketEvidenciaRepository`, `TicketAsignacionRepository`
- `NotificacionRepository`, `AuditLogRepository`
- `UnitOfWork` — implementación de `IUnitOfWork` vía `NpgsqlTransaction`
- `EntityReconstituter` — helper para reconstitución de entidades con constructores privados vía reflection
- `DbConnectionFactory` — actualizado a `NpgsqlDataSourceBuilder`

**Corrección aplicada en revisión final:**
- `UsuarioRepository.ActualizarAsync`: optimistic locking agregado (`WHERE version = @Version AND deleted_at IS NULL`) y `version = version + 1`
- `Infrastructure.csproj`: eliminados atributos `Version=""` incompatibles con central package management

### Infrastructure Services — 7 servicios implementados

- `AuditService` (`IAuditService`)
- `NotificationService` (`INotificationService`)
- `EmailService` (`IEmailService` — stub SMTP)
- `PushNotificationService` (`IPushNotificationService` — stub)
- `SupabaseStorageService` (`IStorageService` vía HTTP REST)
- `SerilogConfiguration` — extension de `WebApplicationBuilder`
- `CacheService` + `CacheOptions` — wrapper de `IMemoryCache`

**Paquetes NuGet agregados:**
- `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Enrichers.Environment`

### Dependency Injection

- `PideServicio.Persistence.DependencyInjection`: 19 repositorios + `UnitOfWork` registrados
- `PideServicio.Infrastructure.DependencyInjection`: todos los servicios + cache + storage registrados

### Documentación

- `docs/backend/INFRASTRUCTURE.md` creado

**QA:** APROBADO.

---

## FASE 6.3 — CAPA APPLICATION DEL BACKEND [2026-07-10] — QA APROBADO

**Proyecto:** `PideServicio.Application` (Clean Architecture, .NET 8)

**Total archivos creados:** 169 archivos en `backend/src/PideServicio.Application/Features/`

**Módulos implementados (Commands + Queries + DTOs + Mappings):**
- Empresas (16 archivos): CreateEmpresa, UpdateEmpresa, ToggleEmpresaActiva, GetEmpresaById, ListEmpresas
- Sucursales (16 archivos): CreateSucursal, UpdateSucursal, ToggleSucursalActiva, GetSucursalById, ListSucursales
- Areas (16 archivos): CreateArea, UpdateArea, ToggleAreaActiva, GetAreaById, ListAreas
- Comentarios (12 archivos): CrearComentario, EditarComentario, EliminarComentario, ListComentarios
- Evidencias (10 archivos): SubirEvidencia, EliminarEvidencia, ListEvidencias
- Notificaciones (11 archivos): MarcarNotificacionLeida, MarcarTodasLeidas, ListNotificaciones, GetConteoNoLeidas
- Auditoria (4 archivos): ListAuditLogs
- Configuracion (9 archivos): UpdateParametro, GetParametros, GetParametroPorClave
- Usuarios (24 archivos): CreateUsuario, UpdateUsuarioPerfil, CambiarRol, CambiarEstadoLaboral, DesactivarUsuario, ActivarUsuario, GetUsuarioById, ListUsuarios
- Roles (9 archivos): GetRoles, GetPermisos, GetPermisosPorRol
- Tickets (42 archivos): CrearTicket, AsignarTicket, ReasignarTicket, IniciarProceso, PauseParaEspera, ReanudarDesdeEspera, SubmitParaValidacion, CerrarTicket, ReabrirTicket, CancelarTicket, CambiarPrioridad, CambiarArea + GetTicketById, ListTickets, GetTicketHistorial

**Archivos comunes creados:**
- `Common/Models/Result.cs` (extendido con ResultTipo enum)
- `Common/Models/ListResult.cs`, `PagedResult.cs`
- `Common/CQRS/ICommandHandler.cs`, `IQueryHandler.cs`
- `Common/Interfaces/IUnitOfWork.cs`, `IEmailService.cs`, `IPushNotificationService.cs`, `IStorageService.cs`
- `Common/Interfaces/Repositories/` — 19 interfaces de repositorios
- `Common/Mappings/MappingConfig.cs` — auto-scan via `config.Scan(Assembly.GetExecutingAssembly())`

**Documentación creada:**
- `docs/backend/APPLICATION_LAYER.md` — documentación completa de la capa

**Patrones implementados:**
- CQRS con MediatR 12.2 (ICommand<T>/IQuery<T> → handler → Result<T>)
- FluentValidation 11.9 (un validator por Request, sin llamadas a BD)
- Mapster 7.4 (IRegister por módulo, auto-scan)
- Result<T> discriminado con ResultTipo enum
- Capa Application depende SOLO de Domain — sin Controllers, sin EF Core, sin SQL directo

**QA:** APROBADO.

---

## FASE 6.2 — CAPA DOMAIN DEL BACKEND [2026-07-10] — QA APROBADO

**Proyecto:** `PideServicio.Domain` (Clean Architecture, .NET 8)

**Entidades creadas (20):**
- `backend/src/PideServicio.Domain/Entities/Empresa.cs`
- `backend/src/PideServicio.Domain/Entities/Sucursal.cs`
- `backend/src/PideServicio.Domain/Entities/Area.cs`
- `backend/src/PideServicio.Domain/Entities/Rol.cs`
- `backend/src/PideServicio.Domain/Entities/Permiso.cs`
- `backend/src/PideServicio.Domain/Entities/RolePermission.cs`
- `backend/src/PideServicio.Domain/Entities/TipoServicio.cs`
- `backend/src/PideServicio.Domain/Entities/Categoria.cs`
- `backend/src/PideServicio.Domain/Entities/MotivoRechazo.cs`
- `backend/src/PideServicio.Domain/Entities/MotivoCancelacion.cs` (nueva, detectada en QA)
- `backend/src/PideServicio.Domain/Entities/Parametro.cs`
- `backend/src/PideServicio.Domain/Entities/TecnicoSucursal.cs`
- `backend/src/PideServicio.Domain/Entities/AuditLog.cs`
- `backend/src/PideServicio.Domain/Entities/Notificacion.cs`
- `backend/src/PideServicio.Domain/Entities/Usuario.cs` (AggregateRoot)
- `backend/src/PideServicio.Domain/Entities/Ticket.cs` (AggregateRoot, máquina de estados completa)
- `backend/src/PideServicio.Domain/Entities/TicketAsignacion.cs`
- `backend/src/PideServicio.Domain/Entities/TicketHistorialEntrada.cs`
- `backend/src/PideServicio.Domain/Entities/TicketEvidencia.cs`
- `backend/src/PideServicio.Domain/Entities/TicketComentario.cs`

**Value Objects completados/corregidos:**
- `backend/src/PideServicio.Domain/ValueObjects/Email.cs` (corrección de validación trailing dot)
- `backend/src/PideServicio.Domain/ValueObjects/TicketCodigo.cs`

**Documentación:**
- `docs/backend/DOMAIN_MODEL.md` (nuevo, referencia completa del modelo de dominio)

**QA:** 2 bloqueantes, 3 moderados y 2 menores detectados y corregidos en la misma sesión. Veredicto: APROBADO.

---

## FASE 6.1 — ARQUITECTURA DEL BACKEND [2026-07-10] — QA APROBADO

**Solución:** `backend/PideServicio.sln` (Clean Architecture, .NET 8)

**Proyectos creados (6):**
- PideServicio.Domain — enums, entidades base, excepciones
- PideServicio.Contracts — ApiResponse<T>, PagedResponse<T>
- PideServicio.Application — CQRS+MediatR, Result<T>, behaviors
- PideServicio.Infrastructure — auth JWT, CurrentUserService
- PideServicio.Persistence — DbConnectionFactory (Dapper+Npgsql)
- PideServicio.Api — Program.cs, middleware, extensiones

**Tests:** PideServicio.Architecture.Tests (5 pruebas NetArchTest)

**Documentación:** docs/backend/ (BACKEND_ARCHITECTURE.md, FOLDER_STRUCTURE.md, CODING_STANDARDS.md)

**Total archivos creados:** 72

**QA:** 6 correcciones aplicadas post-revisión — APROBADO

---

### Iteración 4.10 — Migración 010: Seguridad, RLS, Storage y Realtime (SQL + Documentación) — QA APROBADO
**Fecha:** 2026-07-10
**Archivos creados:**
- `supabase/migrations/010_security_storage_rls.sql` (1977 líneas)
- `docs/database/MIGRATION-010.md`
**SQL (`010_security_storage_rls.sql`):** 0 tablas nuevas. 13 funciones (7 auxiliares RLS SECURITY DEFINER STABLE + 6 de trigger de negocio). 5 triggers: trg_tickets_guard_inmutable (BEFORE UPDATE), trg_tickets_validate_transition (BEFORE UPDATE OF estado), trg_tickets_after_estado (AFTER UPDATE OF estado → historial), trg_tickets_after_insert (AFTER INSERT → historial CREADO), trg_historial_check_rechazo (BEFORE INSERT en ticket_historial). ENABLE ROW LEVEL SECURITY en 21 tablas. FORCE ROW LEVEL SECURITY en 3 tablas (ticket_historial, ticket_asignaciones, audit_logs). 56 políticas RLS implementando RLS_MATRIX.md. 4 Storage buckets (tickets-evidence 25MB, avatars 5MB, system-assets público 10MB, email-images 5MB) + 6 Storage policies. Realtime habilitado en 4 tablas (tickets, ticket_comentarios, ticket_historial, notificaciones). 0 seeds nuevos.
**Funciones auxiliares RLS:** get_current_user_id(), get_current_user_rol(), get_current_user_empresa_id(), get_current_user_sucursales(), is_superadmin(), is_admin_or_above(), create_audit_log().
**Función pura de transiciones:** validate_ticket_transition(estado_anterior, estado_nuevo, rol) IMMUTABLE — implementa la matriz completa del SDD §5.
**Documentación (`MIGRATION-010.md`):** 12 secciones, 17 queries QA, rollback en 5 niveles. Decisiones de diseño documentadas: tecnico_id (no tecnico_asignado_id), audit_logs filtrado via sucursal_id, email-images service_role exclusivo, FORCE RLS semántica.
**QA:** APROBADO — 0 críticos, 4 menores: OBS-001 (email-images sin policies — intencional, documentado con comentario), OBS-002/004 (RLS_MATRIX.md usa nombres incorrectos — errores tipográficos en el doc, migración es correcta), OBS-003 (is_admin_or_above reservada para M-011/Edge Functions).
**Pendiente M-011:** sla_configuraciones + feriados + RLS de esas tablas + FK diferida tickets.sla_id + completar trg_fn_tickets_guard_inmutable con protección de sla_id.

### Iteración 4.9 — Migración 009: notificaciones + audit_logs (SQL + Documentación) — QA APROBADO
**Fecha:** 2026-07-10
**Archivos creados:**
- `supabase/migrations/009_audit_notifications.sql`
- `docs/database/MIGRATION-009.md`
**SQL (`009_audit_notifications.sql`):** 2 tablas, 1 trigger (trg_fn_set_updated_at_only reutilizado de M-002), 0 nuevas funciones, 0 ENUMs nuevos, 9 índices (3 notificaciones + 6 audit_logs), 6 FKs activas, 0 diferidas, 0 seeds. notificaciones: mutable, canal ENUM (IN_APP/EMAIL/PUSH), sin deleted_at, destinatario_id ON DELETE CASCADE, tipo_evento VARCHAR libre, metadata JSONB. audit_logs: completamente inmutable, actor desnormalizado, entidad_id sin FK (polimórfico), ip INET, sucursal_id para RLS futura.
**Documentación (`MIGRATION-009.md`):** 12 secciones, 16 queries QA, rollback en 3 niveles. §6 naturaleza diferenciada. §7 justificación de 9 índices incluyendo partial (cola de entrega) y compuesto de 3 columnas (badge de notificaciones).
**QA:** APROBADO — 0 críticos, 0 menores (primera migración sin hallazgos).
**Decisión documentada:** DUDA-ARQ-009 resuelta — audit_logs sin particionamiento en MVP; evaluación en fase de optimización.

### Iteración 4.8 — Migración 008: ticket_comentarios + ticket_evidencias (SQL + Documentación) — QA APROBADO
**Fecha:** 2026-07-10
**Archivos creados:**
- `supabase/migrations/008_comments_evidence.sql`
- `docs/database/MIGRATION-008.md`
**SQL (`008_comments_evidence.sql`):** 2 tablas, 0 triggers, 0 ENUMs nuevos, 6 índices explícitos (3 por tabla, 2 parciales), 9 FKs activas, 0 diferidas, 0 seeds. ticket_comentarios: parcialmente mutable (editado_en en lugar de updated_at, sin version, sin trigger), es_interno BOOLEAN para visibilidad, soft delete completo. ticket_evidencias: append-only para contenido, url_almacenamiento TEXT preparado para Supabase Storage, evidencia_tipo ENUM (INICIAL/FINAL) de M-002, soft delete.
**Documentación (`MIGRATION-008.md`):** 12 secciones, 15 queries QA, rollback en 3 niveles. Principio de mutabilidad diferenciada documentado. Preparación para Storage, Signed URLs, Realtime y RLS documentada en §9.
**QA:** APROBADO — 0 críticos, 1 menor (ON UPDATE CASCADE omitido, patrón consistente con M-007, documentado).
**Índices especiales:** idx_comentarios_internos (PARTIAL: es_interno=true AND deleted_at IS NULL), idx_evidencias_final_activas (PARTIAL: tipo='FINAL' AND deleted_at IS NULL — validación de cierre de ticket).

### Iteración 4.7 — Documentación MIGRATION-007.md: ticket_asignaciones + ticket_historial
**Fecha:** 2026-07-10
**Archivos creados:**
- `docs/database/MIGRATION-007.md`
**Contenido:** 12 secciones, 14 queries QA, rollback en 3 niveles. 2 tablas append-only (ticket_asignaciones y ticket_historial), 0 triggers, 10 índices explícitos (4 en ticket_asignaciones + 6 en ticket_historial incluyendo GIN sobre metadata JSONB), 9 FKs activas (5 en ticket_asignaciones + 4 en ticket_historial), 0 FKs diferidas, 0 seeds.
**Tablas:** `ticket_asignaciones` (9 cols: id, ticket_id, tecnico_id, asignador_id, es_reasignacion, tecnico_anterior_id, motivo_reasignacion, created_at, created_by; CHECK chk_asignaciones_reasignacion). `ticket_historial` (12 cols: id, ticket_id, actor_id, tipo_evento, estado_anterior, estado_nuevo, comentario_texto, rejection_reason_id, rejection_comment, metadata JSONB, created_at, created_by; sin updated_at/deleted_at/version).
**Principio documentado:** Append-only — por qué no hay updated_at, deleted_at ni version; diferencia con audit_logs; impacto en rollback.
**Índices especiales:** idx_historial_estado_cambiado (PARTIAL para cálculo SLA), idx_historial_metadata_gin (GIN para BI/JSONB), idx_asignaciones_reasignaciones (PARTIAL).
**Secciones especiales:** §5 Principio Append-Only, §7 Alternativas consideradas (tabla única vs tablas separadas por tipo), §8 Restricciones diferidas (trigger trg_historial_check_rechazo).

### Iteración 4.6 — Migración 006: tipos_servicio + tickets (SQL + Documentación)
**Fecha:** 2026-07-10
**Archivos creados:**
- `supabase/migrations/006_tickets.sql`
- `docs/database/MIGRATION-006.md`
**SQL (`006_tickets.sql`):** 2 tablas (tipos_servicio + tickets), 0 funciones trigger nuevas, 2 triggers, 14 índices explícitos (4 de tipos_servicio + 10 de tickets), 12 FKs activas, 4 FKs diferidas (created/updated/deleted_by de tipos_servicio → M-0031 + tickets.sla_id → M-0031). Código PS-XXXXXX por DEFAULT expression con nextval(ticket_codigo_seq). prioridad_efectiva NOT NULL sin DEFAULT (calculada por Backend). version en tickets (optimistic locking), sin version en tipos_servicio.
**Documentación (`MIGRATION-006.md`):** 12 secciones, 13 queries QA, rollback en 3 niveles. Secciones especiales: historia de tipos_servicio, análisis D-015 (prioridad_efectiva), estrategia DEFAULT expression vs trigger, justificación individual de cada índice, restricciones diferidas (§8), preparación para módulos futuros.
**QA:** APROBADO — 0 críticos, 4 menores de documentación (MODEL-PHYSICAL.md congelado, sin modificar).
**Decisiones aplicadas:** DEFAULT expression para código (no trigger); prioridad_efectiva física (D-015); idx_tickets_prioridad sobre prioridad_efectiva; secuencia global.

### Iteración 4.5 — Migración 005: usuarios + preferencias_notificacion + tecnico_sucursales (SQL + Documentación)
**Fecha:** 2026-07-10
**Archivos creados:**
- `supabase/migrations/005_users.sql`
- `docs/database/MIGRATION-005.md`
**SQL (`005_users.sql`):** 3 tablas, 3 funciones trigger nuevas (guard_auth_id, after_insert, guard_principal), 6 triggers, 7 índices explícitos, 13 FKs activas (incluyendo FK cross-schema a auth.users), 0 seeds, 0 FKs diferidas. usuarios con version (optimistic locking) + auth_id inmutable. Creación automática de preferencias_notificacion en INSERT de usuarios.
**Documentación (`MIGRATION-005.md`):** 11 secciones, 13 queries QA, rollback en 3 niveles. Integración con Supabase Auth, estados del usuario (activo × estado_laboral), escalabilidad multi-tenant.
**QA:** APROBADO.

### Iteración 4.4 — Migración 004: Estructura Organizacional
**Fecha:** 2026-07-10
**Archivos creados:**
- `supabase/migrations/004_organizational_structure.sql`
- `docs/database/MIGRATION-004.md`
**Tablas:** empresas, sucursales, areas
**Objetos:** 3 tablas, 3 triggers, 7 índices explícitos, 2 FKs activas (empresa→sucursal→area), 0 seeds
**FKs diferidas:** created_by, updated_by, deleted_by, responsable_id → M-0031
**QA:** APROBADO CON OBSERVACIONES (observación menor resuelta)
**Restricciones cumplidas:** sin version, sin RLS, sin seeds, sin usuarios, sin tickets

## Iteración 4.3 — Migración 003: Seguridad e Identidad
**Fecha:** 2026-07-10
**Archivos:** supabase/migrations/003_security_identity.sql · docs/database/MIGRATION-003.md
**Contenido:** Tabla role_permissions (junction RBAC roles×permisos), 4 índices, FKs activas a roles/permisos, 234 filas seed RBAC (6 roles: SUPERADMIN 63 + ADMIN 60 + SUPERVISOR 60 + TECNICO 19 + TRABAJADOR 17 + USUARIO 15).
**Estado:** COMPLETADO — QA APROBADO

## Iteración 4.2 — Migración 002: Catálogos Maestros del Sistema
**Fecha:** 2026-07-10
**Archivos:** supabase/migrations/002_master_catalogs.sql · docs/database/MIGRATION-002.md
**Contenido:** Función auxiliar trg_fn_set_updated_at_only, 9 ENUMs (rol_tipo, estado_laboral_tipo, ticket_estado_tipo, prioridad_tipo, evidencia_tipo, canal_notificacion_tipo, estado_entrega_tipo, tipo_dato_parametro_tipo, tipo_evento_historial_tipo), secuencia ticket_codigo_seq (PS-000001 a PS-999999), 6 tablas (roles, permisos, categorias, motivos_cancelacion, motivos_rechazo, parametros_sistema), 6 triggers BEFORE UPDATE, 12 índices (incluyendo NULL-safe uniqueness con UUID centinela), 96 filas seed (6 roles + 63 permisos + 9 categorías + 5 motivos_cancelacion + 5 motivos_rechazo + 8 parametros_sistema).
**Estado:** COMPLETADO

## Iteración 4.1 — Migración 001: Configuración Base (2026-07-10)
- [x] Crear `supabase/migrations/001_initial_setup.sql` (extensiones + trg_fn_set_updated_at + comentario schema)
- [x] Crear `docs/database/MIGRATION-001.md` (documentación técnica completa con QA)
- Estado: APROBADO — lista para ejecutar

## Iteración 3.37 — Documentos Técnicos Pre-Migración (2026-07-10)
- [x] Crear `docs/database/NAMING_CONVENTIONS.md` (convenciones de 12 categorías)
- [x] Crear `docs/database/INDEX_STRATEGY.md` (50+ índices con justificación)
- [x] Crear `docs/database/RLS_MATRIX.md` (23 tablas × 6 roles × 4 operaciones)
- [x] Modelo físico congelado por instrucción del usuario
- Estado: APROBADO — listos para Fase 4 (migraciones SQL)

## Iteración 3.36 — Plan de Migraciones (2026-07-10)
- [x] Crear `docs/database/MIGRATION-PLAN.md` con 14 fases, 44 migraciones, mapa de dependencias, estrategia de seeds/RLS/Storage/Realtime/Rollback, riesgos y checklist.
- Estado: APROBADO

## 2026-07-10
- [x] **RESOLUCIÓN DE 7 DUDAS ARQUITECTÓNICAS + Validación Final MODEL-PHYSICAL.md (Iteración 3.35)** — 2026-07-10
  - `docs/database/MODEL-PHYSICAL.md` actualizado — decisiones D-011 a D-017 incorporadas, 3 nuevas tablas añadidas, validación QA final completada, veredicto APROBADO.
  - Total tablas: 25 (de 21 a 25): +`roles`, +`permisos`, +`role_permissions`, +`tecnico_sucursales`, +`feriados`.
  - **DUDA-ARQ-001 RESUELTA:** Supervisor sin implementación funcional en MVP; permanece en ENUM para no migrar después.
  - **DUDA-ARQ-002 RESUELTA:** Técnico multi-sucursal vía tabla puente `tecnico_sucursales` (N:M entre `usuarios` y `sucursales`).
  - **DUDA-ARQ-008 RESUELTA:** RBAC completo implementado — tablas `roles`, `permisos`, `role_permissions` para gestión granular de permisos.
  - **DUDA-ARQ-009 RESUELTA:** `audit_logs` sin particionamiento en MVP; se añade en fase de optimización.
  - **DUDA-ARQ-010 RESUELTA:** `prioridad_efectiva` como columna física en `tickets` (calculada al crear/actualizar, no como columna generada).
  - **DUDA-ARQ-011 RESUELTA:** Solo español en MVP; sin infraestructura i18n en base de datos.
  - **DUDA-ARQ-012 RESUELTA:** SLA en tiempo laboral real — nueva tabla `feriados` para excluir días festivos del cálculo.
  - **Fase 4 DESBLOQUEADA** — todos los bloqueantes resueltos. MODEL-PHYSICAL.md aprobado como contrato definitivo.
  - Siguiente acción: iniciar Fase 4 — planificación y generación de migraciones SQL en Supabase.

- [x] **ARQUITECTURA DE BASE DE DATOS — Documento Contrato Definitivo (Iteración 3.34)** — 2026-07-10
  - `docs/database/MODEL-PHYSICAL.md` creado (nuevo) — documento contrato definitivo de la base de datos.
  - 21 tablas con detalle completo: objetivo, descripción, responsabilidad, columnas con tipos PostgreSQL nativos, PK, FK, constraints, validaciones e índices justificados por tabla.
  - 9 ENUMs de PostgreSQL definidos.
  - 1 secuencia para numeración correlativa de tickets.
  - Estándar completo de auditoría: todas las tablas relevantes incluyen `deleted_by` y `version`.
  - Catálogo de 40+ índices con justificación individual de cada uno.
  - Análisis de rendimiento por consulta frecuente del sistema.
  - Preparación Supabase completa: RLS, Auth, Storage, Realtime, Edge Functions, Cron Jobs, Triggers.
  - Diseño para escalabilidad hasta millones de tickets.
  - 7 Dudas Arquitectónicas pendientes documentadas en §13: DUDA-ARQ-001, DUDA-ARQ-002, DUDA-ARQ-008, DUDA-ARQ-009, DUDA-ARQ-010, DUDA-ARQ-011, DUDA-ARQ-012.
  - Validación QA completa integrada en el documento.
  - Checklist pre-migración de 20 puntos.
  - **Fase 4 bloqueada** hasta que el usuario resuelva las 7 Dudas Arquitectónicas documentadas en §13.

## 2026-07-09
- [x] **ARQUITECTURA DE BASE DE DATOS — Fase 3 — Modelo Físico (Iteración 3.33)** — 2026-07-09
  - `docs/database/MODEL-FISICO.md` creado (nuevo).
  - 18 tablas físicas con DDL completo (tipos nativos PostgreSQL, restricciones, índices).
  - 9 ENUMs de PostgreSQL definidos.
  - 2 secuencias para numeración correlativa de tickets.
  - 8 funciones PostgreSQL (triggers de auditoría, SLA, historial, notificaciones, validaciones).
  - 9 triggers asociados a las funciones anteriores.
  - 5 vistas (dashboard_metricas, tickets_activos_vista, sla_alertas, carga_tecnico, historial_completo).
  - Políticas RLS por rol documentadas para cada tabla.
  - Integración Supabase: Auth (auth.users), Storage (buckets), Realtime (canales por empresa), Edge Functions (hooks de triggers).
  - Decisión ML-DEC-013 incorporada: MotivoRechazo = catálogo configurable (`motivos_rechazo`) + comentario libre; "Otro" requiere comentario obligatorio; DUDA-ML-001 resuelta.
  - Checklist de validación pre-migración (§14) con 6 pendientes que requieren respuesta del usuario antes de ejecutar migraciones SQL.
  - Las tres fases del Modelo de BD están completas: 3.31 Conceptual + 3.32 Lógico + 3.33 Físico.

- [x] **ARQUITECTURA DE BASE DE DATOS — Fase 2 — Modelo Lógico (Iteración 3.32)** — 2026-07-09
  - `docs/database/MODEL-LOGICO.md` creado (nuevo).
  - 18 entidades persistentes definidas con atributos y tipos lógicos.
  - 2 entidades de solo lectura (Dashboard, Reportes — vistas, no tablas).
  - 1 entidad nueva: ConfiguracionSLA (resolución DUDA-ARQ-003 — SLA es entidad propia con matriz empresa × tipo_servicio × prioridad).
  - 4 enumeraciones del sistema definidas (Rol, EstadoLaboral, EstadoTicket, NivelPrioridad + 5 adicionales).
  - 33 relaciones catalogadas con cardinalidad y participación.
  - 12 decisiones del modelo lógico (ML-DEC-001 a ML-DEC-012).
  - Restricciones de unicidad, inmutabilidad, nulabilidad y workflow documentadas.
  - Normalización 3FN con denormalizaciones justificadas.
  - 7 dudas arquitectónicas pendientes; ninguna bloquea el Modelo Físico excepto DUDA-ML-001.
  - Bloqueo antes del Modelo Físico: DUDA-ML-001 — ¿El MotivoRechazo es catálogo o texto libre en MVP?

- [x] **ARQUITECTURA DE BASE DE DATOS — Fase 1 — Modelo Conceptual (Iteración 3.31)** — 2026-07-09
  - `docs/database/MODEL-CONCEPTUAL.md` creado (nuevo).
  - 7 dominios identificados: Organización Multi-tenant, Identidad y Acceso, Catálogos, Tickets, Comunicación, Trazabilidad, Analítica.
  - 24 entidades documentadas: objetivo, responsabilidad, relaciones e importancia para cada entidad.
  - 6 agregados del dominio identificados y justificados (Empresa, Ticket, Usuario, Catálogo, Notificación, AuditoríaLog).
  - Dependencias clasificadas en 4 categorías: fuertes, débiles, opcionales y futuras.
  - Escalabilidad evaluada para 8 escenarios: multiempresa, IA/ML, ERP, WhatsApp, webhooks, Power BI, app móvil nativa, federated search.
  - 8 riesgos arquitectónicos detectados con mitigación propuesta.
  - 7 Dudas Arquitectónicas abiertas: DUDA-ARQ-001 a DUDA-ARQ-007.
  - Veredicto: modelo conceptual COMPLETO. Listo para Modelo Lógico, condicionado a resolución de DUDA-ARQ-003 (SLA).
  - Próxima tarea: `docs/database/MODEL-LOGICO.md` — BLOQUEADO hasta resolución de DUDA-ARQ-003 por el usuario.

- [x] **Módulo Tickets — Propagación de decisiones finales y cierre definitivo (Iteración 3.30)** — 2026-07-09
  - Propagación de las 3 decisiones finales del usuario en los 9 documentos del módulo Tickets.
  - PERMISSIONS.md: rol Técnico incorporado en todas las 33 acciones y 9 tablas de estado. Trabajador redefinido como creador/consultante. 73 ediciones. (1415 → 1466 líneas)
  - LIFECYCLE.md: En Espera y Cancelado confirmados como estados definitivos del MVP. Técnico propagado en ciclo de vida completo, tabla de roles, notificaciones. Historial de comentarios corregido. 105 ediciones.
  - DOMAIN.md: 9 estados en el workflow. Técnico como nuevo actor. Glosario actualizado. Campos editables resueltos. DUDAs cerradas. 59 ediciones. (836 → 890 líneas)
  - EVENTS.md: EVT-TK-010/011/012/016 activos. EVT-TK-015 reclasificado como N/A. Técnico en todos los eventos de ejecución. DUDA-EVT-001 resuelta. 78 ediciones.
  - VALIDATIONS.md: Categoría obligatoria. Área SOLO editable en Sin Asignar. VAL-TK-032 corregido. EVA-TK-035/039 activos. Técnico como actor en validaciones de ejecución. 22 ediciones.
  - BUSINESS-RULES.md: BR-TK-001 corregido (Trabajador SÍ puede crear). Técnico como ejecutor en las 82 reglas del módulo. DUDAs actualizadas. 47+ ediciones.
  - README.md, REVIEW.md, SUMMARY.md: DUDAs actualizadas, estados del módulo actualizados, Técnico incorporado, veredicto final APROBADO para DATABASE.md.
  - Decisiones resueltas: (1) Rol Técnico — el Técnico ejecuta el trabajo asignado; el Trabajador crea y consulta sus propios tickets. (2) DUDA-TK-012 — categoría OBLIGATORIA (NOT NULL). (3) DUDA-TK-003 — área solo editable en Sin Asignar por Admin/SA; excepción en estados operativos con auditoría y motivo obligatorio.
  - DUDAs que siguen abiertas (no bloquean DATABASE.md): DUDA-TK-005, DUDA-TK-008, DUDA-TK-010, DUDA-TK-011, DUDA-PERM-001, DUDA-VAL-001, DUDA-VAL-003, DUDA-VAL-004.
  - Estado del módulo: CERRADO. Listo para DATABASE.md (requiere aprobación expresa del usuario para iniciar).

- [x] **Módulo Tickets — Fase 7 (Consolidación Final) COMPLETADA** — 2026-07-09
  - Lectura de 7 documentos del módulo (~8.358 líneas): README.md, DOMAIN.md, LIFECYCLE.md, BUSINESS-RULES.md, PERMISSIONS.md, EVENTS.md, VALIDATIONS.md.
  - Identificación de 20 inconsistencias entre los documentos y las 31 decisiones resueltas el 2026-07-09.
  - Creación de `docs/modules/tickets/REVIEW.md` — auditoría completa: 20 inconsistencias con origen, descripción y propuesta de resolución; impacto de las 31 decisiones en cada documento; DUDAs resueltas sin propagar; DUDAs bloqueantes con impacto en implementación; evaluación de escalabilidad y calidad; veredicto de preparación para DATABASE.md; plan de actualización de los 6 documentos existentes.
  - Creación de `docs/modules/tickets/SUMMARY.md` — resumen ejecutivo para desarrolladores nuevos: qué es el módulo, actores, ciclo de vida, invariantes críticos, entidades, historial y auditoría, permisos por rol, validaciones críticas, eventos del dominio, integración con otros módulos, preparación para implementación (Database, Backend, Frontend, Notificaciones, Dashboard, Reportes), capacidades futuras y pendientes bloqueantes.
  - Estado del módulo post-Consolidación: documentación funcional completa y auditada. BLOQUEADO para implementación hasta: (1) decisión del usuario sobre rol Técnico, (2) resolución DUDA-TK-003, (3) resolución DUDA-TK-012, y (4) propagación de 20 inconsistencias en los 6 documentos existentes.

- [x] VALIDATIONS.md — docs/modules/tickets/VALIDATIONS.md creado (borrador, pendiente de aprobación del usuario). 39 validaciones funcionales (VAL-TK-001 a VAL-TK-039), 19 casos especiales (CASE-TK-001 a CASE-TK-019), 48 mensajes funcionales en español (MSG-TK-001 a MSG-TK-048), 7 nuevas DUDAs detectadas (DUDA-VAL-001 a DUDA-VAL-007). Metodología Document-First cumplida: sin código, sin referencias técnicas. Siguiente documento: FLOWS.md (bloqueado hasta aprobación de VALIDATIONS.md).

- [x] EVENTS.md — docs/modules/tickets/EVENTS.md creado y aprobado. 21 eventos (EVT-TK-001 a EVT-TK-021), 2 nuevos justificados (EVT-TK-005, EVT-TK-011), 3 no aplicables en MVP, 20 registros de auditoría (AUD-TK-001 a AUD-TK-020). QA: 9 hallazgos (0 CRÍTICO, 5 MAYOR, 4 MENOR) — todos corregidos. 1 nueva DUDA generada: DUDA-EVT-001 (contenido de comentarios en historial).

---

## Módulo Tickets — BUSINESS-RULES.md APROBADO (Fase 2)
Fecha: 2026-07-09
Estado: APROBADO QA
Ubicación: `docs/modules/tickets/BUSINESS-RULES.md`

- 82 reglas (BR-TK-001 a BR-TK-082)
- 3 ciclos de QA: primer ciclo RECHAZADO (13 hallazgos corregidos), segundo ciclo RECHAZADO (5 hallazgos corregidos), tercer ciclo con verificación parcial — APROBADO.
- H-02 (BR-TK-064 escalación automática): corregido en todos los apartados para usar tiempo condicional alineado con LIFECYCLE.md §10.1.
- El documento está completo y puede usarse como referencia para implementación.

---

## Módulo Tickets — PERMISSIONS.md APROBADO (Fase 2)
Fecha: 2026-07-09
Estado: APROBADO QA
Ubicación: `docs/modules/tickets/PERMISSIONS.md`

- 33 acciones documentadas individualmente, 4 roles, 9 estados
- 1 ciclo de QA: "APROBADO CON OBSERVACIONES" (9 hallazgos), todos corregidos y verificados como RESUELTO en ciclo de verificación — resultado final APROBADO.
- 15 restricciones absolutas (REST-TK-001 a REST-TK-015)
- 10 excepciones operacionales (EXC-TK-001 a EXC-TK-010)
- 8 extensiones futuras (EXT-TK-001 a EXT-TK-008)
- 11 DUDAs en la tabla §10 referenciadas (TK-001 a TK-016 relevantes)
- 2 nuevas DUDAs registradas: DUDA-TK-016 (rol "Técnico" no definido en SDD) y DUDA-PERM-001 (cambio de sucursal post-creación no definido en SDD)

Próximos documentos pendientes de inicio: EVENTS.md y VALIDATIONS.md

---

## Módulo Tickets — PERMISSIONS.md (Documento #4 del ciclo Document-First)
Fecha: 2026-07-09
Estado: CREADO — Aprobado en ciclo QA posterior (ver entrada anterior)
Ubicación: `docs/modules/tickets/PERMISSIONS.md`

Contenido del documento creado:
- 10 secciones completas con estructura canónica del módulo.
- 4 roles del SDD documentados: SuperAdministrador, Administrador, Trabajador, Solicitante.
- 33 acciones documentadas individualmente (§4.1 a §4.33).
- Permisos por estado del ticket: 9 estados documentados (§5.1 a §5.9).
- Matriz consolidada (§6): 33 filas × 4 roles.
- 15 restricciones absolutas: REST-TK-001 a REST-TK-015.
- 10 excepciones operacionales: EXC-TK-001 a EXC-TK-010.
- 8 extensiones futuras: EXT-TK-001 a EXT-TK-008.
- 9 DUDAs existentes (DUDA-TK-001 a DUDA-TK-012 relevantes) referenciadas en el texto.
- 2 nuevas DUDAs registradas: DUDA-TK-016 (rol "Técnico" no definido en SDD) y DUDA-PERM-001 (cambio de sucursal post-creación no definido en SDD).

Archivos adicionales actualizados:
- `docs/modules/tickets/README.md` — DUDA-TK-016 y DUDA-PERM-001 añadidas a la sección "Decisiones pendientes".

Estado final: APROBADO QA el 2026-07-09 (ver entrada "PERMISSIONS.md APROBADO" al inicio de este archivo).

---

## Módulo Tickets — Fase 1 (Dominio)
Fecha: 2026-07-08
Estado: COMPLETADO — APROBADO QA (3 ciclos de corrección, todos los defectos corregidos)
Ubicación: `docs/modules/tickets/`

Documentos aprobados (2 de 2 — Fase 1):
- README.md — Punto de entrada del módulo, alcance, decisiones pendientes (7 DUDAs en tabla + 2 adicionales)
- DOMAIN.md — 838 líneas, 12 secciones, 31 términos en glosario, 12 DUDAs registradas (DUDA-TK-001 a DUDA-TK-012)

DUDAs abiertas: 12 (DUDA-TK-001 a DUDA-TK-012). Las más críticas para avanzar a Fase 2:
- DUDA-TK-001: ¿El Trabajador puede crear tickets? — bloquea PERMISSIONS.md
- DUDA-TK-004: ¿Cuál prioridad es la efectiva? — bloquea BUSINESS-RULES.md
- DUDA-TK-006: ¿La validación es siempre obligatoria? — bloquea BUSINESS-RULES.md y FLOWS.md
- DUDA-TK-007: ¿Un ticket tiene un solo Trabajador o puede tener co-responsables? — bloquea DATABASE.md
- DUDA-TK-009: ¿Existe un estado Cancelado? — bloquea BUSINESS-RULES.md y PERMISSIONS.md

Pendiente: Fase 2 (BUSINESS-RULES.md, PERMISSIONS.md, EVENTS.md, VALIDATIONS.md) — bloqueada hasta resolución de DUDAs críticas por el usuario.

---

## Módulo Catálogos del Sistema (Documentación completa)
Fecha: 2026-07-08
Estado: COMPLETADO — APROBADO QA
Ubicación: `docs/modules/system-catalogs/`

Documentos aprobados (8 de 8):
- README.md — Punto de entrada del módulo, alcance, módulos relacionados
- DOMAIN.md — Entidades (Catálogo, TipoCatálogo, ElementoCatálogo), atributos, estados, relaciones y glosario
- BUSINESS-RULES.md — BR-SC-001 a BR-SC-027 (27 reglas de negocio)
- PERMISSIONS.md — Matriz de permisos por rol; DUDAs sobre nivel de acceso del Admin
- EVENTS.md — 10 eventos de dominio del ciclo de vida del catálogo
- VALIDATIONS.md — Validaciones de creación, edición, activación/desactivación, borrado lógico y reordenamiento
- FLOWS.md — FL-SC-001 a FL-SC-009 (9 flujos de ciclo de vida)
- FUTURE.md — FUT-SC-001 a FUT-SC-010 (10 funcionalidades futuras)

Pendiente de decisión del usuario: 6 DUDAs bloqueantes (DUDA-SC-001 a DUDA-SC-006). Ninguna de ellas puede resolverse sin intervención del usuario. Bloquean DATABASE.md y API.md del módulo.

---

## Módulo Tipos de Servicio (Documentación completa)
Fecha: 2026-07-08
Estado: COMPLETADO — APROBADO QA

Documentos aprobados (8 de 8):
- README.md — Punto de entrada del módulo, alcance, módulos relacionados
- DOMAIN.md — Entidades, atributos, estados, relaciones, glosario, registro DUDA-TS-001 a DUDA-TS-006
- BUSINESS-RULES.md — BR-TS-001 a BR-TS-018 (18 reglas de negocio en 5 categorías)
- PERMISSIONS.md — Matriz de permisos por rol; DUDA-TS-001 y DUDA-TS-002
- EVENTS.md — 6 eventos de dominio: service_type.created/updated/activated/deactivated/deleted/reordered
- VALIDATIONS.md — Validaciones de creación, edición, cambio de estado, borrado lógico, reordenamiento, asignación
- FLOWS.md — FL-TS-001 a FL-TS-007 (7 flujos de ciclo de vida)
- FUTURE.md — FUT-TS-001 a FUT-TS-010

Pendiente de decisión del usuario: DUDA-TS-001 (global vs. por empresa). Bloquea DATABASE.md y API.md.

## Iteración 3.21 — Módulo Áreas (Documentación completa)
Fecha: 2026-07-08
Estado: COMPLETADO — APROBADO QA

Documentos aprobados (8 de 8):
- README.md — Punto de entrada del módulo
- DOMAIN.md — Entidades, atributos, glosario, registro canónico DUDA-AR-001 a DUDA-AR-013
- BUSINESS-RULES.md — BR-AR-001 a BR-AR-024 (24 reglas de negocio)
- FLOWS.md — FL-AR-001 a FL-AR-006 (6 flujos de ciclo de vida)
- VALIDATIONS.md — Validaciones de creación, edición, cambio de estado, borrado lógico
- PERMISSIONS.md — Matriz de permisos por rol; DUDA-AR-012, DUDA-AR-013
- EVENTS.md — 7 eventos de dominio: area.created, updated, activated, deactivated, deleted, manager_assigned, config_updated
- FUTURE.md — FUT-AR-001 a FUT-AR-010

Pendiente de decisión del usuario: DUDA-AR-001 (relación área-sucursal). Bloquea DATABASE.md y API.md.

## Iteración 3.18 — Documentación de Negocio del Módulo Empresas (8 archivos)

- **2026-07-07** — `docs/modules/companies/README.md` (nuevo): visión general del módulo — propósito, alcance, entidades principales (Empresa, ConfiguraciónEmpresa, IdentidadVisual), dependencias con sucursales, usuarios, tickets y auditoría, y punto de entrada a la documentación completa.
- **2026-07-07** — `docs/modules/companies/DOMAIN.md` (nuevo): entidad Empresa con todos sus atributos, entidad ConfiguraciónEmpresa, entidad IdentidadVisual; relaciones con otros módulos; glosario del dominio. Dudas DUDA-CO-001 (mono-tenant vs multi-tenant) y DUDA-CO-002 documentadas con impacto en arquitectura.
- **2026-07-07** — `docs/modules/companies/BUSINESS-RULES.md` (nuevo): 15 reglas de negocio catalogadas BR-CO-001 a BR-CO-015. Referencia corregida en BR-CO-013: "BE-008" reemplazado por "SEC-004" tras rechazo de QA (C-003).
- **2026-07-07** — `docs/modules/companies/PERMISSIONS.md` (nuevo): matriz de permisos por rol (SuperAdministrador, Administrador, Trabajador, Usuario) para todas las operaciones del módulo. DUD-CO-001 documentada en sección de dudas pendientes.
- **2026-07-07** — `docs/modules/companies/VALIDATIONS.md` (nuevo): validaciones funcionales en lenguaje de negocio sin código. 3 secciones: creación de empresa, edición de empresa, configuración de empresa. Contradicción sobre obligatoriedad de País/Zona horaria resuelta alineando con DOMAIN.md (campos opcionales con fallback al sistema).
- **2026-07-07** — `docs/modules/companies/EVENTS.md` (nuevo): 6 eventos de dominio — company.created, company.updated, company.deactivated, company.reactivated, company.config_updated, company.deleted — con payloads, módulos suscriptores y efectos. Evento company.deleted añadido tras rechazo de QA (C-001).
- **2026-07-07** — `docs/modules/companies/FLOWS.md` (nuevo): 6 flujos FL-CO-001 a FL-CO-006. FL-CO-006 (borrado lógico de empresa) añadido tras rechazo de QA (C-001). Dudas pendientes DUD-CO-004 a DUD-CO-006 documentadas.
- **2026-07-07** — `docs/modules/companies/FUTURE.md` (nuevo): 8 funcionalidades futuras planificadas FUT-CO-001 a FUT-CO-008.
- **2026-07-07** — Ciclo QA completado: rechazo inicial con 3 críticos (C-001 faltaba FL-CO-006 y company.deleted; C-002 contradicción en campos opcionales; C-003 referencia rota en BR-CO-013). Los 3 críticos corregidos. Estado final: aprobado con observaciones menores pendientes.
- **2026-07-07** — Dudas abiertas que requieren decisión del usuario: DUDA-CO-001 (mono vs multi-tenant, bloqueante para DATABASE.md), DUD-CO-001 (permisos de edición del Admin sobre su empresa), DUD-CO-004 (comportamiento de desactivación con tickets activos), QA-DUD-001 (notificación al desactivar empresa), QA-DUD-002 (evento para cambios de identidad visual).
- **2026-07-07** — No creados: DATABASE.md y API.md (pendiente aprobación de documentos de negocio y resolución de DUDA-CO-001).

## Iteración 3.17 — Documentación de Negocio del Módulo Usuarios (8 archivos)

- **2026-07-07** — `docs/modules/users/README.md` (nuevo): visión general del módulo — alcance, propósito, dependencias con otros módulos (tickets, notificaciones, auditoría) y punto de entrada a la documentación completa.
- **2026-07-07** — `docs/modules/users/DOMAIN.md` (nuevo): entidad Usuario con todos sus atributos, enumerados Rol (SuperAdministrador, Administrador, Trabajador, Usuario) y EstadoLaboral (Activo, Inactivo, Suspendido), relaciones con entidades de otros módulos y glosario del dominio.
- **2026-07-07** — `docs/modules/users/BUSINESS-RULES.md` (nuevo): 11 reglas de negocio BR-US-001 a BR-US-011. BR-US-011 añadida durante QA: un usuario con EstadoLaboral Inactivo o Suspendido no puede recibir nuevas asignaciones de tickets.
- **2026-07-07** — `docs/modules/users/PERMISSIONS.md` (nuevo): matriz de permisos por rol (SuperAdministrador, Administrador, Trabajador, Usuario) para cada operación del módulo. Fila de borrado lógico añadida. Duda pendiente CRIT-02 documentada en sección "Dudas pendientes".
- **2026-07-07** — `docs/modules/users/VALIDATIONS.md` (nuevo): reglas de validación reescritas como documentación de negocio pura (sin código Zod). FL-US-005 separada en FL-US-005A (contraseña inicial en creación) y FL-US-005B (cambio de contraseña por el propio usuario). Requisito de contraseña en minúsculas añadido per SDD §16.3.
- **2026-07-07** — `docs/modules/users/EVENTS.md` (nuevo): 5 eventos de dominio — UsuarioCreado, UsuarioModificado, UsuarioDesactivado, UsuarioBorradoLogico, SucursalUsuarioCambiada — con payloads completos y efectos en otros módulos. OBS-04 corregida (consistencia en receptor de notificación por cambio de sucursal).
- **2026-07-07** — `docs/modules/users/FLOWS.md` (nuevo): 5 flujos principales FL-US-001 (Crear Usuario) a FL-US-005 (Restablecer Contraseña). CRIT-01 resuelto: desactivación (FL-US-003) y borrado lógico (FL-US-004) son operaciones claramente distintas. CRIT-02 documentado como duda pendiente con ambas interpretaciones.
- **2026-07-07** — `docs/modules/users/FUTURE.md` (nuevo): 6 funcionalidades planificadas FUT-US-001 a FUT-US-006 (autenticación 2FA, OAuth, historial de accesos, suspensión temporal, perfil de habilidades, importación masiva por CSV).
- **2026-07-07** — Ciclo QA completado: CRIT-01 (desactivación vs borrado lógico), CRIT-03 (restricción de auto-modificación de rol), CRIT-04 (requisito de contraseña en minúsculas) — resueltos. CRIT-02 documentado como duda pendiente. OBS-01 a OBS-08 gestionadas.
- **2026-07-07** — No creados: DATABASE.md y API.md (pendiente aprobación de los documentos de negocio por el usuario).

## Iteración 3.16 — Nueva Estructura Documental Modular

- **2026-07-07** — `docs/README.md` (nuevo): índice maestro de toda la documentación del proyecto. Punto de entrada único para la estructura documental completa.
- **2026-07-07** — `docs/METHODOLOGY.md` (nuevo): define la metodología Document-First obligatoria. Especifica el orden de trabajo (Dominio → Reglas → Flujos → Permisos → Eventos → Validaciones → Aprobación → DB → API → Backend) y las reglas de aprobación previas a la implementación.
- **2026-07-07** — `docs/architecture/` (8 READMEs nuevos): frontend, backend, database, security, notifications, events, permissions, integrations. Documentación de cada capa arquitectónica del sistema.
- **2026-07-07** — `docs/modules/tickets/` (11 archivos nuevos): documentación completa del módulo de tickets — dominio, reglas, flujos, permisos, eventos, validaciones, DB, API, backend, UI, tests.
- **2026-07-07** — `docs/modules/users/` (11 archivos nuevos): documentación completa del módulo de usuarios.
- **2026-07-07** — `docs/modules/notifications/` (11 archivos nuevos): documentación completa del módulo de notificaciones.
- **2026-07-07** — `docs/modules/dashboard/` (11 archivos nuevos): documentación completa del módulo de dashboard.
- **2026-07-07** — `docs/modules/audit/` (11 archivos nuevos): documentación completa del módulo de auditoría.
- **2026-07-07** — `docs/modules/reports/` (11 archivos nuevos): documentación completa del módulo de reportes.
- **2026-07-07** — `docs/modules/settings/` (11 archivos nuevos): documentación completa del módulo de configuración.
- **2026-07-07** — `docs/modules/companies/` (11 archivos nuevos): documentación completa del módulo de empresas.
- **2026-07-07** — `docs/modules/branches/` (11 archivos nuevos): documentación completa del módulo de sucursales.
- **2026-07-07** — `docs/modules/areas/` (11 archivos nuevos): documentación completa del módulo de áreas.
- **2026-07-07** — `docs/modules/comments/` (11 archivos nuevos): documentación completa del módulo de comentarios.
- **2026-07-07** — `docs/modules/attachments/` (11 archivos nuevos): documentación completa del módulo de adjuntos/evidencias.
- **2026-07-07** — `docs/modules/assignments/` (11 archivos nuevos): documentación completa del módulo de asignaciones.
- **2026-07-07** — `docs/modules/service-types/` (11 archivos nuevos): documentación completa del módulo de tipos de servicio.
- **2026-07-07** — `docs/modules/permissions/` (11 archivos nuevos): documentación completa del módulo de permisos.
- **2026-07-07** — `docs/decisions/ADR-001 a ADR-009` (9 archivos nuevos): Architecture Decision Records — decisiones arquitectónicas con contexto, alternativas consideradas y consecuencias.
- **2026-07-07** — Total: ~185 archivos markdown creados. Sin commit requerido (solo documentación).
- **2026-07-07** — Cambio de metodología aprobado: a partir de esta iteración toda implementación requiere documentación previa del módulo. El ciclo Document-First es ahora parte de la Definición de Terminado.

## Iteración 3.15 — Arquitectura de Notificaciones Multi-Canal

- **2026-07-07** — `src/features/notifications/core/` (8 archivos): `NotificationEventBus` — bus de eventos desacoplado para publicar/suscribir eventos de notificación entre módulos sin acoplamiento directo. `NotificationService` — servicio central que coordina ciclo de vida de notificaciones (creación, enrutamiento, persistencia, estado). `NotificationDispatcher` — orquestador de despacho multi-canal según preferencias del usuario y disponibilidad del canal. `NotificationFactory` — fábrica tipada para construir objetos `Notification` desde payloads de eventos. `NotificationRegistry` — registro central de tipos de notificación, metadatos de canal y prioridades de entrega. `NotificationQueue` — cola con soporte de reintentos y control de concurrencia por canal. `NotificationTemplates` — plantillas core reutilizables (texto plano, HTML, markdown).
- **2026-07-07** — `src/features/notifications/channels/` (10 canales + index): `Internal` (completamente funcional), `Push` (listo para solicitar permiso Web Push), `Email`, `SMS`, `WhatsApp`, `Teams`, `Slack`, `Discord`, `Telegram`, `Webhook` (stubs con interfaz `INotificationChannel` implementada). Barrel `index.ts` re-exporta todos los canales.
- **2026-07-07** — `src/features/notifications/store/preferences.store.ts` (nuevo): Store Zustand con persistencia en localStorage para preferencias de notificación del usuario — canales habilitados, horarios de no molestar, frecuencia de resumen.
- **2026-07-07** — `src/features/notifications/store/notifications.store.ts` (nuevo): Store Zustand para estado de notificaciones en tiempo real — lista, contadores no leídos, estado por ítem.
- **2026-07-07** — `src/features/notifications/hooks/useNotificationService.ts` (nuevo): Hook que expone el `NotificationService` a componentes React con lazy initialization y limpieza automática de suscripciones al desmontar.
- **2026-07-07** — `src/features/notifications/hooks/useNotificationPreferences.ts` (nuevo): Hook para leer y actualizar preferencias de notificación del usuario desde `preferences.store.ts`.
- **2026-07-07** — `src/features/notifications/hooks/useNotificationPermission.ts` (nuevo): Hook que gestiona el estado del permiso push (`default | granted | denied`), solicitud al usuario y persistencia.
- **2026-07-07** — `src/features/notifications/components/PushPermissionBanner.tsx` (nuevo): Banner contextual para solicitar permiso de notificaciones push. Se oculta automáticamente si el permiso ya fue concedido o denegado.
- **2026-07-07** — `src/features/notifications/components/NotificationPreferences.tsx` (nuevo): Componente de UI para configurar preferencias de notificación por canal, consumido por `SettingsPage`.
- **2026-07-07** — `src/features/notifications/index.ts` (nuevo): Barrel que re-exporta todos los elementos públicos de la feature (core, canales, stores, hooks, componentes).
- **2026-07-07** — `src/features/notifications/service-worker/README.md` (nuevo): Documentación de estructura y responsabilidades del Service Worker para notificaciones push futuras (integración con Fase 4).
- **2026-07-07** — `src/features/settings/pages/SettingsPage.tsx` (modificado): Sección "Preferencias de Notificaciones" añadida usando el componente `NotificationPreferences`.
- **2026-07-07** — `.claude/notification-architecture.md` (nuevo): Documento interno de referencia de la arquitectura — decisiones de diseño, contratos de interfaces, guía de extensión de canales.
- **2026-07-07** — TypeScript: 0 errores. Arquitectura desacoplada, extensible y lista para integración real con Supabase Realtime y Web Push API en Fase 4.

## Fase 1 — Inicialización

- **2026-06-29** — Proyecto React 19 + Vite + TypeScript scaffolded manualmente.
- **2026-06-29** — TypeScript strict configurado (`noUnusedLocals`, `noUnusedParameters`, `noImplicitReturns`, `noFallthroughCasesInSwitch`).
- **2026-06-29** — ESLint 9 (flat config) + Prettier 3 + prettier-plugin-tailwindcss.
- **2026-06-29** — TailwindCSS 3 (Mobile First) + variables CSS para estados de tickets.
- **2026-06-29** — `components.json` de shadcn/ui configurado (listo para `npx shadcn add`).
- **2026-06-29** — React Router DOM 6 con router base y layout vacío.
- **2026-06-29** — TanStack Query 5 + Zustand 5 + React Hook Form 7 + Zod 3.
- **2026-06-29** — Cliente Supabase JS 2 (`src/services/supabase.ts`).
- **2026-06-29** — `.env.example` con todas las variables requeridas por el SDD.
- **2026-06-29** — Alias de importación configurados en `vite.config.ts` y `tsconfig.json`.
- **2026-06-29** — Estructura `src/` completa: app, features (9), shared (9 dirs), services, hooks, store, types, utils, constants, lib, styles, assets.
- **2026-06-29** — ThemeProvider (dark/light/system) + useTheme hook separados.
- **2026-06-29** — Zustand stores: `auth.store.ts` y `ui.store.ts`.
- **2026-06-29** — Inter font, favicon SVG, meta tags PWA Mobile First en `index.html`.
- **2026-06-29** — Vitest 2 + Testing Library 16 + 5 tests unitarios pasando.
- **2026-06-29** — Playwright 1 configurado (Desktop Chrome + Pixel 5 + iPhone 12).
- **2026-06-29** — Husky 9 + lint-staged 15 (pre-commit hook activo).
- **2026-06-29** — Build de producción: ✅ 1.93s, 97.78 kB gzip.
- **2026-06-29** — Vulnerabilidad moderada (esbuild dev-server) documentada; no afecta producción.

## Iteración 3.14 — UserEditPage ancho completo, Configuración restringida a admin, modal Cambiar Estado, gestión de empresas/sucursales en UsersPage, filtros y gráficos en ReportsPage, restructuración de ProfilePage

- **2026-07-07** — `UserEditPage.tsx`: `mx-auto max-w-xl` removido del Card principal — formulario ocupa el ancho completo disponible. Correo + Teléfono agrupados en grid `sm:grid-cols-2`. Empresa + Sucursal agrupadas en grid `sm:grid-cols-2`. Props `error={!!errors.name}`, `error={!!errors.apellido}`, `error={!!errors.correo}` añadidos a los Inputs para mostrar borde rojo en campos inválidos.
- **2026-07-07** — `AppHeader.tsx`: variable `isAdmin` añadida (`user?.rol === 'admin' || user?.rol === 'superadmin'`). Item "Configuración" en el dropdown del header envuelto en condicional `{isAdmin && (...)}`. Ahora consistente con AppSidebar donde ya estaba restringido a `adminNav`.
- **2026-07-07** — `TicketDetailPage.tsx`: Modal "Cambiar estado" rediseñado — clase `ps-glow-modal` añadida al DialogContent, tamaño ampliado a `max-w-sm`, descripción muestra el estado actual mediante `<StatusBadge />`, cada opción muestra borde azul + `bg-primary/10` al estar seleccionada + ícono `<Check />` al final, botón "Confirmar cambio" deshabilitado si `pendingStatus === localStatus`. Import `Check` añadido desde lucide-react.
- **2026-07-07** — `TicketDetailPage.tsx`: Confirmación antes de cancelar ticket — nuevo estado local `cancelDialog`. El botón "Cancelar ticket" ahora abre un `ConfirmDialog` destructivo con mensaje de confirmación explícito en lugar de navegar directamente.
- **2026-07-07** — `UsersPage.tsx`: Sección "Empresas" con botón "Nueva empresa", ícono `Building2` por fila, badge activa/inactiva, dropdown por fila (Editar + toggle activación). Sección "Sucursales" con selector de empresa para filtrar, botón "Nueva sucursal", dropdowns por fila con estado activa/inactiva. Estados locales añadidos: `localSucursales`, `localAreas`, `selectedEmpresaFilter`, `toggleEmpresaTarget`, `toggleAreaTarget`. Handlers: `handleToggleEmpresa()` y `handleToggleArea()`. 2 nuevos ConfirmDialogs para toggling de activación. Imports añadidos: `Pencil`, `Power`, `Building2`, `MapPin`.
- **2026-07-07** — `ReportsPage.tsx`: Sección de filtros añadida (Desde, Hasta tipo date, Empresa select, Estado select) con botones "Aplicar filtros" y "Limpiar". Botón "Ver todo"/"Ver toda" (variante ghost con ícono `ArrowRight`) añadido al header de cada gráfico existente. Nuevo gráfico "Tendencia semanal" — AreaChart con gradiente azul usando datos `byWeek`. Nuevo gráfico "Por estado" — PieChart donut con colores semánticos por estado usando datos `byStatus`. Nuevos imports: `AreaChart`, `Area`, `Filter`, `X`, `ArrowRight`, `Input`, selects.
- **2026-07-07** — `ProfilePage.tsx`: Enlace "Cambiar foto" (texto bajo el avatar) eliminado. Campo "Usuario" añadido en columna izquierda bajo empresa/sucursal, renderizado condicionalmente si `user.usuario` existe. Card "Apariencia" movida de columna derecha a columna izquierda (debajo del card de perfil, con `mt-4`). Sección de solo lectura (Empresa/Sucursal/Usuario) eliminada de "Información personal" junto con su Separator. Correo + Teléfono agrupados en grid `sm:grid-cols-2`.
- **2026-07-07** — TypeScript: 0 errores. Preview verificado en todas las páginas modificadas.

## Iteración 3.13 — Favicon logo.jpeg, "Asignado a / Por:" en tickets y renombre de columna

- **2026-07-06** — `index.html`: favicon cambiado de `favicon.svg` a `/logo.jpeg`. El ícono del navegador ahora muestra el logo real del producto.
- **2026-07-06** — `MyTicketsPage.tsx`: helper `getAssignedBy(ticket)` añadido — extrae el autor de la asignación desde `ticket.history.find(h => h.action === 'Ticket asignado')?.author`.
- **2026-07-06** — `MyTicketsPage.tsx` (tarjeta mobile): campo "Asignado a" muestra ahora dos líneas: "Asignado a: [nombre]" y "Por: [nombre]" (quién realizó la asignación).
- **2026-07-06** — `MyTicketsPage.tsx` (tabla desktop): columna "Asignado a" incluye ahora la sub-línea "Por: [nombre]" debajo del nombre del responsable. Columna "Fecha" renombrada a "F. creación".
- **2026-07-06** — TypeScript: 0 errores. Preview verificado en mobile 375px y desktop 1280px.

## Iteración 3.12 — Branding, badge de notificaciones, bordes de cards y menú de acciones de tickets

- **2026-07-06** — `index.html`: `<title>` y `apple-mobile-web-app-title` corregidos de "Pidde" a "Pidde Servicio".
- **2026-07-06** — `AppSidebar.tsx`: atributo `alt` de logo y `<span>` de nombre de marca corregidos a "Pidde Servicio".
- **2026-07-06** — `AuthLayout.tsx`: texto de logo en panel de marca (desktop) y en logo mobile actualizados a "Pidde Servicio".
- **2026-07-06** — `AppLayout.tsx`: título fallback en barra de aplicación actualizado a "Pidde Servicio".
- **2026-07-06** — Badge de notificaciones: color cambiado de `bg-primary` (azul) a `bg-destructive` (rojo) en `AppSidebar.tsx`, `MobileNav.tsx` y `AppHeader.tsx`. Número centrado añadiendo `flex items-center justify-center`. Cap aumentado de `9+` a `99+` (muestra "99+" cuando count > 99).
- **2026-07-06** — `src/shared/ui/card.tsx`: borde por defecto actualizado de `border` a `border border-primary/25` (borde azul semitransparente en todas las cards del sistema).
- **2026-07-06** — `src/styles/globals.css`: clase `ps-glow-card` intensificada — opacidades del glow subidas de `0.25/0.15` a `0.45/0.3` para mayor visibilidad del efecto fosforescente.
- **2026-07-06** — `MyTicketsPage.tsx`: filtros móviles reorganizados — search full-width en fila 1; Estado+Prioridad y Desde+Hasta en grid 2 columnas (fila 2); Limpiar full-width (fila 3). Desktop: filtros en una sola fila sin cambios. Técnica: `grid grid-cols-2 gap-2 lg:contents` + outer `flex flex-col gap-2 lg:flex-row`. Inputs de fecha con `w-full` en mobile para evitar corte del icono de calendario.
- **2026-07-06** — `MyTicketsPage.tsx`: menú de acciones de tickets reestructurado. Removidos del menú: "Cambiar estado", "Cambiar prioridad", "Ver historial" (pertenecen al detalle del ticket). "Modificar ticket" y "Asignar responsable" ahora visibles solo para admin/superadmin. "Ver detalle" mantiene el ícono `Eye`. Añadido "Ver historial de cambios" (todos los roles) con ícono `History` — abre un Dialog con historial mock de modificaciones del ticket.
- **2026-07-06** — `MyTicketsPage.tsx`: nuevos imports `Eye` y `History` de lucide-react. Estado `historyTicketId` añadido para controlar el Dialog de historial de cambios.
- **2026-07-06** — TypeScript: 0 errores. Preview verificado: badge rojo centrado, filtros 2 columnas en mobile, `lg:contents` activo en desktop, menú de acciones correcto por rol.

## Iteración 3.10/3.11 — Modificar Ticket mejorado como Sheet lateral con todos los campos

- **2026-07-06** — `MyTicketsPage.tsx`: reemplazado `EditTicketDialog` (Dialog limitado a 4 campos) por `EditTicketSheet` (Sheet lateral completo). El sheet edita todos los campos del ticket: tipo de servicio, título, empresa, sucursal, prioridad (botones visuales con colores semánticos y estado seleccionado), descripción, estado y asignado a. Sección primaria con fondo `bg-primary/5` y label "INFORMACIÓN PRINCIPAL"; sección secundaria con label "CAMPOS ADICIONALES". Dependencia Empresa→Sucursal aplicada (sucursal deshabilitada hasta seleccionar empresa). Validaciones en español para campos obligatorios. Footer fijo con Cancelar + Guardar cambios. Imports añadidos: `Sheet, SheetContent, SheetHeader, SheetTitle` (desde `@shared/ui/sheet`), `cn` (desde `@lib/utils`), `MOCK_SUCURSALES, MOCK_AREAS, TICKET_TYPES` (desde `@mocks/data`). Import removido: `DialogDescription`. TypeScript: 0 errores. Prueba visual confirmada: sheet abre, pre-pobla todos los campos, selección de prioridad funciona.

## Iteración 3.7 — Gap Audit: refinamiento visual y consistencia transversal

- **2026-07-06** — `DashboardPage`: `ps-glow-card` aplicado en todas las chart cards (antes solo KPI). Consistencia visual completa entre secciones de métricas y gráficos.
- **2026-07-06** — `DashboardPage`: botón "Ver todos" agregado en todas las secciones de gráficos.
- **2026-07-06** — `globals.css`: `ps-glow-card` mejorado — borde sólido 1px + difuminado externo box-shadow más pronunciado.
- **2026-07-06** — `MobileNav`: migrado de `flex-1` a `grid-cols-5` para centrado matemáticamente perfecto.
- **2026-07-06** — Labels obligatorio/opcional: revisión exhaustiva y completado en LoginPage, SettingsPage, UsersPage (modales) y todos los formularios restantes.
- **2026-07-06** — `ProfilePage`: badges de rol con colores semánticos por rol (SuperAdministrador/rojo, Administrador/azul, Trabajador/amarillo, Usuario/secundario).
- **2026-07-06** — Theme validation: sistema dark-only verificado como consistente en todos los componentes. Sin valores hardcodeados.

## Fase 3.4 — Flujo interactivo completo (modales y acciones)

- **2026-06-29** — `AppLayout.tsx`: restaurado `pt-4 lg:pt-5` en `<main>`; `pb-16` → `pb-20` en móvil. El layout controla el gap del header; las páginas individuales no lo tocan.
- **2026-06-29** — `providers/index.tsx`: `defaultTheme="system"` → `"dark"`. La clase `dark` queda activa en `<html>` siempre, activando todos los `dark:` variants de Tailwind.
- **2026-06-29** — `UsersPage.tsx`: reescritura mayor (+768 líneas). `MOCK_USERS` como estado local mutable. `UserRow` recibe callbacks (`onView`, `onEdit`, `onDelete`, `onToggleStatus`, `onResetPw`). Modales implementados: Crear Usuario (nombre/apellido/correo/telefono/rol/sucursal/area con validación), Editar Usuario (pre-poblado), Ver Perfil (lectura). `ConfirmDialog` para Eliminar, Cambiar Estado (activar/desactivar) y Restablecer Contraseña. Filtros y counts sobre estado local reactivo.
- **2026-06-29** — `TicketDetailPage.tsx`: refactor mayor (+710 líneas). Estado local: `localStatus`, `localPriority`, `localAssignedTo`, `localComments`, `localEvidencias`. Modal Asignar Responsable (lista de trabajadores activos filtrada). Modal Cambiar Estado para admins. Botón Comentar actualiza `localComments` con nuevo objeto. Adjuntar Evidencia simulado. Botones Reabrir/Cancelar/Confirmar cierre conectados con handlers y toasts.
- **2026-06-29** — `ProfilePage.tsx`: guardado de datos con `toast.success`. Validación de contraseña (campos requeridos, coincidencia, mínimo 8 chars). Cambio de foto simulado. Switches de preferencias con estado local.
- **2026-06-29** — `SettingsPage.tsx`: reescritura (+309 líneas). Todos los `Switch` con `useState` local reactivo y `onCheckedChange` con toast. `Select`s de configuración con estado local. Botones Guardar por sección. `ConfirmDialog` para restablecer configuración.
- **2026-06-29** — `NotificationsPage.tsx`: notificaciones en estado mutable. Marcar individual como leída. Marcar todas como leídas. Eliminar notificación. Contadores reactivos sobre estado local. Tabs filtran del estado local.
- **2026-06-29** — `ReportsPage.tsx`: estado `downloading`. Botones de descarga con `toast.promise` (1.5s delay simulado). Botones deshabilitados durante descarga.
- **2026-06-29** — `AuditPage.tsx`: botón Exportar con `toast.promise` (2s delay). Mensaje de éxito "Auditoria exportada como audit-log.xlsx".
- **2026-06-29** — Validaciones: Arquitecto aprobado, QA aprobado, UX Lead aprobado, TypeScript 0 errores, Git Guardian ejecutado, Release Reviewer aprobado. Commit: `dc4c1b8`.

## Fase 3.3 — Consistencia visual, scroll horizontal y tipografía unificada

- **2026-06-29** — `globals.css`: escala tipográfica canónica (`.ps-page-title`, `.ps-card-label`, `.ps-body`, `.ps-meta`, `.ps-label`, `.ps-badge-text`, `.ps-table-header`, `.ps-table-cell`, `.ps-nav-item`, `.ps-mono`). `word-break: break-word` en body.
- **2026-06-29** — `TicketDetailPage.tsx`: fix scroll horizontal — causa raíz tabs `flex` sin `flex-1`. Fix: `flex-1 items-center justify-center` garantiza 1/3 del ancho. `CardContent` → `p-3 pt-0`, `CardHeader` → `px-3 pt-3 pb-2`.
- **2026-06-29** — `CreateTicketPage.tsx`: `CardContent` con `p-3 pt-0`. Upload zone `py-4 px-4`. Título de página `text-base`, `CardTitle` → `text-[11px] uppercase tracking-widest`.
- **2026-06-29** — `UsersPage`, `NotificationsPage`, `ProfilePage`, `ReportsPage`, `AuditPage`, `SettingsPage`: normalización completa — padding, títulos `text-base`, `CardTitle` `text-[11px] uppercase`, labels `text-xs`, metadatos `text-[11px] text-muted-foreground`.
- **2026-06-29** — TypeScript: 0 errores | ESLint: 0 warnings. Commit: `ed91387`, `82462be`.

## Fase 3.2 — Design System Oscuro + Dashboard Graph-First

- **2026-06-29** — Paleta oscura corporativa aplicada: `--background 228 19% 5%`, `--card 240 6% 7%`, `--border 240 6% 16%`.
- **2026-06-29** — Fuente Geist configurada (fonts.bunny.net), Inter eliminado de index.html.
- **2026-06-29** — `tailwind.config.ts`: `fontFamily.sans` → Geist stack.
- **2026-06-29** — Paquetes instalados: `geist ^1.7.2`, `@dnd-kit/core ^6.3.1`, `@dnd-kit/sortable ^10.0.0`, `@dnd-kit/utilities ^3.2.2`.
- **2026-06-29** — Dashboard Admin reescrito: 9 widgets con 7 tipos de gráfico distintos (Donut, Bar vertical, Bar horizontal, Area con gradiente, Stacked Bar, RadarChart, ComposedChart Line+Bar).
- **2026-06-29** — Interactividad dashboard: clic en sector/barra → `navigate('/tickets?status=X')` etc.
- **2026-06-29** — Drag & Drop con `@dnd-kit`: modo edición con GripVertical, persistencia en `sessionStorage('ps-dashboard-order')`.
- **2026-06-29** — `MyTicketsPage`: `useSearchParams` para inicializar filtros desde URL + banner "Filtrado desde el dashboard".
- **2026-06-29** — AppSidebar: borde izquierdo en ítem activo, `bg-primary/15 text-primary`.
- **2026-06-29** — TypeScript: 0 errores | Commits: `5ebd756`, `d5f9385`.

## Fase 3.1 — Refinamiento Visual y UX

- **2026-06-29** — Escala tipográfica reducida en todas las páginas.
- **2026-06-29** — Dashboard Admin: AreaChart con gradiente, íconos en stat cards, sección byPriority.
- **2026-06-29** — LoginPage: Card wrapper, inputs `h-9`, labels `text-xs`.
- **2026-06-29** — AuthLayout: gradiente de fondo, bullets `CheckCircle2`.
- **2026-06-29** — Padding Mobile First reducido en todas las páginas.
- **2026-06-29** — TypeScript: 0 errores | Commit: `49a91df`.

## Fase 0 — Infraestructura

- **2026-06-29** — Análisis de la estructura existente del proyecto.
- **2026-06-29** — Creación de `CLAUDE.md` (raíz) con protocolo de sesión, reglas y punteros de memoria.
- **2026-06-29** — `.gitignore` poblado (dependencias, builds, secretos, logs, IDE/SO).
- **2026-06-29** — Memoria `.claude/`: `prompts/` (project-summary, coding-rules, architecture-rules).
- **2026-06-29** — Memoria `.claude/context/` (architecture, database, frontend, backend, permissions, notifications, workflows, decisions).
- **2026-06-29** — Memoria `.claude/progress/`, `.claude/tasks/`, `.claude/validations/`.
- **2026-06-29** — `project-status.json` inicial.
- **2026-06-29** — Subagentes en `.claude/agents/` (arquitecto, dba, backend, frontend, ui-designer, qa, reviewer, documentation-manager, git-guardian, release-reviewer, recovery-manager).
- **2026-06-29** — Skill `control-de-commits` (Git Guardian + Release Reviewer + Recovery Manager).
- **2026-06-29** — `.claude/WORKFLOW.md` (flujo de trabajo y reglas de commits).
- **2026-06-29** — `.claude/settings.json` + hooks (recordatorio de protocolo de sesión).
- **2026-06-29** — Comandos de apoyo en `.claude/commands/`.
