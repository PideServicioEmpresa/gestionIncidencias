# Fase Actual

## Iteración 3.13 — Favicon logo.jpeg, "Asignado a / Por:" en tickets y columna "F. creación" · `completada` | Pendiente: commit y aprobación del usuario

> **Modo activo: Frontend First** — Aprobado por usuario el 2026-06-29.
> Orden modificado: Design System → UI Mock Data → DB → Backend → Integración real.
> La serie de subfases 3.x continua (3.0 → 3.13). El frontend tiene datos mock, flujo interactivo
> completo, UX premium, nomenclatura unificada, componentes transversales, consistencia visual auditada,
> design system refactorizado, vistas dedicadas para el CRUD de usuarios, sheet lateral completo
> para modificar tickets, branding completo con logo en favicon, y trazabilidad de asignación
> visible en tarjetas mobile y tabla desktop.

> **REGLA DE NO COMMIT:** Los cambios de las iteraciones 3.5 a 3.13 están implementados pero
> **NO commiteados**. El commit requiere autorización explícita del usuario antes de ejecutarse.

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
- [x] `/tickets/:id` — History tracking automático, PC layout ancho completo, breadcrumb
- [x] `/usuarios` — Tabla + filtros, acciones navegan a vistas dedicadas, breadcrumb
- [x] `/usuarios/nuevo` — Formulario crear usuario con validación, breadcrumb
- [x] `/usuarios/:id` — Vista de perfil de usuario en modo lectura, breadcrumb
- [x] `/usuarios/:id/editar` — Formulario editar usuario pre-poblado, breadcrumb
- [x] `/notificaciones` — Sort cronológico descendente, patrón Undo, estilos Gmail no leídas, 2 columnas PC
- [x] `/perfil` — Layout 2 columnas PC, foto clickeable FileReader, ojo en contraseñas, breadcrumb
- [x] `/reportes` — Descarga simulada con toast.promise, breadcrumb
- [x] `/auditoria` — Exportar con toast.promise, breadcrumb
- [x] `/configuracion` — Layout PC con navegación lateral, tooltips ayuda, breadcrumb
- [x] `/ui` — Design System Showcase (Fase 2, mantenida)

---

## Nuevos componentes transversales (Fase 3.6)

| Componente | Ruta | Propósito |
|---|---|---|
| `PageBreadcrumb` | `src/shared/components/PageBreadcrumb.tsx` | Navegación jerárquica en todas las páginas |
| `CommandMenu` | `src/shared/components/CommandMenu.tsx` | Búsqueda global `Ctrl+K` |
| `PageSkeletons` | `src/shared/components/PageSkeletons.tsx` | Skeletons de carga específicos por página |

---

## Próximo paso (pendiente autorización del usuario)

1. **Autorizar el commit** de las iteraciones 3.5 → 3.13 (el usuario debe confirmarlo explícitamente antes de ejecutar cualquier operación git).
2. Aprobación visual final → Fase 4 — Base de Datos (Supabase/PostgreSQL).
3. **NO iniciar Fase 4 hasta aprobación explícita del usuario.**
4. **NO iniciar Supabase/Base de Datos sin aprobación explícita del usuario.**

---

## Fases anteriores completadas

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
