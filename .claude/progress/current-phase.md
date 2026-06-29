# Fase Actual

## Fase 2 — Design System · `en_progreso` | Siguiente: Fase 3 — Layout General

> **Modo activo: Frontend First** — Aprobado por usuario el 2026-06-29.
> Orden modificado: Design System → Layout → UI Mock Data → DB → Backend → Integración real.

---

## Objetivo

Definir y construir la capa visual completa: tokens de diseño, componentes base (shadcn/ui), componentes personalizados del dominio, iconografía, sistema tipográfico y una página `/ui` de verificación (Design System Showcase) navegable y con soporte dark/light.

---

## Criterios de finalización (Fase 2)

- [x] Decisión ADR-004 registrada (Frontend First).
- [x] `project-status.json` actualizado con nuevo orden de fases.
- [ ] `globals.css` con tokens semánticos completos (success, warning, info, priority, ticket-foregrounds).
- [ ] `tailwind.config.ts` con colores semánticos mapeados.
- [ ] Componentes shadcn/ui instalados (≥20 componentes base).
- [ ] Componentes personalizados creados: `Spinner`, `EmptyState`, `StatusBadge`, `PriorityBadge`, `ConfirmDialog`.
- [ ] Página `/ui` — Design System Showcase visible y navegable.
- [ ] Soporte dark/light funcional en todos los componentes.
- [ ] TypeScript: 0 errores.
- [ ] ESLint: 0 errores, 0 warnings.
- [ ] Build de producción: OK.
- [ ] Commit realizado con validación Git Guardian + Release Reviewer.
- [ ] Informe presentado y **aprobación del usuario** para iniciar Fase 3.

---

## Fases anteriores completadas

### Fase 1 — Inicialización · `completada`
React 19 + Vite 5 + TypeScript strict + ESLint + Prettier + Tailwind + shadcn/ui setup + React Router + TanStack Query + Zustand + Supabase + Vitest + Playwright + Husky. Build OK, 5 tests.

### Fase 0 — Infraestructura · `completada`
Estructura `.claude/`, memoria persistente, 11 subagentes, skill control-de-commits, reglas de commits, `.gitignore`.
