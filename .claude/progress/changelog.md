# Changelog

> Cambios significativos del proyecto. Formato: fecha — descripción. Se actualiza tras cada commit.

## 2026-07-11 — FASE 7.0 Hardening: Auditoría y 35+ correcciones aplicadas

8 agentes especializados auditaron Frontend, Backend, Seguridad (OWASP), Performance, Accesibilidad (WCAG 2.2 AA) y UX. Correcciones críticas: crash silencioso en StatusBadge, roles inválidos en 16 archivos, vulnerabilidad path traversal en upload, [Authorize] faltante en /auth/me, 6 validators faltantes, debounce en búsqueda, lazy loading, compresión HTTP. Sistema al 62% de preparación para producción. Bloqueante principal: L001 (Supabase Auth Hook).

---

## [2026-07-11] FASE 6.6 — Integración Completa del Sistema

**Frontend:** 13 páginas migradas de mocks a API real. Servicios creados: ticketService, usuarioService, notificacionService, configuracionService, auditoriaService. Hooks TanStack Query para todas las operaciones CRUD y mutations. ROL_COLORS/ROL_LABELS actualizados con los 6 roles del sistema (superadmin, admin, supervisor, tecnico, trabajador, usuario). Infraestructura de integración: apiClient.ts con interceptores JWT, auth.store actualizado con accessToken, authService, SessionRestorer en providers, proxy Vite /api, supabase/config.toml, .env.local y .env.example.

**Backend:** Compilación exitosa en net10.0 (0 errores). 57/57 handlers Application verificados. Auth hook skeleton creado en supabase/functions/auth-hook/index.ts. launchSettings.json creado. global.json con rollForward: latestMajor. Directory.Build.props con net10.0.

**QA Docs:** docs/testing/INTEGRATION_TESTS.md (84 casos de prueba), docs/testing/KNOWN_LIMITATIONS.md (9 limitaciones documentadas, L001 bloqueante: JWT claims hook), docs/deployment/DEPLOYMENT_GUIDE.md (guía completa de deployment).

**Pendiente bloqueante (L001):** Configurar Custom JWT Claims Hook en Supabase Dashboard para que el backend pueda identificar al usuario autenticado. Sin esto, /auth/me retorna 401 aunque el token sea válido.

---

## [2026-07-11] FASE 6.5 — Capa API del Backend

### Creado — Controllers (13 archivos)
- `Controllers/Common/ApiControllerBase.cs` — base controller con Mediator lazy, TraceId, HandleResult, HandleCreated, OkPaged, MapFailure
- `Controllers/V1/Auth/AuthController.cs` — GET /api/v1/auth/me vía ICurrentUserService
- `Controllers/V1/Empresas/EmpresasController.cs` — CRUD + ToggleActiva
- `Controllers/V1/Sucursales/SucursalesController.cs` — CRUD + ToggleActiva
- `Controllers/V1/Areas/AreasController.cs` — CRUD + ToggleActiva
- `Controllers/V1/Roles/RolesController.cs` — GET roles, permisos, permisosPorRol
- `Controllers/V1/Configuracion/ConfiguracionController.cs` — GET parametros, PUT por clave
- `Controllers/V1/Usuarios/UsuariosController.cs` — CRUD + CambiarRol + CambiarEstadoLaboral + Activar + Desactivar
- `Controllers/V1/Tickets/TicketsController.cs` — 15 endpoints (CRUD + 10 transiciones de estado)
- `Controllers/V1/Tickets/ComentariosController.cs` — CRUD comentarios anidados en ticket
- `Controllers/V1/Tickets/EvidenciasController.cs` — GET list + POST multipart + DELETE evidencias
- `Controllers/V1/Notificaciones/NotificacionesController.cs` — Listar + Conteo + MarcarLeida + MarcarTodasLeidas
- `Controllers/V1/Auditoria/AuditoriaController.cs` — Listar logs con filtros

### Modificado — Extensions
- `Extensions/ServiceCollectionExtensions.cs` — SwaggerGen mejorado con JWT Bearer, 5 políticas de autorización (Autenticado / SoloSuperAdmin / AdminOSuperior / SupervisorOSuperior / Tecnico), Rate Limiting 100 req/min por IP (built-in .NET 8 SlidingWindow — sin paquete adicional)
- `Extensions/ApplicationBuilderExtensions.cs` — UseRateLimiter() agregado antes de UseAuthentication

### Patrones
- Clean Architecture: Controllers → MediatR → Application (sin SQL ni lógica en Controllers)
- Respuestas uniformes: ApiResponse<T> / PagedResponse<T> de PideServicio.Contracts
- JWT Auth vía Supabase; policies por claim "rol"
- API Versioning por segmento de URL: /api/v1/

### Documentación
- `docs/backend/API.md` — referencia completa de todos los endpoints y políticas de autorización
- `docs/backend/API_CONVENTIONS.md` — convenciones REST del proyecto

### QA
- APROBADO

### Próximo
- FASE 6.6: Testing / validación de build e integración Frontend-Backend

---

## [2026-07-11] FASE 6.4 — Capa Infrastructure del Backend

### Creado — Persistence Layer (19 repositorios)
- `EmpresaRepository`, `SucursalRepository`, `AreaRepository` — jerarquía organizacional
- `RolRepository`, `PermisoRepository` — RBAC
- `TipoServicioRepository`, `CategoriaRepository`, `MotivoCancelacionRepository`, `MotivoRechazoRepository`, `ParametroRepository` — catálogos
- `TecnicoSucursalRepository`, `UsuarioRepository` — usuarios e identidad
- `TicketRepository`, `TicketHistorialRepository`, `TicketComentarioRepository`, `TicketEvidenciaRepository`, `TicketAsignacionRepository` — ciclo de vida de tickets
- `NotificacionRepository`, `AuditLogRepository` — trazabilidad
- `UnitOfWork` — implementa `IUnitOfWork` vía `NpgsqlTransaction`
- `EntityReconstituter` — helper para entidades con constructores privados (reflection)
- `DbConnectionFactory` — actualizado a `NpgsqlDataSourceBuilder`

### Creado — Infrastructure Services (7 servicios)
- `AuditService` (`IAuditService`)
- `NotificationService` (`INotificationService`)
- `EmailService` (`IEmailService` — stub SMTP)
- `PushNotificationService` (`IPushNotificationService` — stub)
- `SupabaseStorageService` (`IStorageService` vía HTTP REST)
- `SerilogConfiguration` — extension de `WebApplicationBuilder`
- `CacheService` + `CacheOptions` — wrapper de `IMemoryCache`

### Paquetes NuGet agregados
- `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Enrichers.Environment`

### Dependency Injection
- `PideServicio.Persistence.DependencyInjection`: 19 repositorios + `UnitOfWork`
- `PideServicio.Infrastructure.DependencyInjection`: todos los servicios + cache + storage

### Corregido (revisión final)
- `Infrastructure.csproj`: atributos `Version=""` eliminados (incompatibles con central package management)
- `UsuarioRepository.ActualizarAsync`: optimistic locking — `WHERE version = @Version AND deleted_at IS NULL`, `version = version + 1`

### Documentación
- `docs/backend/INFRASTRUCTURE.md` creado

### QA
- APROBADO

### Próximo
- FASE 6.5: Capa API — Controllers, Endpoints, DTOs, Middleware, Swagger, error handling global, health checks

---

## [2026-07-10] FASE 6.3 — Capa Application del Backend

### Creado
- 169 archivos C# en `backend/src/PideServicio.Application/Features/`
- 11 módulos completos con Commands, Queries, Handlers, DTOs y Mappings:
  - Empresas (16): CreateEmpresa, UpdateEmpresa, ToggleEmpresaActiva, GetEmpresaById, ListEmpresas
  - Sucursales (16): CreateSucursal, UpdateSucursal, ToggleSucursalActiva, GetSucursalById, ListSucursales
  - Areas (16): CreateArea, UpdateArea, ToggleAreaActiva, GetAreaById, ListAreas
  - Comentarios (12): CrearComentario, EditarComentario, EliminarComentario, ListComentarios
  - Evidencias (10): SubirEvidencia, EliminarEvidencia, ListEvidencias
  - Notificaciones (11): MarcarNotificacionLeida, MarcarTodasLeidas, ListNotificaciones, GetConteoNoLeidas
  - Auditoria (4): ListAuditLogs
  - Configuracion (9): UpdateParametro, GetParametros, GetParametroPorClave
  - Usuarios (24): CreateUsuario, UpdateUsuarioPerfil, CambiarRol, CambiarEstadoLaboral, DesactivarUsuario, ActivarUsuario, GetUsuarioById, ListUsuarios
  - Roles (9): GetRoles, GetPermisos, GetPermisosPorRol
  - Tickets (42): CrearTicket, AsignarTicket, ReasignarTicket, IniciarProceso, PauseParaEspera, ReanudarDesdeEspera, SubmitParaValidacion, CerrarTicket, ReabrirTicket, CancelarTicket, CambiarPrioridad, CambiarArea, GetTicketById, ListTickets, GetTicketHistorial
- Archivos comunes:
  - `Common/Models/Result.cs` con ResultTipo enum discriminado, `ListResult.cs`, `PagedResult.cs`
  - `Common/CQRS/ICommandHandler.cs`, `IQueryHandler.cs`
  - `Common/Interfaces/IUnitOfWork.cs`, `IEmailService.cs`, `IPushNotificationService.cs`, `IStorageService.cs`
  - `Common/Interfaces/Repositories/` — 19 interfaces de repositorios
  - `Common/Mappings/MappingConfig.cs` — auto-scan via `Assembly.GetExecutingAssembly()`
- Documentación: `docs/backend/APPLICATION_LAYER.md`

### Patrones
- CQRS con MediatR 12.2 — ICommand<T>/IQuery<T> → handler → Result<T>
- FluentValidation 11.9 — un validator por Request, sin llamadas a BD en validadores
- Mapster 7.4 — IRegister por módulo, auto-scan en MappingConfig
- Result<T> discriminado con ResultTipo enum (Success, NotFound, Conflict, Forbidden, Validation, Error)
- Capa Application depende SOLO de Domain: sin Controllers, sin EF Core, sin SQL directo

### Próximo
- FASE 6.4: Capa Infrastructure — CurrentUserService (Supabase JWT), DateTimeService, StorageService, EmailService, PushNotificationService

---

## [2026-07-10] FASE 6.2 — Capa Domain del Backend

### Creado
- 20 entidades de dominio en `backend/src/PideServicio.Domain/Entities/`:
  - `Empresa`, `Sucursal`, `Area` — jerarquía organizacional multi-tenant
  - `Rol`, `Permiso`, `RolePermission` — RBAC granular
  - `TipoServicio`, `Categoria`, `MotivoRechazo`, `MotivoCancelacion`, `Parametro` — catálogos del sistema
  - `TecnicoSucursal` — asignación multi-sucursal de técnicos (tabla puente N:M)
  - `AuditLog`, `Notificacion` — trazabilidad y notificaciones
  - `Usuario` (AggregateRoot) — identidad con vinculación a Supabase Auth
  - `Ticket` (AggregateRoot) — máquina de estados completa con todas las transiciones del SDD §5
  - `TicketAsignacion`, `TicketHistorialEntrada`, `TicketEvidencia`, `TicketComentario`
- Value Objects completados/corregidos:
  - `Email.cs` — corrección de validación trailing dot detectada en QA
  - `TicketCodigo.cs` — formato PS-XXXXXX
- Documentación: `docs/backend/DOMAIN_MODEL.md` (referencia completa del modelo de dominio)

### Detectado y corregido en QA
- 2 hallazgos bloqueantes corregidos
- 3 hallazgos moderados corregidos
- 2 hallazgos menores corregidos

### Próximo
- FASE 6.3: Capa Application — Commands, Queries, Handlers (MediatR), DTOs, interfaces de repositorios

---

## [2026-07-10] FASE 6.1 — Arquitectura del Backend

