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

## PENDIENTE-001 — Typo en `docs/WROKFLOWS.md`
**Fecha:** 2026-06-29 · **Estado:** Propuesta (requiere aprobación)

**Contexto:** El archivo está en staging como `WROKFLOWS.md` (debería ser `WORKFLOWS.md`). Está vacío.

**Propuesta:** Renombrar a `docs/WORKFLOWS.md` (`git mv`). No se ejecuta hasta aprobación del usuario, por estar ya rastreado en git.

**Impacto:** Consistencia de nombres de documentación.
