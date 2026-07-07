# Changelog

> Cambios significativos del proyecto. Formato: fecha — descripción. Se actualiza tras cada commit.

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
