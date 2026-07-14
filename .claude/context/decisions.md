# Decisiones de Arquitectura (ADR ligero)

> Registro cronológico de decisiones importantes. Cada decisión: contexto, decisión, motivo, impacto. No se borra; se añade.

---

## ADR-001 — Coexistencia de la memoria `.claude/` con la configuración operativa de Claude Code
**Fecha:** 2026-06-29 · **Estado:** Aceptada

**Contexto:** El SDD (secc. 22) define `.claude/` como memoria persistente (`context/`, `progress/`, `tasks/`, `validations/`, `prompts/`). Claude Code también usa `.claude/` para `agents/`, `skills/`, `commands/`, `hooks/` y `settings.json`.

**Decisión:** Mantener ambas en `.claude/` de forma aditiva. La memoria del SDD se respeta tal cual; se añaden las carpetas operativas de Claude Code sin renombrar ni mover lo del SDD.

**Motivo:** No hay conflicto de nombres; permite que la memoria y la automatización vivan juntas y versionadas.

**Impacto:** `.claude/` ahora contiene tanto documentación de proyecto como infraestructura de agentes/skills.

---

## ADR-002 — `CLAUDE.md` en la raíz del repositorio
**Fecha:** 2026-06-29 · **Estado:** Aceptada

**Contexto:** Se necesita un punto de entrada que se cargue automáticamente cada sesión.

**Decisión:** Colocar `CLAUDE.md` en la raíz (convención de Claude Code) con el protocolo de sesión y punteros a la memoria. `WORKFLOW.md` se ubica en `.claude/WORKFLOW.md`.

**Motivo:** `CLAUDE.md` en raíz se autocarga; centraliza el protocolo de arranque.

**Impacto:** Toda sesión arranca leyendo `CLAUDE.md` → `project-summary.md` → `current-phase.md` → `pending.md`.

---

## ADR-003 — `.gitignore` poblado en Fase 0
**Fecha:** 2026-06-29 · **Estado:** Aceptada

**Contexto:** El `.gitignore` estaba vacío; riesgo de commitear `node_modules/`, `.env`, builds, logs (incumple SDD secc. 24.7 e INF-001/SEC-006).

**Decisión:** Poblar `.gitignore` con dependencias, builds, secretos, cobertura, logs, temporales y archivos de IDE/SO.

**Motivo:** Evitar fugas de secretos y ruido en el repositorio desde el inicio.

**Impacto:** Base segura para los commits desde Fase 1.

---

## ADR-004 — Modo Frontend First (prioridad sobre orden SDD)
**Fecha:** 2026-06-29 · **Estado:** Aceptada

**Contexto:** El SDD define un orden: Infraestructura → DB → Auth → Layout → Componentes → Módulos. El usuario decide cambiar el orden para priorizar una demostración visual funcional antes de integrar el backend.

**Decisión:** Adoptar modo "Frontend First": Design System → Layout General → Pantallas con Mock Data → DB → Backend → Integración real.

**Motivo:** Permite validar la experiencia visual completa con el cliente/stakeholder antes de invertir en backend. Reduce el riesgo de rehacer UI tras feedback.

**Impacto:** El orden de fases en `project-status.json` fue actualizado. El SDD sigue siendo la referencia técnica; solo el orden de implementación cambia. Las restricciones de seguridad del SDD (SEC-001 a SEC-010) se respetan igualmente.

---

## ADR-005 — ESLint override para `src/shared/ui/`
**Fecha:** 2026-06-29 · **Estado:** Aceptada

**Contexto:** shadcn/ui genera archivos que exportan tanto componentes como variantes (`buttonVariants`, `badgeVariants`) y hooks (`useFormField`) desde el mismo archivo. Esto activa la regla `react-refresh/only-export-components` que consideramos error con `--max-warnings 0`.

**Decisión:** Desactivar `react-refresh/only-export-components` solo para `src/shared/ui/**` mediante un override en `eslint.config.js`.