### Creado
- Solución PideServicio.sln con 6 proyectos en Clean Architecture (.NET 8)
- 72 archivos totales (C#, csproj, json, sln, editorconfig, global.json)
- Domain: 9 enums + 3 entidades base + 5 excepciones + IDomainEvent
- Contracts: ApiResponse<T>, ApiError, PagedResponse<T>, PaginacionParams
- Application: ICommand/IQuery (MediatR CQRS), Result<T>, CurrentUser, 3 behaviors, 6 interfaces
- Infrastructure: CurrentUserService (lazy JWT), DateTimeService, opciones de configuración
- Persistence: DbConnectionFactory (Npgsql+Dapper), DatabaseOptions
- Api: Program.cs, GlobalExceptionMiddleware, CorrelationIdMiddleware, 3 appsettings
- Tests: ArchitectureTests con 5 pruebas de dependencias entre capas
- Documentación: docs/backend/ (3 documentos de referencia)

### Corregido (post-QA)
- CurrentUserService: resolución lazy de ClaimsPrincipal
- Api.csproj: referencia explícita a Domain
- Directory.Build.props: TreatWarningsAsErrors=true para Release
- PerformanceBehavior: umbral externalizado a PerformanceOptions
- appsettings.json: secciones SMTP y Performance
- /health endpoint: protegido con RequireAuthorization()

### Próximo
- FASE 6.2: Módulo Usuarios

---

## 2026-07-10 — Iteración 4.10 — Migración 010: Seguridad, RLS, Storage y Realtime — QA APROBADO (0 críticos, 4 menores)

### Sin commit — base de datos (SQL + documentación)

### Agregado
- CREADO: `supabase/migrations/010_security_storage_rls.sql` (1977 líneas)
  - 0 tablas nuevas — migración de infraestructura pura
  - 13 funciones: 7 auxiliares RLS (SECURITY DEFINER STABLE SET search_path=public) + 6 trigger de negocio
  - 5 triggers: guard_inmutable (BEFORE UPDATE), validate_transition (BEFORE UPDATE OF estado), after_estado (AFTER UPDATE OF estado), after_insert (AFTER INSERT), check_rechazo (BEFORE INSERT ticket_historial)
  - ENABLE ROW LEVEL SECURITY en 21 tablas
  - FORCE ROW LEVEL SECURITY en 3 tablas: ticket_historial, ticket_asignaciones, audit_logs
  - 56 políticas RLS implementando RLS_MATRIX.md por tabla y rol
  - 4 Storage buckets: tickets-evidence (privado 25MB), avatars (privado 5MB), system-assets (público 10MB), email-images (privado service_role)
  - 6 Storage policies en storage.objects
  - Realtime habilitado en 4 tablas: tickets, ticket_comentarios, ticket_historial, notificaciones
  - 0 seeds nuevos (330 filas ya existentes de M-002/M-003)
  - Idempotente: CREATE OR REPLACE (funciones), DROP IF EXISTS + CREATE (triggers y políticas), ON CONFLICT DO NOTHING (buckets), DO block con EXCEPTION (Realtime)
- CREADO: `docs/database/MIGRATION-010.md`
  - 12 secciones, 17 queries QA, rollback en 5 niveles
  - Matriz completa de transiciones de estado documentada (validate_ticket_transition)
  - Decisiones de diseño: tecnico_id vs tecnico_asignado_id, email-images service_role exclusivo, FORCE RLS semántica, orden de triggers por nombre

### QA
- APROBADO — 0 críticos, 4 menores
- OBS-001: email-images sin storage policies — RESUELTO (comentario añadido al SQL confirmando diseño intencional)
- OBS-002/004: RLS_MATRIX.md tiene errores tipográficos (tecnico_asignado_id, empresa_id en audit_logs) — errores en doc, migración correcta. Tarea registrada para corrección separada.
- OBS-003: is_admin_or_above() creada pero reservada para M-011/Edge Functions — no es un error

### Impacto
- Base de datos con seguridad completa activada (21 tablas con RLS)
- Storage listo para carga de evidencias y avatares
- Realtime activado para actualizaciones en tiempo real de la PWA
- Triggers de negocio activos: validación de transiciones, registro de historial automático, guard de inmutabilidad
- Pendiente M-011: sla_configuraciones + feriados + FK diferida tickets.sla_id

## 2026-07-10 — Iteración 4.9 — Migración 009: notificaciones + audit_logs — QA APROBADO (0 críticos, 0 menores)

### Sin commit — base de datos (SQL + documentación)

### Agregado
- CREADO: `supabase/migrations/009_audit_notifications.sql`
  - 2 tablas, 1 trigger (reutilizado de M-002), 0 nuevas funciones, 0 ENUMs nuevos
  - `notificaciones`: 16 cols, mutable, canal_notificacion_tipo ENUM, estado_entrega_tipo ENUM, tipo_evento VARCHAR libre, destinatario_id ON DELETE CASCADE, sin deleted_at, metadata JSONB, 4 FKs, 3 índices (1 partial)
  - `audit_logs`: 15 cols, completamente inmutable, actor desnormalizado, entidad_id polimórfico sin FK, ip INET, sucursal_id para RLS futura, 2 FKs, 6 índices
  - 6 FKs activas totales, 0 diferidas, 0 seeds, idempotente (DROP TRIGGER IF EXISTS + CREATE TRIGGER)
- CREADO: `docs/database/MIGRATION-009.md`
  - 12 secciones, 16 queries QA, rollback en 3 niveles
  - §6 naturaleza diferenciada (mutable vs inmutable en misma migración)
  - §7 justificación de los 9 índices incluyendo compuesto de 3 cols para badge de notificaciones
  - §9 preparación para Push/Email/WhatsApp/Teams/Slack/Webhooks/BI/IA sin modificar esquema

### QA
- APROBADO — 0 críticos, 0 menores

## 2026-07-10 — Iteración 4.8 — Migración 008: ticket_comentarios + ticket_evidencias — QA APROBADO

### Sin commit — base de datos (SQL + documentación)

### Agregado
- CREADO: `supabase/migrations/008_comments_evidence.sql`
  - 2 tablas, 0 triggers, 0 ENUMs nuevos (evidencia_tipo reutilizado de M-002)
  - `ticket_comentarios`: 11 cols, parcialmente mutable, es_interno BOOLEAN, editado_en (no updated_at), soft delete, 5 FKs activas, 3 índices (1 partial)
  - `ticket_evidencias`: 12 cols, append-only para contenido, url_almacenamiento TEXT (Storage), evidencia_tipo ENUM, soft delete, 4 FKs activas, 3 índices (1 partial)
  - 9 FKs activas totales, 0 diferidas, 0 seeds, idempotente
- CREADO: `docs/database/MIGRATION-008.md`
  - 12 secciones, 15 queries QA, rollback en 3 niveles
  - §6 Principio mutabilidad diferenciada (parcialmente mutable vs append-only para contenido)
  - §7 Justificación idx_comentarios_internos (PARTIAL) e idx_evidencias_final_activas (PARTIAL)
  - §9 Preparación para Storage, Signed URLs, Antivirus/IA/OCR, historial ediciones, versionado, RLS, Realtime

### QA
- APROBADO — 0 críticos, 1 menor (ON UPDATE CASCADE omitido, patrón consistente M-007)

## 2026-07-10 — Iteración 4.7 — Documentación MIGRATION-007.md: ticket_asignaciones + ticket_historial

### Sin commit — solo documentación

### Agregado
- CREADO: `docs/database/MIGRATION-007.md`
  - 12 secciones, 14 queries QA, rollback en 3 niveles
  - Tabla `ticket_asignaciones`: 9 cols, append-only (sin updated_at/deleted_at/version), CHECK chk_asignaciones_reasignacion, 5 FKs activas, 4 índices BTREE (incluyendo partial idx_asignaciones_reasignaciones)
  - Tabla `ticket_historial`: 12 cols, append-only, tipo_evento ENUM (9 valores), metadata JSONB, actor_id nullable (sistema puede insertar sin actor), 4 FKs activas, 6 índices (4 BTREE + 2 PARTIAL + 1 GIN sobre metadata)
  - §5 Principio Append-Only: diferencia con audit_logs, razón de ausencia de updated_at/deleted_at/version, inmutabilidad por service_role
  - §7 Alternativas consideradas: tabla única (elegida) vs tablas separadas por tipo de evento (descartada)
  - §8 Restricciones diferidas: trg_historial_check_rechazo, RLS INSERT-only, validación consistencia tipo_evento
  - §9 Preparación para SLA (idx_historial_estado_cambiado), BI (GIN metadata), ticket_comentarios (M-008), audit_logs (diferencia de capas)

### Pendiente
- SQL `supabase/migrations/007_ticket_modules.sql` — por generar

## 2026-07-10 — Iteración 4.6 — Migración 006: tipos_servicio + tickets (SQL + Documentación) — QA APROBADO

### Sin commit — pendiente autorización del usuario

### Agregado
- CREADO: `supabase/migrations/006_tickets.sql`
  - Tabla `tipos_servicio`: 12 cols, sin version, empresa_id NOT NULL, FK activa → empresas RESTRICT, 4 índices (2 regulares + 2 UNIQUE parciales), trg_fn_set_updated_at_only()
  - Tabla `tickets`: 36 cols, version optimistic locking, codigo VARCHAR(10) DEFAULT ('PS-'||LPAD(nextval(...)::TEXT,6,'0')), prioridad_efectiva NOT NULL sin DEFAULT, estado DEFAULT 'NUEVO'
  - 2 triggers (trg_fn_set_updated_at_only en tipos_servicio, trg_fn_set_updated_at en tickets)
  - 12 FKs activas, 4 FKs diferidas (tipos_servicio.created/updated/deleted_by + tickets.sla_id → M-0031)
  - 2 CHECKs (valoracion BETWEEN 1 AND 5, tiempo_estimado_min > 0)
  - 10 índices en tickets (empresa_estado, sucursal_estado, tecnico_estado, solicitante, area_estado, fecha_creacion DESC, fecha_resolucion ASC, activos parcial, sla_vencidos parcial, prioridad sobre prioridad_efectiva)
- CREADO: `docs/database/MIGRATION-006.md`
  - 12 secciones, 13 queries QA, rollback en 3 niveles
  - §5 Estrategia DEFAULT expression (no trigger) para codigo PS-XXXXXX
  - §6 D-015: prioridad_efectiva columna física, COALESCE calculado por Backend
  - §7 Justificación individual de 14 índices (4 tipos_servicio + 10 tickets)
  - §8 9 restricciones de negocio diferidas (trigger guard inmutabilidad + business rules)

### QA
- APROBADO: 0 críticos, 4 menores (inconsistencias en MODEL-PHYSICAL.md — congelado, sin modificar)
- 36 columnas tickets verificadas, 12/12 FKs activas, 10/10 índices tickets, 2/2 triggers, 3/3 CHECKs

## 2026-07-10 — Iteración 4.5 (Documentación) — MIGRATION-005.md: Usuarios, Preferencias y Alcance Territorial

### Sin commit — solo documentación

### Agregado
- CREADO: `docs/database/MIGRATION-005.md`
  - 11 secciones, 13 queries QA, rollback en 3 niveles
  - §4.1 Tabla `usuarios`: 22 columnas, version, auth_id inmutable, 3 funciones trigger nuevas
  - §4.2 Tabla `preferencias_notificacion`: 17 columnas, trigger de creación automática
  - §4.3 Tabla `tecnico_sucursales`: 9 columnas, guard de unicidad de es_principal
  - §5 Funciones trigger: guard_auth_id, after_insert, guard_principal (con fundamento técnico)
  - §6 Integración con Supabase Auth: flujo create-user, FK CASCADE cross-schema
  - §7 Estados del usuario: combinaciones activo × estado_laboral
  - §8 Escalabilidad multi-tenant: empresa_id como eje de RLS, idx_usuarios_activos parcial
  - 7 índices explícitos, 13 FKs activas (incluyendo FK cross-schema a auth.users), 0 seeds, 0 FKs diferidas

### Estado
- Fase 4: documentación de migración 005 completada
- Pendiente: generar `supabase/migrations/005_usuarios.sql`
- Próxima documentación: MIGRATION-006.md (tipos_servicio, sla_configuraciones, feriados)

---

## 2026-07-10 — Iteración 4.4 — Migración 004: Estructura Organizacional

### Sin commit — migración SQL (pendiente ejecución en Supabase)

### Agregado
- CREADO: `supabase/migrations/004_organizational_structure.sql`
  - Tablas: empresas, sucursales, areas
  - 3 triggers (trg_fn_set_updated_at_only), 7 índices explícitos
  - 2 FKs activas (empresa→sucursal→area ON DELETE RESTRICT)
  - 11 FKs a usuarios diferidas → M-0031
  - Sin seeds, sin RLS, sin version
- CREADO: `docs/database/MIGRATION-004.md`
  - 10 secciones, 5 decisiones técnicas, 12 queries QA
  - Diagrama de jerarquía multiempresa

### Estado
- Fase 4: 4 migraciones completadas (001, 002, 003 y 004)
- Migración 004: COMPLETADA — QA APROBADO (observación menor corregida)
- Próxima: 005_users_identity.sql (usuarios + tecnico_sucursales + preferencias_notificacion)

---

## 2026-07-10 — Iteración 4.3 — Migración 003: Seguridad e Identidad

### Sin commit — migración SQL (pendiente ejecución en Supabase)

### Agregado
- CREADO: `supabase/migrations/003_security_identity.sql`: Tercera migración real. 1 tabla: role_permissions (junction RBAC roles×permisos con override opcional por empresa); 4 índices: idx_role_perm_rol, idx_role_perm_permiso, idx_role_perm_empresa, uq_role_perm (NULL-safe); FKs activas a roles(codigo) y permisos(id); FKs diferidas para empresas y usuarios (M-0031); 234 filas seed RBAC para 6 roles: SUPERADMIN(63) + ADMIN(60) + SUPERVISOR(60) + TECNICO(19) + TRABAJADOR(17) + USUARIO(15).
- CREADO: `docs/database/MIGRATION-003.md`: Documentación técnica con propósito, dependencias, resumen de objetos, definición de columnas, diseño RBAC, 5 decisiones técnicas (DT-003-01 a DT-003-05), rollback en 2 niveles y 12 queries QA.

### Estado
- Fase 4: 3 migraciones completadas (001, 002 y 003)
- Migración 003: COMPLETADA — QA APROBADO (8/8 secciones sin errores críticos)
- Próxima: 004_company_hierarchy.sql (empresas + sucursales + areas, jerarquía multi-tenant con RLS base)

---

## 2026-07-10 — Iteración 4.2 — Migración 002: Catálogos Maestros del Sistema

### Sin commit — migración SQL (pendiente ejecución en Supabase)

### Agregado
- `supabase/migrations/002_master_catalogs.sql`: Segunda migración real (1 función auxiliar: trg_fn_set_updated_at_only para catálogos sin columna version; 9 ENUMs: rol_tipo, estado_laboral_tipo, ticket_estado_tipo, prioridad_tipo, evidencia_tipo, canal_notificacion_tipo, estado_entrega_tipo, tipo_dato_parametro_tipo, tipo_evento_historial_tipo; secuencia ticket_codigo_seq para numeración PS-000001 a PS-999999; 6 tablas: roles, permisos, categorias, motivos_cancelacion, motivos_rechazo, parametros_sistema; 6 triggers BEFORE UPDATE; 12 índices incluyendo NULL-safe uniqueness con UUID centinela; 96 filas seed: 6 roles + 63 permisos + 9 categorías + 5 motivos_cancelacion + 5 motivos_rechazo + 8 parametros_sistema)
- `docs/database/MIGRATION-002.md`: Documentación técnica con justificación de diseño, QA y rollback

### Estado
- Fase 4: 2 migraciones completadas (001 y 002)
- Migración 002: COMPLETADA
- Próxima: 003_role_permissions.sql (tabla junction roles×permisos + seeds RBAC completos)

---

## 2026-07-10 — Iteración 4.1 — Primera Migración SQL

### Sin commit — migración SQL (pendiente ejecución en Supabase)

### Agregado
- `supabase/migrations/001_initial_setup.sql`: Primera migración real (4 extensiones: uuid-ossp, pgcrypto, unaccent, pg_trgm; 1 extensión condicional: pg_cron con bloque DO EXCEPTION; función global trg_fn_set_updated_at() reutilizable para 20 tablas; COMMENT ON SCHEMA public con convenciones del proyecto)
- `docs/database/MIGRATION-001.md`: Documentación técnica con justificación de extensiones, QA y rollback

### Estado
- Fase 4 INICIADA
- Migración 001: APROBADA — idempotente, lista para ejecutar
- Próxima: 002_enums.sql (9 tipos ENUM del sistema)

---

## 2026-07-10 — Iteración 3.37 — Documentos Técnicos Pre-Migración

### Sin commit — documentación pura (no requiere commit de código)

### Agregado
- `docs/database/NAMING_CONVENTIONS.md`: 13 secciones, convenciones para todas las categorías SQL (tablas, columnas, constraints, índices, funciones, triggers, políticas RLS, vistas, Storage, Edge Functions, Supabase, seeds, comentarios)
- `docs/database/INDEX_STRATEGY.md`: 50+ índices documentados con justificación individual por tabla, índices parciales, compuestos, NULL-safe y checklist de FK coverage
- `docs/database/RLS_MATRIX.md`: Matriz completa SA/AD/SU/TE/TR/US × 23 tablas × 4 operaciones (SELECT/INSERT/UPDATE/DELETE), detalle de políticas por tabla, reglas especiales y checklist de implementación

### Decisión
- Modelo físico CONGELADO por instrucción del usuario: sin nuevas entidades, renombres ni decisiones de arquitectura adicionales hasta indicación contraria

### Estado
- Fase 4 (Migraciones SQL): LISTA PARA INICIAR
- Documentación de BD: COMPLETA y CONGELADA

---

## 2026-07-10 — Iteración 3.36 — Plan de Migraciones

### Sin commit — documentación pura (no requiere commit de código)

**Archivo creado:** `docs/database/MIGRATION-PLAN.md`

### Agregado
- `docs/database/MIGRATION-PLAN.md`: Plan completo de implementación de la base de datos.
  - 14 fases de migración (F-0 a F-13), 44 migraciones planificadas.
  - 25 tablas mapeadas con sus dependencias.
  - 9 ENUMs + 1 secuencia.
  - 12 funciones planificadas y 12 triggers planificados.
  - Estrategia de seeds: obligatorios, opcionales y SuperAdmin.
  - Estrategia de RLS con orden de activación por tabla.
  - Estrategia de Storage: 4 buckets (`ticket-evidencias`, `empresa-logos`, `ticket-thumbnails`, `avatars`).
  - Estrategia de Realtime: 4 tablas habilitadas.
  - Estrategia de Rollback en 3 niveles (migración individual, fase completa, rollback total).
  - 6 grupos de riesgos identificados con mitigaciones.
  - Validación de escalabilidad (multi-empresa, nuevos módulos, Power BI, IA).
  - Checklist pre-implementación completo.

### Estado
- Documentación de BD: COMPLETA (Conceptual + Lógico + Físico + Contrato Definitivo + Plan de Migraciones).
- Fase 4 (Migraciones SQL): LISTA PARA INICIAR.

---

## 2026-07-10 — Resolución de 7 Dudas Arquitectónicas + Validación Final MODEL-PHYSICAL.md (Iteración 3.35)

### Sin commit — documentación pura (no requiere commit de código)

**Archivo modificado:** `docs/database/MODEL-PHYSICAL.md`

Resolución definitiva de las 7 Dudas Arquitectónicas que bloqueaban la Fase 4. Decisiones D-011 a D-017 incorporadas al contrato. Total tablas: 25 (de 21 a 25: +`roles`, +`permisos`, +`role_permissions`, +`tecnico_sucursales`, +`feriados`). Validación QA final completada. Veredicto: APROBADO.

**DUDA-ARQ-001 — Supervisor sin implementación funcional en MVP:**
El rol Supervisor permanece en el ENUM `rol_usuario` para no requerir migración futura, pero no tiene lógica de negocio ni permisos activos en el MVP. La diferenciación operativa es: Administrador gestiona, Técnico ejecuta, Trabajador crea.

**DUDA-ARQ-002 — Técnico multi-sucursal:**
Implementado mediante tabla puente `tecnico_sucursales` (N:M entre `usuarios` y `sucursales`). Un Técnico puede estar activo en múltiples sucursales simultáneamente. La tabla incluye `activo BOOLEAN`, `created_at`, `created_by` para auditoría de la asignación.

**DUDA-ARQ-008 — RBAC completo:**
Tres tablas nuevas: `roles` (catálogo de roles del sistema), `permisos` (catálogo de acciones granulares), `role_permissions` (tabla puente roles × permisos con `granted BOOLEAN`). Permite extender permisos sin migraciones de ENUM. El ENUM `rol_usuario` sigue siendo la referencia de autenticación; las tablas RBAC gestionan la autorización granular.

**DUDA-ARQ-009 — audit_logs sin particionamiento:**
La tabla `audit_logs` se crea sin particionamiento en MVP. Se añade particionamiento por rango de fecha (`PARTITION BY RANGE (created_at)`) en la fase de optimización cuando el volumen lo justifique. El índice `idx_audit_logs_created_at` mitiga el rendimiento hasta entonces.

**DUDA-ARQ-010 — prioridad_efectiva como columna física:**
`prioridad_efectiva nivel_prioridad NOT NULL` se almacena como columna física en la tabla `tickets`. Se calcula al crear o actualizar el ticket (lógica en Edge Function `on_ticket_created` y trigger de actualización). No es columna generada (evita restricciones de PostgreSQL sobre columnas generadas con lógica compleja) ni vista (evita recálculo en cada consulta).

**DUDA-ARQ-011 — Solo español en MVP:**
Sin infraestructura i18n en base de datos. Todos los textos del sistema (mensajes de error, estados, etiquetas) se almacenan y devuelven en español. La internacionalización se aplaza a una fase posterior mediante columnas `_es`, `_en` o tabla `traducciones`.

**DUDA-ARQ-012 — SLA en tiempo laboral real con tabla feriados:**
Nueva tabla `feriados` con columnas `fecha DATE`, `descripcion TEXT`, `empresa_id UUID` (NULL = feriado global). El cálculo de SLA excluye fines de semana y los feriados registrados en esta tabla. La función `calcular_horas_laborales(inicio, fin, empresa_id)` implementa la lógica. La tabla `configuracion_sla` almacena `horas_inicio_laboral` y `horas_fin_laboral` por empresa.

**Impacto en Fase 4:**
- Fase 4 DESBLOQUEADA. Todos los bloqueantes arquitectónicos resueltos.
- `docs/database/MODEL-PHYSICAL.md` es el contrato definitivo para las migraciones SQL.
- Las 25 tablas, 9 ENUMs, 1 secuencia y 40+ índices están completamente especificados.

---

## 2026-07-10 — Arquitectura de Base de Datos — Documento Contrato Definitivo completado (Iteración 3.34)

### Sin commit — documentación pura (no requiere commit de código)

**Archivo creado:** `docs/database/MODEL-PHYSICAL.md`

- 21 tablas con detalle completo por tabla: objetivo de negocio, descripción técnica, responsabilidad en el sistema, columnas con tipos PostgreSQL nativos, claves primarias, foráneas, constraints de integridad, validaciones de negocio e índices justificados individualmente.
- 9 ENUMs de PostgreSQL: `rol_usuario`, `estado_laboral`, `estado_ticket`, `nivel_prioridad`, `canal_notificacion`, `estado_notificacion`, `tipo_auditoria`, `estado_sla`, `tipo_motivo_rechazo`.
- 1 secuencia: `ticket_numero_seq` para numeración correlativa global de tickets.
- Estándar completo de auditoría: todas las tablas de negocio incluyen `deleted_by` (quién ejecutó el borrado lógico) y `version` (control de concurrencia optimista).
- Catálogo de 40+ índices con justificación individual: tipo de índice (B-tree, GIN, parcial), consultas que lo aprovechan y cardinalidad estimada.
- Análisis de rendimiento: consultas frecuentes del sistema con plan de ejecución estimado y índices que las optimizan.
- Preparación Supabase completa: políticas RLS por rol en cada tabla, integración Auth (`auth.users`), Storage (bucket `ticket-evidencias` y `empresa-logos`), Realtime (canales `empresa:{id}:tickets` y `ticket:{id}:updates`), Edge Functions (hooks `on_ticket_created`, `on_ticket_status_changed`, `on_ticket_assigned`), Cron Jobs (escalación SLA, limpieza de tokens), Triggers de base de datos.
- Escalabilidad diseñada para millones de tickets: particionamiento de `audit_logs` por rango de fecha, índices compuestos para multi-tenancy, normalización sin sacrificar rendimiento de lectura.
- 7 Dudas Arquitectónicas pendientes de resolución del usuario (§13): DUDA-ARQ-001 (nivel de multi-tenancy), DUDA-ARQ-002 (UUID v4 o ULID como PK), DUDA-ARQ-008 (partición de audit_logs), DUDA-ARQ-009 (prioridad_efectiva como columna generada o vista), DUDA-ARQ-010 (validación de transiciones en BD o solo Edge Functions), DUDA-ARQ-011 (SLA tiempo calendario vs laboral en MVP), DUDA-ARQ-012 (Técnico multi-sucursal).
- Validación QA completa integrada: ciclo de revisión completado dentro del mismo documento.
- Checklist pre-migración de 20 puntos: prerequisitos de entorno, revisión de schema, verificación de ENUMs, seeds, políticas RLS, pruebas de integración.

**Serie de documentos de BD completa:**
- Iteración 3.31 — Modelo Conceptual (`docs/database/MODEL-CONCEPTUAL.md`) — COMPLETADO
- Iteración 3.32 — Modelo Lógico (`docs/database/MODEL-LOGICO.md`) — COMPLETADO
- Iteración 3.33 — Modelo Físico (`docs/database/MODEL-FISICO.md`) — COMPLETADO
- Iteración 3.34 — Documento Contrato Definitivo (`docs/database/MODEL-PHYSICAL.md`) — COMPLETADO

**Próxima acción:** Usuario resuelve las 7 Dudas Arquitectónicas de §13 para habilitar Fase 4 (migraciones SQL en Supabase).

---

## 2026-07-09 — Arquitectura de Base de Datos — Fase 3 — Modelo Físico completado (Iteración 3.33)

### Sin commit — documentación pura (no requiere commit de código)

**Archivo creado:** `docs/database/MODEL-FISICO.md`

- 18 tablas físicas con DDL completo: tipos nativos PostgreSQL, restricciones CHECK, valores por defecto, índices compuestos optimizados para las consultas frecuentes del sistema.
- 9 ENUMs de PostgreSQL: `rol_usuario`, `estado_laboral`, `estado_ticket`, `nivel_prioridad`, `canal_notificacion`, `estado_notificacion`, `tipo_auditoria`, `estado_sla`, `tipo_motivo_rechazo`.
- 2 secuencias: `ticket_numero_seq` (numeración global) y `ticket_numero_empresa_seq` (numeración por empresa).
- 8 funciones PostgreSQL: auditoría automática, cálculo de SLA al crear ticket, registro de historial en cada transición de estado, despacho de notificaciones internas, validación de transiciones permitidas, cálculo de carga del Técnico, actualización de métricas de dashboard y borrado lógico en cascada.
- 9 triggers: asociados a las 8 funciones anteriores, activados en INSERT/UPDATE sobre las tablas correspondientes.
- 5 vistas: `dashboard_metricas` (KPIs en tiempo real), `tickets_activos_vista` (tickets sin cerrar con datos desnormalizados), `sla_alertas` (tickets próximos a vencer SLA), `carga_tecnico` (tickets asignados por Técnico activo), `historial_completo` (unión de historial de ticket y auditoría).
- Políticas RLS: documentadas por tabla y por rol (SuperAdministrador, Administrador, Técnico, Trabajador). Multi-tenant via `empresa_id` en todas las tablas de negocio.
- Integración Supabase Auth: columna `auth_user_id` en `usuarios` referencia `auth.users(id)`. Trigger post-signup crea registro en `usuarios`.
- Integración Supabase Storage: bucket `ticket-evidencias` con política de acceso por `empresa_id`. Bucket `empresa-logos` para identidad visual.
- Integración Supabase Realtime: canales `empresa:{id}:tickets` y `ticket:{id}:updates` para notificaciones en tiempo real.
- Integración Edge Functions: hooks `on_ticket_created`, `on_ticket_status_changed`, `on_ticket_assigned` invocan funciones serverless para notificaciones externas (email, push).
- **Decisión ML-DEC-013 (resolución de DUDA-ML-001):** MotivoRechazo = catálogo configurable en tabla `motivos_rechazo` + columna `comentario_rechazo` libre en `historial_tickets`. Si `motivo_id` corresponde a "Otro", el comentario es obligatorio (validación en trigger y en Edge Function).
- Checklist de validación pre-migración (§14): 6 preguntas pendientes de respuesta del usuario — PF-001 (partición audit_logs), PF-002 (prioridad_efectiva generada vs vista), PF-003 (validación de estado en BD vs solo EF), PF-004 (SLA tiempo calendario vs laboral en MVP), DUDA-ARQ-001 (permisos Supervisor sobre tickets de otras áreas), DUDA-ARQ-002 (Técnico multi-sucursal).

**Las tres fases del Modelo de Base de Datos están completas:**
- Iteración 3.31 — Modelo Conceptual (`docs/database/MODEL-CONCEPTUAL.md`) — COMPLETADO
- Iteración 3.32 — Modelo Lógico (`docs/database/MODEL-LOGICO.md`) — COMPLETADO
- Iteración 3.33 — Modelo Físico (`docs/database/MODEL-FISICO.md`) — COMPLETADO

**Próxima acción:** Aprobación del usuario del Modelo Físico + resolución de los 6 pendientes del checklist §14 para iniciar Fase 4 (migraciones SQL en Supabase).

---

## 2026-07-09 — Arquitectura de Base de Datos — Fase 2 — Modelo Lógico completado (Iteración 3.32)

### Sin commit — documentación pura (no requiere commit de código)

**Archivo creado:** `docs/database/MODEL-LOGICO.md`

- 18 entidades persistentes definidas con atributos y tipos lógicos.
- 2 entidades de solo lectura (Dashboard, Reportes — vistas materializadas, no tablas físicas).
- 1 entidad nueva: ConfiguracionSLA — resolución de DUDA-ARQ-003 (SLA es entidad propia con matriz empresa × tipo_servicio × prioridad, no campo informativo).
- 4 enumeraciones del sistema: Rol, EstadoLaboral, EstadoTicket, NivelPrioridad + 5 enumeraciones adicionales.
- 33 relaciones catalogadas con cardinalidad (1:1, 1:N, N:M) y participación (obligatoria/opcional) explícitas.
- 12 decisiones del modelo lógico: ML-DEC-001 a ML-DEC-012.
- Restricciones documentadas: unicidad (unicidad de correo por empresa, unicidad de área por sucursal, etc.), inmutabilidad (empresa en ticket no cambia post-creación), nulabilidad y workflow.
- Normalización 3FN cumplida con denormalizaciones justificadas documentadas.
- 7 dudas pendientes registradas (DUDA-ML-001 a DUDA-ML-007); ninguna bloquea el Modelo Físico excepto DUDA-ML-001.

**Bloqueante para Modelo Físico:** DUDA-ML-001 — ¿El MotivoRechazo es catálogo configurable (tabla propia) o texto libre (VARCHAR) en el MVP? Requiere decisión del usuario antes de iniciar `docs/database/MODEL-FISICO.md`.

---

## 2026-07-09 — Arquitectura de Base de Datos — Fase 1 — Modelo Conceptual completado (Iteración 3.31)

### Sin commit — documentación pura (no requiere commit de código)

**Archivo creado:** `docs/database/MODEL-CONCEPTUAL.md`

- 7 dominios del sistema documentados: Organización Multi-tenant, Identidad y Acceso, Catálogos, Tickets, Comunicación, Trazabilidad, Analítica.
- 24 entidades documentadas con objetivo, responsabilidad, relaciones e importancia.
- 6 agregados del dominio identificados y justificados (Empresa, Ticket, Usuario, Catálogo, Notificación, AuditoríaLog).
- Dependencias clasificadas: fuertes (bloquean el diseño), débiles (condicionan decisiones), opcionales (pueden diferirse), futuras (fuera del MVP).
- Escalabilidad evaluada para 8 escenarios: multiempresa, IA/ML, ERP, WhatsApp, webhooks, Power BI, app móvil nativa, federated search.
- 8 riesgos arquitectónicos con mitigación propuesta.
- 7 Dudas Arquitectónicas registradas: DUDA-ARQ-001 a DUDA-ARQ-007.
- Veredicto: COMPLETO — listo para Modelo Lógico, condicionado a resolución de DUDA-ARQ-003 (SLA).

**Bloqueante para Modelo Lógico:** DUDA-ARQ-003 — ¿SLA configurables con lógica automatizada o campo informativo en MVP? Requiere decisión expresa del usuario antes de iniciar `docs/database/MODEL-LOGICO.md`.

---

## 2026-07-09 — Módulo Tickets CERRADO — Propagación de 3 decisiones finales en 9 documentos (Iteración 3.30)

### Sin commit — documentación pura (no requiere commit de código)

**9 documentos actualizados:** `docs/modules/tickets/PERMISSIONS.md`, `LIFECYCLE.md`, `DOMAIN.md`, `EVENTS.md`, `VALIDATIONS.md`, `BUSINESS-RULES.md`, `README.md`, `REVIEW.md`, `SUMMARY.md`

Propagación de las 3 decisiones finales del usuario que cerraron el módulo Tickets:

1. **Rol Técnico definido:** El Técnico ejecuta el trabajo asignado. El Trabajador crea y consulta sus propios tickets. Técnico incorporado como rol en todas las 33 acciones de PERMISSIONS.md, en las 9 tablas de estado, en el ciclo de vida completo de LIFECYCLE.md, en todos los eventos de ejecución de EVENTS.md, en las reglas de negocio de BUSINESS-RULES.md y en las validaciones de ejecución de VALIDATIONS.md.

2. **DUDA-TK-012 resuelta:** La categoría es un campo OBLIGATORIO (NOT NULL) en la creación del ticket. VALIDATIONS.md actualizado. VAL-TK-032 corregido.

3. **DUDA-TK-003 resuelta:** El área es editable SOLO cuando el ticket está en estado Sin Asignar, por Admin o SuperAdministrador. En estados operativos existe una excepción documentada con auditoría y motivo obligatorio.

**Ediciones por documento:**
- PERMISSIONS.md: 73 ediciones (1415 → 1466 líneas). Técnico en 33 acciones + 9 estados.
- LIFECYCLE.md: 105 ediciones. En Espera y Cancelado confirmados como estados definitivos del MVP.
- DOMAIN.md: 59 ediciones (836 → 890 líneas). 9 estados en el workflow. Técnico como nuevo actor.
- EVENTS.md: 78 ediciones. EVT-TK-010/011/012/016 activos. EVT-TK-015 N/A. DUDA-EVT-001 resuelta.
- VALIDATIONS.md: 22 ediciones. Categoría obligatoria. Área SOLO editable en Sin Asignar.
- BUSINESS-RULES.md: 47+ ediciones. BR-TK-001 corregido. Técnico como ejecutor en las 82 reglas.
- README.md, REVIEW.md, SUMMARY.md: DUDAs actualizadas. REVIEW.md con veredicto APROBADO para DATABASE.md.

**DUDAs que siguen abiertas (no bloquean DATABASE.md):** DUDA-TK-005, DUDA-TK-008, DUDA-TK-010, DUDA-TK-011, DUDA-PERM-001, DUDA-VAL-001, DUDA-VAL-003, DUDA-VAL-004.

**Estado final del módulo Tickets:** CERRADO. Completamente especificado y listo para DATABASE.md. Requiere aprobación expresa del usuario para iniciar DATABASE.md.

---

## 2026-07-09 — Módulo Tickets — Fase 7 (Consolidación Final) COMPLETADA

### Sin commit — documentación pura (no requiere commit de código)

**Archivos creados:** `docs/modules/tickets/REVIEW.md`, `docs/modules/tickets/SUMMARY.md`

- **REVIEW.md — Auditoría completa del módulo tras resolución de las 31 decisiones del 2026-07-09:**
  - 20 inconsistencias identificadas entre los 7 documentos existentes y las decisiones resueltas. Cada inconsistencia documentada con: identificador, documento afectado, sección, origen de la inconsistencia, descripción y propuesta de resolución.
  - Impacto de las 31 decisiones en cada documento del módulo (README, DOMAIN, BUSINESS-RULES, PERMISSIONS, EVENTS, VALIDATIONS).
  - DUDAs resueltas por las 31 decisiones pero aún no propagadas a los documentos existentes.
  - DUDAs aún abiertas con impacto en implementación: 3 bloqueantes para DATABASE.md (rol Técnico, DUDA-TK-003, DUDA-TK-012).
  - Evaluación de escalabilidad del módulo.
  - Evaluación de calidad de la documentación.
  - Veredicto de preparación para DATABASE.md: NO listo — requiere resolver 3 bloqueantes y propagar 20 inconsistencias.
  - Plan de actualización de los 6 documentos existentes.

- **SUMMARY.md — Resumen ejecutivo para desarrolladores nuevos:**
  - Qué es el módulo, actores (con rol Técnico pendiente de definición).
  - Ciclo de vida del ticket (estados, transiciones).
  - Invariantes críticos del dominio.
  - Entidades del módulo y sus atributos clave.
  - Historial y auditoría.
  - Permisos por rol para las acciones principales.
  - Validaciones críticas antes de implementar.
  - Eventos del dominio y sus consumidores.
  - Integración con otros módulos del sistema.
  - Preparación para implementación (Database, Backend, Frontend, Notificaciones, Dashboard, Reportes).
  - Capacidades futuras planificadas.
  - Pendientes bloqueantes antes de implementar.

**Estado del módulo Tickets tras la Consolidación Final:**
- Documentación funcional: COMPLETA (7 documentos auditados + REVIEW.md + SUMMARY.md).
- Listo para implementación: NO.
- Razón del bloqueo: (1) rol Técnico no especificado en los documentos, (2) 20 inconsistencias por propagar en 6 documentos existentes, (3) DUDA-TK-003 abierta, (4) DUDA-TK-012 abierta.
- Listo para implementación CUANDO: usuario resuelva 3 decisiones bloqueantes + se propaguen las 20 inconsistencias.

---

## 2026-07-09 — Módulo Tickets — VALIDATIONS.md creado (borrador, Fase 2, Documento #6)

### Sin commit — documentación pura (no requiere commit de código)

**VALIDATIONS.md — BORRADOR:** `docs/modules/tickets/VALIDATIONS.md`

- 39 validaciones funcionales: VAL-TK-001 a VAL-TK-039 (creación, estados, campos, adjuntos, comentarios, permisos, SLA, negocio).
- 19 casos especiales: CASE-TK-001 a CASE-TK-019.
- 48 mensajes funcionales en español: MSG-TK-001 a MSG-TK-048 (informativos, advertencias, errores, confirmaciones).
- 7 nuevas DUDAs detectadas durante la elaboración: DUDA-VAL-001 a DUDA-VAL-007 — requieren resolución del usuario antes de DATABASE.md y API.md.
- Estado: BORRADOR — pendiente de aprobación del usuario.
- Metodología Document-First cumplida: sin código, sin referencias técnicas de implementación.
- Siguiente documento: FLOWS.md (Documento #7) — bloqueado hasta aprobación de VALIDATIONS.md.

---

## 2026-07-09 — Módulo Tickets — EVENTS.md aprobado QA (Fase 2, Documento #5)

### Sin commit — documentación pura (no requiere commit de código)

**EVENTS.md — APROBADO:** `docs/modules/tickets/EVENTS.md`

- 21 eventos aplicables: EVT-TK-001 a EVT-TK-021.
- 2 eventos nuevos justificados: EVT-TK-005 (TicketReasignadoSinSucesor) y EVT-TK-011 (TicketRetomado), ambos ausentes en el SDD pero necesarios por el dominio.
- 3 eventos no aplicables en el MVP: EmpresaModificada, ComentarioEditado, ComentarioEliminado — documentados con justificación.
- 20 registros de auditoría: AUD-TK-001 a AUD-TK-020.
- Secciones documentadas: Catálogo de eventos, Matriz de consumidores, Canal de notificaciones (MVP: interna + correo; futuro: push + WhatsApp), Indicadores de dashboard (14), Tablas de reportes (11), Integraciones futuras (6), Escalabilidad y Dudas pendientes.
- 7 DUDAs preexistentes referenciadas con impacto en eventos.
- 1 nueva DUDA registrada: DUDA-EVT-001 (¿el contenido del comentario debe incluirse en el historial del ticket o solo la referencia?).
- Ciclo QA: 9 hallazgos — 0 CRÍTICO, 5 MAYOR, 4 MENOR — todos corregidos y verificados como RESUELTO.
- El documento reemplazó la versión anterior que no cumplía la estructura Document-First canónica.

**Estado del módulo Tickets — Fase 2 tras esta aprobación:**
- BUSINESS-RULES.md: APROBADO (82 reglas, 3 ciclos QA)
- PERMISSIONS.md: APROBADO (33 acciones, 4 roles, 9 estados, 1 ciclo QA)
- EVENTS.md: APROBADO (21 eventos, 20 registros de auditoría, 1 ciclo QA)
- VALIDATIONS.md, FLOWS.md, FUTURE.md: pendientes de inicio

---

## 2026-07-09 — Módulo Tickets — BUSINESS-RULES.md y PERMISSIONS.md aprobados QA (Fase 2)

### Sin commit — documentación pura (no requiere commit de código)

**BUSINESS-RULES.md — APROBADO:**
- 82 reglas de negocio (BR-TK-001 a BR-TK-082).
- 3 ciclos de QA completados: primer ciclo RECHAZADO (13 hallazgos corregidos); segundo ciclo RECHAZADO (5 hallazgos corregidos); tercer ciclo con verificación parcial — APROBADO.
- Corrección crítica H-02: BR-TK-064 (escalación automática) reescrita en tiempo condicional para alinearse con LIFECYCLE.md §10.1 en todos los apartados del documento donde aparecía.
- Documento completo y habilitado como referencia para implementación.

**PERMISSIONS.md — APROBADO:**
- 33 acciones documentadas individualmente (§4.1 a §4.33), 4 roles, 9 estados del ticket.
- 1 ciclo de QA con veredicto "APROBADO CON OBSERVACIONES" (9 hallazgos). Todos corregidos y verificados como RESUELTO en ciclo de verificación — resultado final APROBADO.
- 15 restricciones absolutas (REST-TK-001 a REST-TK-015), 10 excepciones operacionales (EXC-TK-001 a EXC-TK-010), 8 extensiones futuras (EXT-TK-001 a EXT-TK-008).
- 2 nuevas DUDAs registradas: DUDA-TK-016 (rol "Técnico" no definido en SDD) y DUDA-PERM-001 (cambio de sucursal post-creación no definido en SDD).

**Estado del módulo Tickets — Fase 2 tras esta aprobación:**
- BUSINESS-RULES.md: APROBADO
- PERMISSIONS.md: APROBADO
- EVENTS.md, VALIDATIONS.md, FLOWS.md, FUTURE.md: pendientes de inicio

---

## 2026-07-09 — Módulo Tickets — PERMISSIONS.md creado (Documento #4 del ciclo Document-First)

### Sin commit — documentación pura (no requiere commit de código)

**Archivos creados/modificados:** `docs/modules/tickets/PERMISSIONS.md` (nuevo), `docs/modules/tickets/README.md` (actualizado).

#### Contenido de PERMISSIONS.md
- 10 secciones completas: introducción, alcance, roles con capacidades, acciones por rol (§4.1–§4.33), permisos por estado (§5.1–§5.9), matriz consolidada (§6), restricciones absolutas, excepciones operacionales, extensiones futuras y registro de DUDAs.
- 4 roles del SDD documentados: SuperAdministrador, Administrador, Trabajador, Solicitante.
- 33 acciones documentadas individualmente con permiso explícito para cada rol.
- Matriz consolidada: 33 filas × 4 roles.
- 15 restricciones absolutas (REST-TK-001 a REST-TK-015): reglas de permiso que no admiten excepciones.
- 10 excepciones operacionales (EXC-TK-001 a EXC-TK-010): situaciones donde las reglas normales tienen matices.
- 8 extensiones futuras documentadas (EXT-TK-001 a EXT-TK-008).
- 9 DUDAs existentes referenciadas a lo largo del documento.
- 2 nuevas DUDAs registradas: DUDA-TK-016 (rol "Técnico" no definido en SDD) y DUDA-PERM-001 (cambio de sucursal post-creación no definido en SDD).

#### Actualización de README.md
- DUDA-TK-016 y DUDA-PERM-001 añadidas a la sección "Decisiones pendientes".

#### Estado del módulo Tickets — Fase 2
- BUSINESS-RULES.md: creado en sesión anterior (82 reglas, 2209 líneas), QA en ejecución — resultado pendiente.
- PERMISSIONS.md: creado 2026-07-09 — pendiente de QA.
- EVENTS.md, VALIDATIONS.md, FLOWS.md, FUTURE.md: por crear.

---

## 2026-07-08 — Módulo Tickets Fase 1 — Dominio del Ticket — APROBADO

- Aprobación QA de los 2 documentos de Fase 1 del módulo Tickets tras 3 ciclos de corrección
- `docs/modules/tickets/README.md` — 189 líneas, punto de entrada del módulo, alcance, 7 DUDAs en tabla de decisiones pendientes + 2 adicionales
- `docs/modules/tickets/DOMAIN.md` — 836 líneas, 12 secciones, 31 términos en glosario, 12 DUDAs registradas (DUDA-TK-001 a DUDA-TK-012)
- 12 DUDAs abiertas: las 5 críticas (DUDA-TK-001, TK-004, TK-006, TK-007, TK-009) bloquean Fase 2
- Siguiente: Fase 2 del módulo Tickets (BUSINESS-RULES.md, PERMISSIONS.md, EVENTS.md, VALIDATIONS.md, FLOWS.md, FUTURE.md) — pendiente resolución de DUDAs por el usuario

---

## 2026-07-08 — Módulo Catálogos del Sistema aprobado (8 documentos)

- Aprobación QA de los 8 documentos del módulo Catálogos del Sistema (`docs/modules/system-catalogs/`)
- 27 reglas de negocio (BR-SC-001 a BR-SC-027), 9 flujos (FL-SC-001 a FL-SC-009), 10 eventos de dominio, 10 funcionalidades futuras (FUT-SC-001 a FUT-SC-010)
- 6 DUDAs registradas (DUDA-SC-001 a DUDA-SC-006), todas abiertas y pendientes de decisión del usuario
- DUDA-SC-001 es la principal bloqueante: afecta el esquema de tablas, RLS y consultas (¿catálogos globales o por empresa?)
- DUDA-SC-005 tiene impacto transversal: si se aprueba, el módulo Tipos de Servicio queda absorbido por este módulo
- DATABASE.md y API.md no creados: bloqueados hasta resolución de DUDAs

---

## 2026-07-08 — Módulo Tipos de Servicio aprobado

- Aprobación QA de los 8 documentos del módulo Tipos de Servicio
- 18 reglas de negocio (BR-TS-001 a BR-TS-018), 7 flujos (FL-TS-001 a FL-TS-007), 6 eventos de dominio, 10 funcionalidades futuras (FUT-TS-001 a FUT-TS-010)
- 6 DUDAs registradas (DUDA-TS-001 a DUDA-TS-006), todas abiertas
- DUDA-TS-001 permanece abierta: bloquea DATABASE.md y API.md del módulo

---

## 2026-07-08 — Módulo Áreas aprobado (Iteración 3.21)

- Aprobación QA de los 8 documentos del módulo Áreas tras 4 ciclos de revisión
- Defectos corregidos en este ciclo: NEW-01 (DUDA-AR-001 → DUDA-AR-008 en VALIDATIONS.md), NEW-02 ("Sí" con tilde en tablas de PERMISSIONS.md y EVENTS.md), NEW-03 ("Responsable designado" en glosario de DOMAIN.md)
- 24 reglas de negocio (BR-AR-001 a BR-AR-024) y 13 DUDAs (DUDA-AR-001 a DUDA-AR-013) registradas
- DUDA-AR-001 permanece abierta: bloquea DATABASE.md y API.md

---

## [Sin versión] — Iteración 3.18 · Documentación de Negocio del Módulo Empresas (2026-07-07)

### Sin commit — documentación pura (no requiere commit de código)

**Archivos creados:** `docs/modules/companies/README.md`, `docs/modules/companies/DOMAIN.md`, `docs/modules/companies/BUSINESS-RULES.md`, `docs/modules/companies/PERMISSIONS.md`, `docs/modules/companies/VALIDATIONS.md`, `docs/modules/companies/EVENTS.md`, `docs/modules/companies/FLOWS.md`, `docs/modules/companies/FUTURE.md`. Total: 8 archivos markdown.

#### Iteración 3.18 — Documentación de negocio del módulo Empresas

- **docs/modules/companies/README.md:** Visión general del módulo — propósito, alcance, entidades principales (Empresa, ConfiguraciónEmpresa, IdentidadVisual), dependencias con sucursales, usuarios, tickets y auditoría. Punto de entrada a la documentación completa del módulo.
- **docs/modules/companies/DOMAIN.md:** Modelo de dominio completo. Entidad Empresa con sus atributos, entidad ConfiguraciónEmpresa (zona horaria, idioma, moneda, límites), entidad IdentidadVisual (logo, colores primario/secundario, favicon). Relaciones con otros módulos. Glosario de términos. Dudas pendientes: DUDA-CO-001 (mono-tenant vs multi-tenant — impacto arquitectónico crítico) y DUDA-CO-002.
- **docs/modules/companies/BUSINESS-RULES.md:** 15 reglas de negocio catalogadas BR-CO-001 a BR-CO-015. Referencia corregida en BR-CO-013: "BE-008" → "SEC-004" (corrección post-QA; C-003 resuelto).
- **docs/modules/companies/PERMISSIONS.md:** Matriz de permisos 4 roles × N operaciones. DUD-CO-001 documentada en sección de dudas pendientes (¿puede Administrador editar nombre, logo y colores de su empresa?).
- **docs/modules/companies/VALIDATIONS.md:** Validaciones funcionales en lenguaje de negocio sin código Zod. 3 secciones: creación, edición, configuración. Contradicción sobre obligatoriedad de País/Zona horaria (C-002) resuelta: alineado con DOMAIN.md (campos opcionales con fallback al sistema).
- **docs/modules/companies/EVENTS.md:** 6 eventos de dominio — company.created, company.updated, company.deactivated, company.reactivated, company.config_updated, company.deleted — con esquema de payload, módulos suscriptores y efectos concretos. Evento company.deleted añadido tras rechazo de QA (C-001 resuelto).
- **docs/modules/companies/FLOWS.md:** 6 flujos: FL-CO-001 (Crear Empresa), FL-CO-002 (Editar Empresa), FL-CO-003 (Desactivar Empresa), FL-CO-004 (Reactivar Empresa), FL-CO-005 (Actualizar Configuración), FL-CO-006 (Borrado Lógico). FL-CO-006 añadido tras rechazo de QA (C-001 resuelto). Dudas pendientes: DUD-CO-004 a DUD-CO-006.
- **docs/modules/companies/FUTURE.md:** 8 funcionalidades futuras planificadas: FUT-CO-001 a FUT-CO-008.
- **Ciclo QA:** Rechazo inicial con 3 críticos — C-001 (faltaba FL-CO-006 y evento company.deleted), C-002 (contradicción DOMAIN.md vs VALIDATIONS.md sobre País/Zona horaria), C-003 (referencia rota "BE-008" en BR-CO-013). Los 3 críticos fueron corregidos. Estado final: aprobado con observaciones menores pendientes.
- **Dudas abiertas:** DUDA-CO-001 (mono vs multi-tenant, bloqueante crítica para DATABASE.md), DUD-CO-001 (permisos de edición del Admin), DUD-CO-004 (comportamiento al desactivar con tickets activos), QA-DUD-001 (notificación al desactivar), QA-DUD-002 (evento para cambios de identidad visual).
- **Pendiente:** Usuario debe revisar y aprobar los 8 documentos; resolver DUDA-CO-001 antes de proceder a DATABASE.md y API.md de cualquier módulo.

---

## [Sin versión] — Iteración 3.17 · Documentación de Negocio del Módulo Usuarios (2026-07-07)

### Sin commit — documentación pura (no requiere commit de código)

**Archivos creados:** `docs/modules/users/README.md`, `docs/modules/users/DOMAIN.md`, `docs/modules/users/BUSINESS-RULES.md`, `docs/modules/users/PERMISSIONS.md`, `docs/modules/users/VALIDATIONS.md`, `docs/modules/users/EVENTS.md`, `docs/modules/users/FLOWS.md`, `docs/modules/users/FUTURE.md`. Total: 8 archivos markdown.

#### Iteración 3.17 — Documentación de negocio del módulo Usuarios

- **docs/modules/users/README.md:** Visión general del módulo — alcance, propósito, entidades principales (Usuario, Rol, EstadoLaboral), dependencias con otros módulos (tickets, notificaciones, auditoría) y punto de entrada a toda la documentación del módulo.
- **docs/modules/users/DOMAIN.md:** Modelo de dominio completo. Entidad Usuario con 15 atributos. Enumerados: Rol (SuperAdministrador, Administrador, Trabajador, Usuario) y EstadoLaboral (Activo, Inactivo, Suspendido). Relaciones con empresas, sucursales, tickets, notificaciones y auditoría. Glosario de términos del dominio.
- **docs/modules/users/BUSINESS-RULES.md:** 11 reglas de negocio catalogadas (BR-US-001 a BR-US-011). BR-US-001 (unicidad de correo), BR-US-002 (jerarquía de roles), BR-US-003 (restricciones de auto-modificación), BR-US-004 (borrado lógico), BR-US-005 a BR-US-010 (reglas de operación). BR-US-011 añadida por QA: usuario Inactivo o Suspendido no puede recibir tickets nuevos.
- **docs/modules/users/PERMISSIONS.md:** Matriz de permisos 4 roles × N operaciones. Fila de borrado lógico añadida. Sección "Dudas pendientes" con CRIT-02 (¿puede Administrador restablecer contraseñas de otros usuarios?).
- **docs/modules/users/VALIDATIONS.md:** Reglas de validación en lenguaje de negocio, sin código Zod. FL-US-005 separada en FL-US-005A (contraseña al crear usuario) y FL-US-005B (cambio de contraseña por el propio usuario). Requisito de contraseña en minúsculas añadido per SDD §16.3.
- **docs/modules/users/EVENTS.md:** 5 eventos de dominio — UsuarioCreado, UsuarioModificado, UsuarioDesactivado, UsuarioBorradoLogico, SucursalUsuarioCambiada — con esquema de payload, módulos suscriptores y efectos concretos. OBS-04 corregida: unificado el receptor de la notificación por cambio de sucursal.
- **docs/modules/users/FLOWS.md:** 5 flujos diagramados: FL-US-001 (Crear Usuario), FL-US-002 (Editar Usuario), FL-US-003 (Desactivar Usuario), FL-US-004 (Borrado Lógico — operación distinta a desactivar, CRIT-01 resuelto), FL-US-005 (Restablecer Contraseña — CRIT-02 pendiente). Duda CRIT-02 documentada con ambas interpretaciones contradictorias.
- **docs/modules/users/FUTURE.md:** 6 funcionalidades futuras planificadas: FUT-US-001 (autenticación 2FA), FUT-US-002 (OAuth), FUT-US-003 (historial de accesos), FUT-US-004 (suspensión temporal con fecha de reactivación), FUT-US-005 (perfil de habilidades/certificaciones), FUT-US-006 (importación masiva por CSV).
- **Ciclo QA:** 4 problemas críticos identificados. CRIT-01 (flujo de desactivación confundido con borrado lógico), CRIT-03 (faltaba regla explícita de auto-modificación de rol), CRIT-04 (requisito de contraseña en minúsculas no documentado) — resueltos. CRIT-02 (¿Administrador puede restablecer contraseñas?) documentado como duda pendiente con las dos interpretaciones contradictorias. OBS-01 a OBS-08 gestionadas.
- **Pendiente:** Usuario debe revisar y aprobar los 8 documentos antes de proceder a DATABASE.md y API.md. Decisión sobre CRIT-02 requerida antes de documentar la capa de permisos en DATABASE.md.

---

## [Sin versión] — Iteración 3.16 · Nueva Estructura Documental Modular (2026-07-07)

### Sin commit — documentación pura (no requiere commit de código)

**Archivos creados:** `docs/README.md`, `docs/METHODOLOGY.md`, `docs/architecture/` (8 READMEs), `docs/modules/` (15 módulos × 11 archivos = 165 archivos), `docs/decisions/ADR-001` a `ADR-009`. Total: ~185 archivos markdown.

#### Iteración 3.16 — Cambio de metodología: Document-First obligatoria

- **docs/README.md:** Índice maestro de toda la documentación del proyecto. Estructura clara de secciones: arquitectura, módulos, decisiones. Punto de entrada único para cualquier consulta documental.
- **docs/METHODOLOGY.md:** Documento de metodología Document-First. Define el ciclo de trabajo obligatorio antes de implementar cualquier módulo: (1) Dominio, (2) Reglas, (3) Flujos, (4) Permisos, (5) Eventos, (6) Validaciones, (7) Aprobación del usuario, (8) DB, (9) API, (10) Backend. Ningún módulo puede avanzar a implementación sin completar y aprobar los pasos 1–7.
- **docs/architecture/ (8 READMEs):** Documentación arquitectónica por capa — `frontend/README.md` (estructura de features, patrones, convenciones), `backend/README.md` (Edge Functions, servicios, contratos), `database/README.md` (esquema general, convenciones, migraciones), `security/README.md` (autenticación, autorización, RLS), `notifications/README.md` (arquitectura multi-canal), `events/README.md` (EventBus, contratos, flujo pub/sub), `permissions/README.md` (matriz de permisos por rol), `integrations/README.md` (Supabase, Web Push, canales externos).
- **docs/modules/ (15 módulos × 11 archivos):** Cada módulo documentado con los 11 artefactos del ciclo Document-First. Módulos cubiertos: `tickets`, `users`, `notifications`, `dashboard`, `audit`, `reports`, `settings`, `companies`, `branches`, `areas`, `comments`, `attachments`, `assignments`, `service-types`, `permissions`. Los 11 archivos por módulo son: `01-domain.md`, `02-rules.md`, `03-flows.md`, `04-permissions.md`, `05-events.md`, `06-validations.md`, `07-approval.md`, `08-database.md`, `09-api.md`, `10-backend.md`, `11-tests.md`.
- **docs/decisions/ADR-001 a ADR-009:** Architecture Decision Records — documenta las 9 decisiones arquitectónicas principales del proyecto con su contexto, alternativas consideradas, decisión tomada y consecuencias.
- **Impacto en el proceso:** La metodología Document-First pasa a ser parte de la Definición de Terminado de cada módulo. Ningún subagente puede iniciar implementación sin que los documentos 1–7 estén completos y el usuario haya aprobado. Esta iteración establece la base documental para toda la Fase 4 en adelante.

---

## [Sin versión] — Iteración 3.15 · Arquitectura de Notificaciones Multi-Canal (2026-07-07)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos creados:** `src/features/notifications/core/` (8 archivos), `src/features/notifications/channels/` (10 archivos + index), `src/features/notifications/store/preferences.store.ts`, `src/features/notifications/store/notifications.store.ts`, `src/features/notifications/hooks/` (3 archivos), `src/features/notifications/components/` (2 archivos), `src/features/notifications/index.ts`, `src/features/notifications/service-worker/README.md`, `.claude/notification-architecture.md`

**Archivos modificados:** `src/features/settings/pages/SettingsPage.tsx`

#### Iteración 3.15 — Arquitectura escalable de notificaciones desacoplada por eventos

- **Capa core (7 clases + barrel):** `NotificationEventBus` implementa el patrón Publish/Subscribe — desacopla emisores de eventos de los consumidores sin importaciones directas entre módulos. `NotificationService` coordina el ciclo de vida completo: recibe eventos del bus, resuelve canal(es) destino, genera el payload final via `NotificationFactory` y persiste el resultado. `NotificationDispatcher` toma la decisión de enrutamiento multi-canal consultando las preferencias del usuario en `preferences.store.ts` y el estado de disponibilidad de cada canal en `NotificationRegistry`. `NotificationFactory` construye objetos `Notification` con tipado estricto TypeScript desde los payloads crudos de los eventos del bus. `NotificationRegistry` mantiene el mapa de tipos de notificación soportados, metadatos de cada canal (nombre, ícono, requiere permiso) y prioridades de entrega por defecto. `NotificationQueue` mantiene la cola de notificaciones pendientes de entrega con soporte de reintentos configurables y control de concurrencia máxima por canal. `NotificationTemplates` provee las plantillas base reutilizables para generar el cuerpo de notificaciones en texto plano, HTML y markdown.
- **Canales (10 implementaciones):** `InternalChannel` — funcional completo: escribe directamente en `notifications.store.ts`, genera contadores de no leídos en tiempo real. `PushChannel` — implementado y listo para activar: detecta soporte del navegador, solicita permiso Web Push via `Notification.requestPermission()`, crea `ServiceWorkerRegistration` cuando el permiso es concedido. Los 8 canales restantes (`EmailChannel`, `SMSChannel`, `WhatsAppChannel`, `TeamsChannel`, `SlackChannel`, `DiscordChannel`, `TelegramChannel`, `WebhookChannel`) son stubs que implementan la interfaz `INotificationChannel` con método `send()` que retorna `Promise<void>` — están diseñados para recibir la implementación real de llamadas a Edge Functions en Fase 4 sin modificar la firma del contrato.
- **Stores Zustand:** `preferences.store.ts` persiste en `localStorage` las preferencias del usuario — objeto `channels` (booleano por canal), `quietHours` (inicio/fin de horario de no molestar), `digestFrequency` (inmediata / cada hora / diaria). `notifications.store.ts` mantiene el estado reactivo en memoria — array `notifications`, contadores `unreadCount` / `totalCount`, acciones `markAsRead` / `markAllAsRead` / `removeNotification` / `addNotification`.
- **Hooks React:** `useNotificationService` — inicializa el `NotificationService` una sola vez con `useRef` y limpia todas las suscripciones del `EventBus` al desmontar el componente host. `useNotificationPreferences` — envuelve `preferences.store.ts` exponiendo `preferences` y `updatePreference(channel, value)` con un setter tipado. `useNotificationPermission` — estado derivado del permiso push actual (`Notification.permission`), función `requestPermission()` que actualiza el estado al confirmar o denegar, compatible con SSR (guard `typeof window !== 'undefined'`).
- **Componentes:** `PushPermissionBanner` — banner contextual que se monta solo cuando `permission === 'default'`; botones "Activar notificaciones" (llama `requestPermission`) y "Ahora no" (marca `dismissed` en sessionStorage). Se oculta automáticamente al conceder o denegar. `NotificationPreferences` — formulario de configuración con un `Switch` por canal (10 canales), sección de horario de no molestar con inputs de hora, y selector de frecuencia de resumen. Consume `useNotificationPreferences` y persiste cada cambio inmediatamente.
- **SettingsPage.tsx:** Nueva sección "Preferencias de Notificaciones" añadida al final del formulario de configuración, justo antes de la zona de peligro. Importa y renderiza el componente `NotificationPreferences` sin cambios en el resto de secciones existentes.
- **Verificación:** TypeScript 0 errores. Arquitectura desacoplada mediante EventBus — ningún módulo del sistema necesita importar directamente los canales de notificación. Extensible: añadir un canal nuevo requiere solo implementar `INotificationChannel` y registrarlo en `NotificationRegistry`. Lista para integración real con Supabase Realtime y Web Push API en Fase 4.

---

## [Sin versión] — Iteración 3.14 · UserEditPage ancho completo, Configuración restringida, modal Cambiar Estado, gestión empresas/sucursales, filtros+gráficos en Reportes, restructuración ProfilePage (2026-07-07)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos modificados:** `src/features/users/pages/UserEditPage.tsx`, `src/app/components/AppHeader.tsx`, `src/features/tickets/pages/TicketDetailPage.tsx`, `src/features/users/pages/UsersPage.tsx`, `src/features/reports/pages/ReportsPage.tsx`, `src/features/profile/pages/ProfilePage.tsx`

#### Iteración 3.14 — Refinamiento de formularios, control de acceso, UX de tickets, gestión de organizaciones y ampliación de reportes

- **UserEditPage.tsx — layout a ancho completo:** Clase `mx-auto max-w-xl` removida del Card principal; el formulario ahora ocupa todo el ancho disponible del contenedor. Correo + Teléfono reorganizados en grid `sm:grid-cols-2`. Empresa + Sucursal reorganizadas en grid `sm:grid-cols-2`. Props `error={!!errors.name}`, `error={!!errors.apellido}` y `error={!!errors.correo}` añadidos a los respectivos Inputs; activan el borde rojo del design system al fallar la validación.
- **AppHeader.tsx — restricción de Configuración por rol:** Variable local `isAdmin` calculada como `user?.rol === 'admin' || user?.rol === 'superadmin'`. El item "Configuración" del dropdown de usuario envuelto en `{isAdmin && (...)}`. La ruta `/configuracion` queda accesible por URL directa pero ya no aparece en la navegación para trabajadores y usuarios básicos. Alineado con AppSidebar.tsx donde el item ya existía solo en `adminNav`.
- **TicketDetailPage.tsx — modal "Cambiar estado" rediseñado:** DialogContent ampliado a `max-w-sm` con clase `ps-glow-modal`. La descripción del modal muestra el estado actual mediante `<StatusBadge />` en lugar de texto plano. Cada opción de estado aplica borde azul + fondo `bg-primary/10` cuando está seleccionada, e incluye un ícono `<Check />` a la derecha. El botón "Confirmar cambio" queda deshabilitado mientras el estado seleccionado (`pendingStatus`) sea igual al estado actual (`localStatus`). Import `Check` añadido desde lucide-react.
- **TicketDetailPage.tsx — confirmación al cancelar ticket:** Nuevo estado `cancelDialog: boolean` inicializado en `false`. El botón "Cancelar ticket" dispara `setCancelDialog(true)` en lugar de navegar directamente. Un `ConfirmDialog` de variante destructiva gestiona la confirmación; la navegación de cancelación solo ocurre cuando el usuario acepta el diálogo.
- **UsersPage.tsx — gestión interactiva de empresas y sucursales:** Sección "Empresas": encabezado con botón "Nueva empresa", filas con ícono `Building2`, badge de estado activa/inactiva, y `DropdownMenu` por fila con acciones Editar y toggle de activación. Sección "Sucursales": selector de empresa para filtrar la lista, botón "Nueva sucursal", `DropdownMenu` por fila con estado activa/inactiva. Estados locales añadidos: `localSucursales`, `localAreas` (listas mutables), `selectedEmpresaFilter` (filtro de empresa activo), `toggleEmpresaTarget` y `toggleAreaTarget` (IDs para los diálogos de confirmación). Handlers `handleToggleEmpresa()` y `handleToggleArea()` actualizan el estado local con la activación/desactivación correspondiente. Dos nuevos `ConfirmDialog` para confirmar la activación o desactivación. Imports añadidos: `Pencil`, `Power`, `Building2`, `MapPin` desde lucide-react.
- **ReportsPage.tsx — filtros y 2 nuevos gráficos:** Sección de filtros añadida sobre los gráficos: inputs Desde y Hasta (type date), Select de Empresa, Select de Estado, botón "Aplicar filtros" (primario) y botón "Limpiar" (outline). Botón "Ver todo" o "Ver toda" (variante ghost + ícono `ArrowRight`) añadido al header de cada gráfico existente para navegar al módulo correspondiente. Nuevo gráfico "Tendencia semanal": `AreaChart` de Recharts con gradiente azul (`linearGradient`), usando datos `byWeek` del mock. Nuevo gráfico "Por estado": `PieChart` donut (innerRadius 50) con colores semánticos por estado (asignados via `COLORS` map), usando datos `byStatus` del mock. Nuevos imports: `AreaChart`, `Area` (Recharts), `Filter`, `X`, `ArrowRight` (lucide-react), `Input` (shared/ui), componentes Select.
- **ProfilePage.tsx — restructuración de layout:** Enlace de texto "Cambiar foto" debajo del avatar eliminado (la foto ya es clickeable vía FileReader). Campo "Usuario" añadido en la columna izquierda después de empresa y sucursal, renderizado condicionalmente con `{user.usuario && (...)}`. Card "Apariencia" (tema, idioma, preferencias visuales) movida desde la columna derecha a la columna izquierda, debajo del card de perfil personal, con separación `mt-4`. Sección de solo lectura (campos Empresa, Sucursal, Usuario) eliminada del card "Información personal", junto con el `<Separator />` que la antecedía. Correo y Teléfono agrupados en grid `sm:grid-cols-2` para mejor uso del ancho disponible.
- **Verificación:** TypeScript 0 errores. Preview confirmado en todas las páginas modificadas.

---

## [Sin versión] — Iteración 3.13 · Favicon logo, trazabilidad de asignación y columna "F. creación" (2026-07-06)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos modificados:** `index.html`, `src/features/tickets/pages/MyTicketsPage.tsx`

#### Iteración 3.13 — Favicon logo.jpeg, "Asignado a / Por:" en tickets y renombre de columna de fecha

- **Favicon → logo.jpeg:** `index.html` — el atributo `href` del elemento `<link rel="icon">` se actualizó de `favicon.svg` a `/logo.jpeg`. La pestaña del navegador ahora muestra el logo real del producto en lugar del SVG genérico.
- **Helper `getAssignedBy(ticket)`:** `MyTicketsPage.tsx` — función auxiliar añadida que recorre `ticket.history` buscando la primera entrada con `action === 'Ticket asignado'` y devuelve el campo `author` de esa entrada. Retorna `null` si no existe historial de asignación. La función es puramente derivada de datos; no genera efectos secundarios.
- **Tarjeta mobile — doble línea en campo de asignación:** En la vista de lista mobile (tarjeta por ticket), el campo que antes solo mostraba el nombre del responsable ahora muestra dos líneas: `"Asignado a: [nombre]"` en la primera y `"Por: [nombre]"` en la segunda. La segunda línea solo se renderiza cuando `getAssignedBy` devuelve un valor no nulo.
- **Tabla desktop — columna "F. creación" y sub-línea "Por:":** En la tabla de tickets de desktop, la cabecera de columna `"Fecha"` fue renombrada a `"F. creación"` para precisar que el valor corresponde a la fecha de creación del ticket, no a ninguna otra fecha del ciclo de vida. La celda de la columna "Asignado a" en desktop también muestra la sub-línea `"Por: [nombre]"` con la misma lógica condicional.
- **Verificación:** TypeScript 0 errores. Preview confirmado en mobile 375px (tarjeta con "Por:") y desktop 1280px (columna "F. creación" + sub-línea "Por:" en celda).

---

## [Sin versión] — Iteración 3.12 · Branding, badge de notificaciones, bordes de cards y menú de acciones (2026-07-06)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos modificados:** `index.html`, `AppSidebar.tsx`, `AuthLayout.tsx`, `AppLayout.tsx`, `MobileNav.tsx`, `AppHeader.tsx`, `src/shared/ui/card.tsx`, `src/styles/globals.css`, `src/features/tickets/pages/MyTicketsPage.tsx`

#### Iteración 3.12 — Branding, badge de notificaciones, bordes de cards y menú de acciones de tickets

- **Nombre de marca "Pidde Servicio":** Corregido en todos los puntos de branding. `index.html`: etiqueta `<title>` y `apple-mobile-web-app-title`. `AppSidebar.tsx`: atributo `alt` del logo y `<span>` de nombre visible. `AuthLayout.tsx`: texto del logo en panel de marca desktop y en logo mobile. `AppLayout.tsx`: título fallback de la barra de aplicación.
- **Badge de notificaciones — rojo, centrado y cap 99+:** Actualizado en `AppSidebar.tsx`, `MobileNav.tsx` y `AppHeader.tsx`. Color cambiado de `bg-primary` (azul) a `bg-destructive` (rojo). Número centrado con `flex items-center justify-center`. Cap actualizado: `9+` → `99+` (se muestra cuando count supera 99).
- **Bordes de cards azules:** `src/shared/ui/card.tsx` actualizado — la clase de borde por defecto pasó de `border` a `border border-primary/25`. Todas las cards del sistema muestran ahora un borde azul semitransparente de forma consistente.
- **Glow de cards intensificado:** `src/styles/globals.css` — clase `ps-glow-card` con opacidades subidas de `0.25/0.15` a `0.45/0.3`. El efecto fosforescente/azul es más pronunciado y visible en el diseño oscuro.
- **Filtros móviles en MyTicketsPage — layout 3 filas:** Campo de búsqueda en fila 1 (ancho completo). Grid 2 columnas en fila 2: Estado+Prioridad a la izquierda, Desde+Hasta a la derecha (inputs de fecha con `w-full` para no cortar el ícono de calendario). Botón Limpiar en fila 3 (ancho completo). Desktop: todos los filtros en una sola fila sin cambios. Técnica: `grid grid-cols-2 gap-2 lg:contents` dentro de un `flex flex-col gap-2 lg:flex-row`.
- **Menú de acciones en tickets — reestructurado por rol:** Removidos "Cambiar estado", "Cambiar prioridad" y "Ver historial" del menú contextual (estos controles pertenecen al detalle del ticket). "Modificar ticket" y "Asignar responsable" ahora visibles únicamente para admin y superadmin. "Ver detalle" conserva el ícono `Eye`. Añadido "Ver historial de cambios" (disponible para todos los roles) con ícono `History` que abre un Dialog mostrando el historial mock de modificaciones del ticket. Estado `historyTicketId` añadido para gestionar la apertura del Dialog. Nuevos imports `Eye` y `History` desde lucide-react.
- **TypeScript:** 0 errores. Preview verificado: badge rojo centrado, filtros 2 columnas en mobile, `lg:contents` en desktop, menú diferenciado por rol.

---

## [Sin versión] — Iteración 3.10/3.11 · Modificar Ticket como Sheet lateral completo (2026-07-06)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos modificados:** `src/features/tickets/pages/MyTicketsPage.tsx`

#### Iteración 3.10/3.11 — Modificar Ticket mejorado como Sheet lateral

- **MyTicketsPage — EditTicketSheet:** El componente `EditTicketDialog` (Dialog limitado a 4 campos) fue reemplazado por `EditTicketSheet`, un Sheet lateral que expone todos los campos editables del ticket. El formulario incluye: tipo de servicio, título, empresa, sucursal, prioridad, descripción, estado y asignado a.
- **Botones visuales de prioridad:** Los cuatro niveles (Baja, Media, Alta, Crítica) se presentan como botones con colores semánticos. El nivel seleccionado muestra estado visual destacado; los no seleccionados quedan en variante `outline`.
- **Secciones diferenciadas:** La sección primaria usa fondo `bg-primary/5` con label "INFORMACIÓN PRINCIPAL". La sección secundaria usa label "CAMPOS ADICIONALES" sin fondo especial.
- **Dependencia Empresa→Sucursal:** El selector de Sucursal queda deshabilitado hasta que se selecciona una Empresa. Al cambiar de Empresa, la Sucursal se resetea y la lista se filtra por empresa seleccionada.
- **Validaciones en español:** Tipo de servicio, título, empresa y sucursal son campos obligatorios con mensajes de error en español. Se eliminan los últimos mensajes de validación en inglés del módulo de tickets.
- **Footer fijo:** El Sheet mantiene un footer pegado al fondo con los botones Cancelar y Guardar cambios, visibles sin necesidad de hacer scroll dentro del sheet.
- **Imports:** Añadidos `Sheet`, `SheetContent`, `SheetHeader`, `SheetTitle` (desde `@shared/ui/sheet`), `cn` (desde `@lib/utils`), `MOCK_SUCURSALES`, `MOCK_AREAS`, `TICKET_TYPES` (desde `@mocks/data`). Removido `DialogDescription` (ya no necesario).
- **TypeScript:** 0 errores tras el cambio. Prueba visual confirmada: sheet abre correctamente, pre-pobla todos los campos con datos del ticket seleccionado, selección de prioridad actualiza el estado de manera reactiva.

---

## [Sin versión] — Iteración 3.10 · Correcciones transversales de UX y consistencia (2026-07-06)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos modificados:** SettingsPage, ProfilePage, DashboardPage, MyTicketsPage (DatePicker, Limpiar, ModificarTicket), AuditoriaPage, formularios (validaciones), CreateTicketPage/UserNewPage (dependencia Empresa→Sucursal), EvidenciasSection, AsignarTrabajadorModal, componentes de scroll.

#### Iteración 3.10 — Correcciones transversales de UX y consistencia

- **Settings/Profile — vista unica sin navegacion interna:** Las paginas de Configuracion y Perfil eliminan la navegacion interna por tabs o acordeones. El contenido se despliega en una sola vista continua y desplazable, reduciendo la complejidad de interaccion.
- **Dashboard — dropdowns Empresa/Sucursal alineados con botones:** Los selectores de filtro del dashboard se alinean visualmente con los botones de accion. Espaciado y altura unificados con el resto de controles de la barra de filtros.
- **Tickets — DatePicker consistente:** El selector de fecha en el modulo de tickets usa el componente DatePicker del design system, reemplazando inputs nativos inconsistentes. Formato y comportamiento unificados con el resto del sistema.
- **Tickets — boton Limpiar con componente Button:** El boton para limpiar filtros reemplaza su implementacion ad-hoc por el componente `Button` del design system con variante y tamanio correctos.
- **Tickets — dialog Modificar Ticket:** Se implementa el dialogo de modificacion de ticket como modal del design system, con formulario de edicion completo y acciones Guardar/Cancelar estandarizadas.
- **Auditoria — overflow dropdown corregido:** El dropdown de acciones en la tabla de auditoria ya no queda cortado por el overflow del contenedor. Se aplica `overflow-visible` o portal correcto para que el menu flote sobre el contenido.
- **Formularios — validaciones en espanol:** Todos los mensajes de validacion de React Hook Form + Zod se muestran en espanol. Se eliminan mensajes en ingles o generico en los modulos revisados.
- **Formularios — dependencia Empresa→Sucursal:** El selector de Sucursal se resetea y filtra automaticamente al cambiar la Empresa seleccionada. Aplica en creacion de usuarios, creacion de tickets y cualquier formulario con ambos campos.
- **Evidencias — galeria con preview, nombre, tamano y eliminar:** La seccion de evidencias muestra miniatura de imagen (o icono para otros tipos), nombre del archivo, tamano legible y boton de eliminar por cada elemento. Mejora la usabilidad al adjuntar y revisar archivos en un ticket.
- **Asignar Trabajador — modal con Design System:** El modal de asignacion de trabajador se rehace usando los componentes Dialog, Button, Select y FormField del design system. Consistencia visual con el resto de modales del sistema.
- **Scroll — causa raiz investigada y corregida:** Se identifica y corrige la causa raiz del scroll horizontal no deseado y del doble scroll vertical en vistas principales. Se aplica `overflow-x: hidden` donde corresponde y se elimina altura fija incorrecta en contenedores interiores.

---

## [Sin versión] — Iteración 3.9 · Vistas dedicadas de usuarios, logo Pidde, breadcrumb y sorting (2026-07-06)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos modificados:** Sidebar, AuthLayout, PageBreadcrumb, UsersPage, UserNewPage (nuevo), UserDetailPage (nuevo), UserEditPage (nuevo), router, Tickets, Notificaciones, Dashboard, SettingsPage, ProfilePage.

#### Iteración 3.9 — Vistas dedicadas de usuarios, logo Pidde, breadcrumb y sorting cronológico

- **Logo y nombre de marca:** `/logo.jpeg` integrado en `AppSidebar` y `AuthLayout`. El nombre de la aplicación muestra "Pidde" en todos los contextos de marca.
- **Breadcrumb — separador actualizado:** El separador del componente `PageBreadcrumb` cambió de `/` a `›` para mayor claridad visual y alineación con patrones de UI modernos.
- **UsersPage — eliminación de modales:** Los modales de Crear, Ver y Editar usuario fueron eliminados. Las acciones correspondientes ahora navegan a páginas dedicadas via `react-router-dom`. La página queda limpia, con tabla y filtros solamente.
- **UserNewPage** (`src/features/users/pages/UserNewPage.tsx`): nueva página dedicada para crear usuario. Formulario completo con validación, campos de rol, empresa, sucursal y estado. Ruta: `/usuarios/nuevo`.
- **UserDetailPage** (`src/features/users/pages/UserDetailPage.tsx`): nueva página dedicada para ver el perfil de un usuario. Muestra todos los datos en modo lectura con breadcrumb de retorno. Ruta: `/usuarios/:id`.
- **UserEditPage** (`src/features/users/pages/UserEditPage.tsx`): nueva página dedicada para editar un usuario. Formulario pre-poblado con los datos del usuario seleccionado. Ruta: `/usuarios/:id/editar`.
- **Router:** Tres nuevas rutas registradas bajo el guard `RequireAuth`: `/usuarios/nuevo`, `/usuarios/:id`, `/usuarios/:id/editar`. La ruta `/usuarios` mantiene el componente `UsersPage` existente.
- **Sort cronológico descendente:** Implementado en Tickets (`MyTicketsPage`), Notificaciones (`NotificationsPage`) y Dashboard (sección de tickets recientes). El criterio de ordenamiento por defecto es siempre el más reciente primero.
- **SettingsPage — layout PC con navegación lateral:** En viewports de escritorio, la configuración muestra un panel lateral con navegación por secciones (tabs verticales) y el contenido de la sección activa a la derecha. Mobile mantiene el layout de acordeón o secciones apiladas.
- **ProfilePage — layout PC de 2 columnas:** En escritorio, el perfil divide el contenido en columna izquierda (datos personales + foto) y columna derecha (seguridad + preferencias). Mobile mantiene el layout de una columna.

---

## [Sin versión] — Iteración 3.8 · Design System Refactor — Componentes base y contextuales (2026-07-06)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos modificados:** Input, Textarea, FormField (nuevo), SearchInput (nuevo), Pagination (nuevo), ChartContainer (nuevo), globals.css, MyTicketsPage, NotificationsPage, LoginPage, CreateTicketPage, ProfilePage, SettingsPage, UsersPage.

#### Iteración 3.8 — Design System Refactor: componentes base y contextuales

- **Input**: nuevo prop `error` — cuando se pasa un string, el campo muestra borde rojo + glow rojo sutil. Integrado con el patrón de validación del sistema.
- **Textarea**: mismo patrón que Input — prop `error` con borde rojo + glow rojo sutil. Consistencia visual con el campo Input en formularios con validación.
- **FormField** (`src/shared/components/FormField.tsx`): nuevo componente base para todos los formularios del sistema. Encapsula el patrón Label + campo + mensaje de error + texto de ayuda (hint). Elimina la duplicación de este patrón en cada página.
- **SearchInput** (`src/shared/components/SearchInput.tsx`): nuevo componente construido sobre Input con icono `Search` integrado. Uso unificado en todas las páginas que tienen campo de búsqueda.
- **Pagination** (`src/shared/components/Pagination.tsx`): nuevo componente base reutilizable para paginación. Reemplaza las implementaciones ad-hoc en páginas individuales.
- **ChartContainer** (`src/shared/components/ChartContainer.tsx`): nuevo componente extraído de DashboardPage. Provee el wrapper visual unificado (Card + header + acciones) para todos los gráficos del sistema.
- **globals.css** — `ps-glow-card` y `ps-glow-form` corregidos: el glow actúa como borde luminoso (box-shadow exterior), no como cambio de fondo. Comportamiento visual correcto para el design system oscuro.
- **globals.css** — nueva clase `ps-glow-modal`: glow específico para modales y drawers. Diferencia visualmente los modales de las cards y formularios.
- **MyTicketsPage**: botón "Asignar" inline reemplazado por `DropdownMenu` contextual. Patrón consistente con el resto de acciones de tabla en el sistema.
- **NotificationsPage**: botones "X" y "Marcar todo" directos reemplazados por `DropdownMenu` contextual. Acción `onMarkUnread` agregada (marcar como no leída). Interfaz más limpia y consistente con otros módulos.
- **LoginPage, CreateTicketPage, ProfilePage, SettingsPage, UsersPage**: todos los formularios migrados a `FormField`. Elimina código duplicado de Label+error+hint en cada página.

---

## [Sin versión] — Iteración 3.7 · Gap Audit — Refinamiento visual y consistencia transversal (2026-07-06)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos modificados:** DashboardPage, globals.css, MobileNav, LoginPage, SettingsPage, UsersPage, ProfilePage + forms de todos los módulos.

#### Iteración 3.7 — Gap Audit: refinamiento visual y consistencia transversal

- **DashboardPage**: `ps-glow-card` aplicado en **todas** las chart cards, no solo en KPI cards. Consistencia visual completa entre secciones de métricas y gráficos.
- **DashboardPage**: Botón "Ver todos" agregado en **todas** las secciones de gráficos (no solo en tickets recientes). Cada sección de gráfico ahora tiene acción de navegación directa.
- **globals.css**: `ps-glow-card` mejorado — borde sólido 1px añadido + difuminado externo (box-shadow) más pronunciado. El glow ahora es visible tanto como borde como como brillo ambiental.
- **MobileNav**: Migrado de layout `flex-1` a `grid-cols-5` para centrado matemáticamente perfecto de los 5 tabs en la barra de navegación inferior. Elimina desalineaciones visuales en distintos anchos de pantalla.
- **Labels obligatorio/opcional completados**: Revisión exhaustiva — todos los formularios del sistema marcan `*` en campos requeridos y `(Opcional)` en opcionales. Aplicado o verificado en: LoginPage, SettingsPage, UsersPage (modales Crear y Editar), y todos los formularios restantes.
- **ProfilePage**: Badges de rol con mayor contraste visual — colores semánticos por rol (`SuperAdministrador` → destructivo/rojo, `Administrador` → primario/azul, `Trabajador` → warning/amarillo, `Usuario` → secundario). Mejora la legibilidad y la diferenciación de rol a primera vista.
- **Theme validation**: Verificado que el sistema dark-only es consistente en todos los componentes. No hay valores hardcodeados que quiebren el tema oscuro. CSS variables utilizadas correctamente en todo el árbol de componentes.

---

## [Sin versión] — Iteraciones 3.5 + 3.6 · UX Premium, nomenclatura global y componentes transversales (2026-07-06)

### Pendiente de commit — requiere autorización explícita del usuario

**Archivos modificados:** AppLayout, MobileNav, globals.css, LoginPage, DashboardPage, MyTicketsPage, TicketDetailPage, CreateTicketPage, NotificationsPage, ProfilePage, SettingsPage, UsersPage, mocks/data.ts, constants/index.ts + 3 archivos nuevos.

#### Iteración 3.5 — UX Premium y mejoras de interacción

- **AppLayout.tsx**: ScrollToTop al cambiar de ruta. Evita que las páginas nuevas se abran con el scroll en la posición de la página anterior.
- **MobileNav**: Fix de centrado de íconos y etiquetas en la barra inferior.
- **globals.css**: Animaciones CSS `@keyframes glow` con clases utilitarias para bordes y sombras animadas.
- **LoginPage**: Fondo animado con gradiente/partículas. Glow border en el card principal. Selector de empresa y sucursal previo al login. Modal "Olvidé mi contraseña" con formulario de recuperación.
- **DashboardPage**: Glow cards en KPIs. Botones "Ver todos" en secciones de tickets recientes. Sparklines en desktop (mini-gráficos inline en stat cards). Filtros empresa/sucursal para administradores. Opción "General" (todas las sucursales) en el selector de scope.
- **MyTicketsPage**: Ordenamiento por fecha (más reciente primero por defecto). Filtros de rango de fecha. UI de asignación mejorada (avatar + nombre). Tabla de datos en desktop en lugar de cards.
- **TicketDetailPage**: History tracking automático — cada cambio de estado, prioridad o responsable genera una entrada en la pestaña de historial sin intervención manual del usuario. Layout de ancho completo en PC.
- **CreateTicketPage**: Opción "Otros" condicional en tipo de servicio (muestra campo de texto libre). Evidencias con preview de imagen via FileReader y barra de progreso simulada. Layout 2 columnas en PC.
- **NotificationsPage**: Patrón Undo para eliminar notificaciones (toast con botón "Deshacer" durante 5 segundos). Estilos tipo Gmail para no leídas (fondo diferenciado, punto indicador azul). Layout 2 columnas en PC.
- **ProfilePage**: Foto de perfil clickeable con preview inmediato via FileReader. Campos de contraseña con toggle ojo. Secciones renombradas. Agrupación en grupos lógicos en PC.
- **SettingsPage**: Tooltips de ayuda en opciones avanzadas (icono `?` con Popover de shadcn/ui). Layout 2 columnas en PC.
- **UsersPage**: Campo Estado visible en tabla y en modal Editar. Nomenclatura de columnas y etiquetas actualizada.

#### Iteración 3.6 — Nomenclatura global y componentes transversales

- **Nomenclatura unificada en todo el sistema:**
  - `Sucursales` → `Empresa` (nivel superior de organización)
  - `Área` → `Sucursal` (unidad dentro de la empresa)
  - `Tipo de Solicitud` → `Tipo de Servicio`
  - Aplicado en: mocks/data.ts, constants/index.ts, todos los formularios, tablas, filtros, placeholders y textos de UI de todas las páginas.
- **PageBreadcrumb** (`src/shared/components/PageBreadcrumb.tsx`): Nuevo componente de breadcrumb integrado en todas las páginas interiores. Muestra la ruta jerárquica de navegación con separadores y enlace de regreso.
- **CommandMenu** (`src/shared/components/CommandMenu.tsx`): Nuevo componente de búsqueda/navegación global. Se activa con `Ctrl+K` (o `Cmd+K`). Permite navegar entre páginas, buscar tickets y ejecutar acciones frecuentes desde cualquier pantalla.
- **PageSkeletons** (`src/shared/components/PageSkeletons.tsx`): Nuevo archivo con skeletons de carga específicos por página (Dashboard, MyTickets, TicketDetail, CreateTicket, Users, Notifications, Profile, Settings, Reports, Audit). Reemplazan el spinner genérico durante la carga inicial.
- **Labels obligatorio/opcional**: Todos los formularios marcan con `*` los campos requeridos y con `(Opcional)` los opcionales. Aplicado en CreateTicketPage, ProfilePage, SettingsPage, modal Crear Usuario y modal Editar Usuario.

## [Sin versión] — Fase 3.4 · Flujo interactivo completo (modales y acciones) (2026-06-29)

### commit dc4c1b8 — Modales, acciones y estado mutable en todas las páginas

**9 archivos modificados** (1658 inserciones).

- **AppLayout.tsx**: `pt-4 lg:pt-5` restaurado en `<main>`; `pb-16` → `pb-20` en móvil. El layout controla el gap del header; las páginas individuales no lo sobrescriben.
- **providers/index.tsx**: `defaultTheme="system"` → `"dark"`. La clase `dark` queda permanente en `<html>`, activando todos los `dark:` variants de Tailwind en todo momento.
- **UsersPage.tsx** (+768 líneas): reescritura mayor. `MOCK_USERS` convertido a estado local mutable. `UserRow` recibe cinco callbacks. Modales completos: Crear (validación), Editar (pre-poblado), Ver Perfil (lectura). `ConfirmDialog` para Eliminar, Cambiar Estado y Restablecer Contraseña. Filtros y counts reactivos sobre el estado local.
- **TicketDetailPage.tsx** (+710 líneas): refactor mayor. Cinco variables de estado local mutable (`localStatus`, `localPriority`, `localAssignedTo`, `localComments`, `localEvidencias`). Modal Asignar Responsable con lista filtrada de trabajadores activos. Modal Cambiar Estado acotado a admins. Botones Reabrir/Cancelar/Confirmar cierre con handlers y `toast`.
- **ProfilePage.tsx**: guardado real con `toast.success`. Validación de contraseña (requeridos, coincidencia, mínimo 8 chars). Cambio de foto simulado. Switches de preferencias con estado local.
- **SettingsPage.tsx** (+309 líneas): reescritura. Todos los `Switch` con `useState` reactivo y `onCheckedChange` con toast. `Select`s con estado local. Botones Guardar por sección. `ConfirmDialog` para restablecer.
- **NotificationsPage.tsx**: notificaciones en estado mutable. Marcar individual/todas como leídas. Eliminar notificación. Contadores y tabs reactivos sobre estado local.
- **ReportsPage.tsx**: estado `downloading`. `toast.promise` con 1.5s de delay simulado. Botones deshabilitados durante la descarga.
- **AuditPage.tsx**: `toast.promise` con 2s de delay. Mensaje de éxito "Auditoria exportada como audit-log.xlsx".
- Validaciones: Arquitecto, QA, UX Lead, TypeScript (0 errores), Git Guardian, Release Reviewer — todos aprobados.

## [Sin versión] — Fase 3.3 · Consistencia visual, scroll y tipografía (2026-06-29)

### commit ed91387 — Scroll horizontal, escala tipográfica y padding compacto

**Múltiples archivos modificados** (22 subagentes, 641k tokens).

- **TicketDetailPage.tsx**: Eliminar scroll horizontal — causa raíz: tab bar `flex` sin `flex-1`. Fix: cada botón de tab tiene `flex-1 items-center justify-center` garantizando 1/3 del ancho disponible sin desbordamiento. Remover `pl-10` en acciones de comentario. `CardContent` → `p-3 pt-0`, `CardHeader` → `px-3 pt-3 pb-2`. Título de página `text-lg` → `text-base`. `CardTitle` → `text-[11px] uppercase tracking-widest`.
- **CreateTicketPage.tsx**: `CardContent` con `p-3 pt-0` en todos los bloques (default shadcn era `p-6`). Upload zone `py-6 px-6` → `py-4 px-4`. Título de página `text-lg` → `text-base`. `CardTitle` → `text-[11px] uppercase tracking-widest`.
- **globals.css**: Escala tipográfica canónica `.ps-page-title`, `.ps-card-label`, `.ps-body`, `.ps-meta`, `.ps-label`, `.ps-badge-text`, `.ps-table-header`, `.ps-table-cell`, `.ps-nav-item`, `.ps-mono`. Placeholder → `0.6875rem` (11px). `word-break: break-word` en body.
- **UsersPage, NotificationsPage, ProfilePage, ReportsPage, AuditPage, SettingsPage**: Normalización completa — `CardHeader`/`CardContent` con padding correcto, títulos de página `text-base`, `CardTitle` `text-[11px] uppercase`, labels `text-xs font-medium`, metadatos `text-[11px] text-muted-foreground`.
- **MyTicketsPage, DashboardPage**: Verificación y corrección de violaciones tipográficas menores.
- **LoginPage, AuthLayout**: Ajuste de subtítulos y descripciones a la escala canónica.
- TypeScript: 0 errores | ESLint: 0 warnings.

## [Sin versión] — Fase 3.2 · Design System Oscuro + Dashboard Graph-First (2026-06-29)

### commit 5ebd756 + d5f9385 — Design system v2 + Dashboard interactivo

**7 archivos modificados** (897 inserciones, 342 eliminaciones) + package.json/lock.

- **globals.css**: CSS variables reemplazadas — `:root` ahora usa dark como predeterminado.
  - `--background: 228 19% 5%` (#0B0C10), `--card: 240 6% 7%` (#111113), `--border: 240 6% 16%` (#26262B).
  - Variables semánticas de ticket/prioridad preservadas. Fuente Geist aplicada en `body`.
- **index.html**: Inter (Google Fonts CDN) eliminado; Geist vía `fonts.bunny.net` agregado. `theme-color` → `#0B0C10`.
- **tailwind.config.ts**: `fontFamily.sans` → `['Geist', '-apple-system', ...]`.
- **DashboardPage.tsx** (965 líneas, +750): Reescritura completa del dashboard Admin.
  - 9 widgets: DonutChart (status), BarChart vertical (prioridad), AreaChart (tendencia 16d), BarChart horizontal (sucursal), Stacked BarChart (áreas), PieChart (tipos), BarChart responsables, RadarChart (comparativa), ComposedChart (tendencia semanal).
  - Interactividad: `onClick` en cada chart → `navigate('/tickets?status=X')` etc.
  - Drag & Drop con `@dnd-kit`: `DndContext + SortableContext`, modo edición, `sessionStorage('ps-dashboard-order')`.
  - KPI cards: 4 métricas clave (Total abiertos, Críticos, Cerrados, Tasa resolución).
  - Tooltip oscuro (`#111113` bg, `#26262B` border) en todos los charts.
  - WorkerDashboard preservado con estilos del nuevo tema.
- **MyTicketsPage.tsx**: `useSearchParams` para inicializar filtros desde URL; banner "Filtrado desde el dashboard".
- **AppSidebar.tsx**: Ítem activo con `bg-primary/15 border-l-2 border-primary`; iconografía mejorada.
- **AppHeader.tsx**: Ajustes de consistencia visual con el nuevo tema.
- **Paquetes**: `geist ^1.7.2`, `@dnd-kit/core ^6.3.1`, `@dnd-kit/sortable ^10.0.0`, `@dnd-kit/utilities ^3.2.2`.
- **Proceso**: workflow multi-agente (11 subagentes, ~10 min, 389k tokens). TypeScript limpio, ESLint sin errores.

## [Sin versión] — Fase 3.1 · Refinamiento Visual y UX (2026-06-29)

### commit 49a91df — Refinamiento visual multi-agente
- **15 archivos modificados** (392 inserciones, 269 eliminaciones).
- **Tipografía**: escala reducida en toda la aplicación. Títulos de página `text-xl/2xl` → `text-lg font-semibold`. Valores de stat cards `text-3xl` → `text-xl/2xl font-semibold`. Labels `text-sm` → `text-xs font-medium`.
- **Espaciado Mobile First**: páginas `p-4 lg:p-6` → `p-3 lg:p-5`. Cards `p-4/5` → `p-3/4`. Gaps `gap-4` → `gap-3`. Section spacing `space-y-6` → `space-y-4`.
- **LoginPage**: card con `shadow-lg`, inputs `h-9`, labels `text-xs`, demo buttons fuera de la card con `h-9`. Aria-live para accesibilidad.
- **AuthLayout**: gradiente de fondo `from-background to-muted/30`. Bullets `✓` → íconos `CheckCircle2`. Tipografía del panel de marca reducida.
- **DashboardPage (Admin)**: `LineChart` → `AreaChart` con gradientes via `<defs>/<linearGradient>`. Status cards ahora tienen íconos (Inbox, AlertCircle, CheckCheck, RotateCcw, Clock, CheckCircle2). Valores `text-2xl font-bold` → `text-xl font-semibold tabular-nums`. Nueva sección `byPriority` con 4 mini-cards. Wrapper `p-3 lg:p-5`.
- **DashboardPage (Worker)**: heading reducido, tickets recientes más compactos `py-2.5`.
- **AppHeader**: `ChevronDown` eliminado del trigger del usuario. Solo avatar + nombre desktop.
- **AppSidebar**: nav items `py-2` → `py-1.5`, `px-3` → `px-2.5`. `ChevronRight` en items activos eliminado.
- **NotificationsPage**: items `p-4` → `py-2.5 px-3`, íconos `h-9 w-9` → `h-8 w-8`.
- **UsersPage, ProfilePage, ReportsPage, AuditPage, SettingsPage**: padding reducido, tipografía compactada, filtros `h-8 text-xs`.
- **Tickets (Create, Detail, MyTickets)**: form inputs `h-9`, filtros `h-8`, paddings reducidos.
- **Proceso**: workflow multi-agente con 14 subagentes (Auditoría → Especificaciones → Implementación ×4 paralelo → Validación → Commit). TypeScript limpio, ESLint sin errores.

## [Sin versión] — Fase 3 · Aplicación con Mock Data

### 2026-06-29
- `src/mocks/data.ts`: 15 tickets realistas, 7 usuarios, 4 sucursales, 9 áreas, tipos de ticket, 8 notificaciones, datos de tendencia (16 días) para gráficos.
- `src/constants/index.ts`: ruta `DESIGN_SYSTEM` añadida; helper `ticketDetailPath(id)`.
- `src/app/components/AppSidebar.tsx`: sidebar role-based (worker vs admin), navegación con badges de conteo, acceso rápido a nuevo ticket, footer con avatar del usuario.
- `src/app/components/AppHeader.tsx`: header con hamburger (mobile), título dinámico por ruta, campana de notificaciones con badge, dropdown de usuario.
- `src/app/components/MobileNav.tsx`: navegación inferior con 5 tabs y FAB flotante para nuevo ticket.
- `src/app/layouts/AuthLayout.tsx`: layout de autenticación (panel branded desktop + panel de formulario).
- `src/app/layouts/AppLayout.tsx`: layout principal (sidebar desktop + Sheet mobile + header + bottom nav).
- `src/app/router/index.tsx`: router completo con guards RequireAuth / RequireGuest; 12 rutas.
- `src/features/auth/pages/LoginPage.tsx`: login con validación Zod, toggle contraseña, 3 botones de acceso demo.
- `src/features/dashboard/pages/DashboardPage.tsx`: dashboard adaptativo — trabajador (mis stats) / admin (6 cards + LineChart Recharts + tickets recientes).
- `src/features/tickets/pages/MyTicketsPage.tsx`: lista con búsqueda, filtros estado/prioridad, paginación, card layout responsive.
- `src/features/tickets/pages/CreateTicketPage.tsx`: formulario con RHF + Zod, selector de área dependiente de sucursal, adjuntos mock, estado de éxito.
- `src/features/tickets/pages/TicketDetailPage.tsx`: descripción, tabs (comentarios/historial/evidencias) con badges de conteo, burbuja de comentarios, timeline de historial, acciones contextuales.
- `src/features/users/pages/UsersPage.tsx`: gestión de usuarios con stats, filtros, badges de rol coloreados, sección sucursales/áreas.
- `src/features/notifications/pages/NotificationsPage.tsx`: tabs todas/sin leer/leídas, timestamps relativos, íconos por tipo.
- `src/features/profile/pages/ProfilePage.tsx`: datos personales pre-cargados, cambio de contraseña, preferencias de notificación y apariencia.
- `src/features/reports/pages/ReportsPage.tsx`: BarChart por sucursal, PieChart por prioridad, BarChart horizontal por tipo, lista de reportes.
- `src/features/audit/pages/AuditPage.tsx`: log de eventos con íconos coloreados por tipo, búsqueda, summary de conteos.
- `src/features/settings/pages/SettingsPage.tsx`: configuración general, seguridad, tickets, notificaciones, zona de peligro.
- `vite.config.ts`: alias `@mocks` añadido; port usa `process.env.PORT` (para preview server).
- `tsconfig.json`: paths `@mocks/*` añadido.
- `.claude/launch.json`: configuración del preview server para desarrollo.
- Build prod: OK (9.74s). TypeScript: 0 errores. ESLint: 0 errores, 0 warnings.

## [Sin versión] — Fase 2 · Design System

### 2026-06-29
- `components.json`: corregido `baseColor: "blue"` → `"zinc"` (azul es color primario via CSS vars, no base).
- 32 componentes shadcn/ui instalados en `src/shared/ui/`: accordion, alert, alert-dialog, avatar, badge, button, calendar, card, checkbox, command, dialog, drawer, dropdown-menu, form, hover-card, input, label, navigation-menu, popover, progress, radio-group, scroll-area, select, separator, sheet, skeleton, slider, switch, table, tabs, textarea, tooltip.
- `globals.css`: tokens semánticos completos (success, warning, info + foregrounds; ticket status × 6 + foregrounds; priority × 4 + foregrounds; sidebar tokens; dark mode para todos).
- `tailwind.config.ts`: colores semánticos mapeados (success, warning, info, ticket.*, priority.*); escala tipográfica extendida (3xl, 4xl, 5xl, display); dimensiones sidebar (260px, 64px) y header (56px).
- Componentes personalizados: `Spinner.tsx`, `SpinnerOverlay`, `EmptyState.tsx`, `StatusBadge.tsx`, `PriorityBadge.tsx`, `ConfirmDialog.tsx`.
- Página Design System Showcase en `/ui` con 10 secciones: Colores, Tipografía, Botones, Controles, Tarjetas, Badges, Alertas, Diálogos, Loading, Íconos.
- `eslint.config.js`: override para `src/shared/ui/**` desactiva `react-refresh/only-export-components` (patrón shadcn no compatible con la regla).
- Router actualizado: ruta `/ui` → `DesignSystemPage`.
- Build prod: OK (4.86s, 157kB gzip). TypeScript: 0 errores. ESLint: 0 errores, 0 warnings. Tests: 5/5.

## [Sin versión] — Fase 1

### 2026-06-29
- Proyecto React 19 + Vite 5 + TypeScript strict inicializado.
- ESLint 9 flat config + Prettier + Tailwind + shadcn/ui (components.json).
- React Router DOM 6 · TanStack Query 5 · Zustand 5 · RHF 7 · Zod 3 · Supabase JS 2.
- ThemeProvider (dark/light/system) + `useTheme` hook separado.
- Stores Zustand: auth y ui. Alias de importación en vite + tsconfig.
- Estructura src/ completa: 9 features, 9 dirs shared, app, services, hooks, store, types, utils, constants, lib, styles.
- Vitest 2 (5 tests pasando) · Playwright 1 · Husky + lint-staged.
- Build producción: OK (1.93s, 97.78 kB gzip).

## [Sin versión] — Fase 0

### 2026-06-29
- Infraestructura de Fase 0: estructura `.claude/` completa (context, progress, tasks, validations, prompts).
- Memoria persistente: `CLAUDE.md`, `WORKFLOW.md`, `project-status.json`.
- 11 subagentes especializados documentados.
- Skill `control-de-commits` (Git Guardian + Release Reviewer + Recovery Manager).
- `.gitignore` poblado; `settings.json` con hook de protocolo de sesión.
