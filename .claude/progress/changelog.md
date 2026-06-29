# Changelog

> Cambios significativos del proyecto. Formato: fecha — descripción. Se actualiza tras cada commit.

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
