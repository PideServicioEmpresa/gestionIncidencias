# Changelog

> Cambios significativos del proyecto. Formato: fecha — descripción. Se actualiza tras cada commit.

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
