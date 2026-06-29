# Tareas Pendientes

> Se actualiza tras cada tarea y commit. Nunca se elimina trabajo completado; se mueve a `completed.md`.

---

## Bloqueante actual

**Esperando aprobación visual del usuario para la Fase 3.2 (Design System + Dashboard).**
**NO iniciar Fase 4 (Base de Datos) hasta aprobación explícita del usuario.**

---

## Fase 3.2 — Design System + Dashboard (completada — pendiente aprobación visual)

- [x] Design system oscuro: `#0B0C10` fondo, `#111113` componentes, `#26262B` bordes
- [x] Migración de fuente Inter → Geist (fonts.bunny.net CDN)
- [x] CSS variables actualizadas en `globals.css` (formato HSL)
- [x] `tailwind.config.ts` actualizado con fontFamily Geist
- [x] Paquetes instalados: `geist ^1.7.2`, `@dnd-kit/core`, `@dnd-kit/sortable`, `@dnd-kit/utilities`
- [x] Dashboard Admin reescrito: 9 widgets gráficos distintos (Donut, Bar vertical, Bar horizontal, Area, Stacked Bar, Radar, ComposedChart)
- [x] Interactividad: clic en gráfico → navega a `/tickets?status=X` / `?priority=X` / `?sucursal=X`
- [x] Drag & Drop con `@dnd-kit`: modo edición explícito, persistencia en `sessionStorage`
- [x] `MyTicketsPage`: URL params para prefiltrado desde dashboard + banner contextual
- [x] AppSidebar: indicador activo (borde izquierdo), iconografía mejorada
- [x] TypeScript: 0 errores | ESLint: 0 errores

Commits: `5ebd756` (design system + dashboard), `d5f9385` (package.json)

---

## Fase 4 — Base de Datos con Supabase (pendiente de aprobación)

NO iniciar hasta aprobación explícita del usuario.

### Trabajo a realizar
- [ ] Migraciones SQL: tablas usuarios, sucursales, areas, tickets, comentarios, evidencias, notificaciones, auditoria
- [ ] Índices y claves foráneas
- [ ] Row Level Security (RLS) por rol
- [ ] Funciones y triggers PostgreSQL (historial, auditoría, contadores)
- [ ] Seeds de datos de prueba reales
- [ ] Supabase conectado al proyecto React

---

## Mejoras opcionales (si el usuario las solicita)

- [ ] Vista desktop del Ticket Detail (columna derecha con metadata + acciones)
- [ ] Sidebar colapsable en desktop
- [ ] Filtro por sucursal en el Dashboard Admin
- [ ] Tabla en lugar de cards para la lista de tickets en desktop
- [ ] Animaciones de transición entre rutas