**Motivo:** `src/shared/ui/` es una librería de componentes base (no código de aplicación). La regla react-refresh aplica a feature components, no a una UI library. Modificar los archivos generados por shadcn crea deuda de mantenimiento al actualizar componentes.

**Impacto:** La regla sigue activa en todo `src/` excepto `src/shared/ui/`.

---

## ADR-010 — Módulo `roles-permissions` como documentación de negocio autorizada del sistema de autorización
**Fecha:** 2026-07-08 · **Estado:** Aceptada

**Contexto:** El sistema aprueba la incorporación del rol `tecnico` (Técnico) como rol activo en el MVP, posicionado en el nivel jerárquico 4 junto a `worker` (Trabajador). La documentación de negocio del sistema de autorización estaba fragmentada: existía `docs/modules/permissions/` como placeholder generado en la iteración 3.16 (4 roles), y `docs/architecture/permissions/README.md` como guía de implementación técnica. Ninguno de los dos capturaba el sistema completo de 6 roles (4 activos + 2 futuros) ni las dudas de negocio aún sin resolver.

**Decisión:** Crear `docs/modules/roles-permissions/` como el módulo de documentación de negocio definitivo del sistema de autorización, con los archivos README.md, DOMAIN.md y FUTURE.md. El módulo cubre los 6 roles del sistema, el modelo RBAC, los conceptos de scope, jerarquía, permisos especiales y funcionalidades futuras FUT-RP-001 a FUT-RP-012.

**Motivo:** La separación entre documentación de negocio (`docs/modules/roles-permissions/`) y documentación técnica (`docs/architecture/permissions/`) es consistente con la metodología Document-First adoptada en la iteración 3.16. El módulo `docs/modules/permissions/` preexistente es un placeholder que deberá ser revisado y alineado o marcado como deprecado una vez aprobado este nuevo módulo.

**Impacto:** Dos dudas bloqueantes documentadas (DUDA-RP-001 y DUDA-RP-002) deben ser resueltas antes de crear PERMISSIONS.md y las implementaciones técnicas del rol `tecnico`. La distinción exacta de permisos entre `worker` y `tecnico` es incierta en esta etapa.

---

## ADR-011 — Documentación funcional del módulo Áreas: apertura de DUDA-AR-001
**Fecha:** 2026-07-08 · **Estado:** Aceptada

