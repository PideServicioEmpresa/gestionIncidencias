# Tareas Completadas

> Registro acumulativo. Se añade al final; no se borra.

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