**Contexto:** El módulo Áreas tenía documentación preexistente en `docs/modules/areas/` pero en formato técnico (con tipos SQL, sin tabla de transiciones de estado, sin relaciones con Dashboard y Reportes, sin convención de códigos y sin FUTURE.md en formato FUT-AR-###). La metodología Document-First exige documentación de negocio como prerequisito a cualquier decisión técnica. El SDD (§7 y §15.1) menciona áreas como atributo en usuarios y tickets y como módulo configurable, pero no define explícitamente si un área pertenece a una sucursal o directamente a la empresa.

**Decisión:** Reescribir los tres documentos de análisis funcional del módulo Áreas (README.md, DOMAIN.md, FUTURE.md) en lenguaje de negocio, siguiendo el patrón establecido por `docs/modules/branches/`. Abrir `DUDA-AR-001` para documentar la ambigüedad sobre la relación área-sucursal, bloqueando el avance de DATABASE.md y la API hasta que el usuario tome una decisión explícita. Registrar diez funcionalidades futuras `FUT-AR-001` a `FUT-AR-010`.

**Motivo:** La ambigüedad sobre la jerarquía área-sucursal tiene impacto directo en el modelo de datos y las reglas de negocio. Diseñar la base de datos sin resolver esta duda generaría deuda técnica costosa de revertir. La documentación funcional primero garantiza que la decisión se tome con el contexto completo antes de escribir código o migraciones.

**Impacto:** DATABASE.md y API.md del módulo Áreas quedan bloqueados hasta resolución de DUDA-AR-001 por el usuario. Las implementaciones de los módulos Usuarios y Tickets que dependen del campo de área deben esperar la misma resolución para la parte relativa a la relación área-sucursal.

---

## ADR-012 — Documentación funcional del módulo Catálogos del Sistema
**Fecha:** 2026-07-08 · **Estado:** Aceptada

**Contexto:** El SDD (Regla 6) establece que todo catálogo es configurable y nunca hardcodeado, pero no existía documentación de negocio que describiera la estructura de dos niveles (Tipo de Catálogo / Elemento de Catálogo), el inventario mínimo del MVP ni las relaciones con todos los módulos consumidores. La metodología Document-First exige esta documentación como prerequisito a cualquier decisión técnica sobre catálogos.

**Decisión:** Crear `docs/modules/system-catalogs/` con tres documentos iniciales: README.md (alcance, módulos relacionados, inventario, estado del módulo), DOMAIN.md (entidades, estados, relaciones, glosario y cinco dudas abiertas DUDA-SC-001 a DUDA-SC-005) y FUTURE.md (diez funcionalidades futuras FUT-SC-001 a FUT-SC-010). La estructura de dos niveles Tipo/Elemento es el concepto central del módulo. Los cinco documentos restantes (BUSINESS-RULES.md, FLOWS.md, PERMISSIONS.md, VALIDATIONS.md, EVENTS.md) quedan pendientes.

**Motivo:** Cinco dudas de negocio bloqueantes (DUDA-SC-001 a DUDA-SC-005) deben ser resueltas por el usuario antes de que DATABASE.md y los contratos de API puedan definirse. Las decisiones sobre scope global vs. por empresa, permisos del Administrador para crear Tipos, configurabilidad de los estados del workflow y la coexistencia con el módulo Tipos de Servicio tienen impacto directo en el modelo de datos y la arquitectura de módulos.

**Impacto:** Los módulos BUSINESS-RULES.md, FLOWS.md, PERMISSIONS.md, VALIDATIONS.md y EVENTS.md del módulo Catálogos del Sistema quedan bloqueados en sus aspectos que dependen de DUDA-SC-001 a DUDA-SC-005. El módulo Tipos de Servicio (`docs/modules/service-types/`) permanece vigente hasta la resolución de DUDA-SC-005.

---

## ADR-013 — Auditoría de Arquitectura Fase 7.0: correcciones de capa aplicadas
**Fecha:** 2026-07-11 · **Estado:** Aceptada

**Contexto:** Antes del Release Candidate se realizó una auditoría completa de separación de capas en frontend y backend. Se detectaron imports directos de `@mocks/data` en componentes de infraestructura de la app (AppHeader, AppSidebar, MobileNav) para leer el conteo de notificaciones no leídas, en lugar de leer del store de Zustand ya existente.

**Decisión:** Reemplazar en los tres componentes `app/` el import de `getUnreadNotifications` desde `@mocks/data` por `useNotificationsStore` desde `@features/notifications/store/notifications.store`, usando el selector `state.notifications.filter(n => !n.read).length`.

**Motivo:** Los componentes de layout deben leer estado de la capa de store, no de datos mock. El store ya existía con la lógica correcta. El fix es atómico (3 archivos, mismo cambio) y no introduce nueva arquitectura.

**Impacto:** Se eliminan 3 de los 8 imports de `@mocks/data` que existían en el proyecto. Quedan 5 pendientes que son mocks de catálogos (sucursales, áreas, usuarios, tipos de servicio) con TODOs documentados pendientes de endpoints de catálogos en el backend. El store de notificaciones sigue inicializando desde mock data — su limpieza es trabajo del agente frontend durante la integración real con Supabase Realtime.

---

## PENDIENTE-001 — Typo en `docs/WROKFLOWS.md`
**Fecha:** 2026-06-29 · **Estado:** Propuesta (requiere aprobación)

**Contexto:** El archivo está en staging como `WROKFLOWS.md` (debería ser `WORKFLOWS.md`). Está vacío.

**Propuesta:** Renombrar a `docs/WORKFLOWS.md` (`git mv`). No se ejecuta hasta aprobación del usuario, por estar ya rastreado en git.

**Impacto:** Consistencia de nombres de documentación.
