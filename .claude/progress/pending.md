# Tareas Pendientes

> Se actualiza tras cada tarea y commit. Nunca se elimina trabajo completado; se mueve a `completed.md`.

---

## Bloqueante actual

**Esperando aprobación del usuario para iniciar Fase 3 — Layout General.**

---

## Fase 3 — Layout General (pendiente de aprobación)

Construir el esqueleto visual de la aplicación: navegación, sidebar y layouts responsive.

### Componentes a construir
- [ ] `AppSidebar.tsx` — sidebar colapsable con ítems de navegación y rol de usuario
- [ ] `AppHeader.tsx` — topbar con avatar, notificaciones, breadcrumb y toggle sidebar
- [ ] `MainLayout.tsx` — actualizar con sidebar + header + content area
- [ ] `AuthLayout.tsx` — layout para pantallas de login/forgot-password
- [ ] `MobileNav.tsx` — bottom navigation bar para móvil (< md)
- [ ] `PageWrapper.tsx` — wrapper con título de página y acciones

### Rutas a definir (sin lógica de negocio)
- [ ] `/login`, `/forgot-password` → AuthLayout
- [ ] `/dashboard`, `/tickets`, `/tickets/:id`, `/tickets/nuevo` → MainLayout
- [ ] `/usuarios`, `/sucursales`, `/areas` → MainLayout (solo SuperAdmin/Admin)
- [ ] `/perfil` → MainLayout
- [ ] `/notificaciones` → MainLayout

### Criterios de finalización
- [ ] Sidebar: fijo en desktop (md+), drawer en mobile.
- [ ] Sidebar: ítems filtrados por rol (mock de rol = admin).
- [ ] Header: toggle de tema, contador de notificaciones (mock), avatar.
- [ ] Routes configuradas con React Router loaders.
- [ ] Layout responsive comprobado en mobile (< 640px) y desktop (> 1024px).
- [ ] TypeScript 0 errores · ESLint 0 errores/warnings · Build OK.
- [ ] Commit validado + aprobación para Fase 4.

---

## Fase 4 — Pantallas con Mock Data (pendiente)

Construir todas las pantallas del sistema usando datos simulados. Sin Supabase, sin lógica de negocio.

### Pantallas a construir (con mock data)
- [ ] Login + Forgot Password
- [ ] Dashboard (métricas, gráficos Recharts, tickets recientes)
- [ ] Lista de Tickets (con filtros, búsqueda, paginación)
- [ ] Detalle de Ticket (timeline, evidencias, comentarios, acciones de estado)
- [ ] Nuevo Ticket (formulario RHF + Zod)
- [ ] Usuarios (tabla, crear, editar)
- [ ] Sucursales y Áreas
- [ ] Perfil de usuario
- [ ] Notificaciones
- [ ] Reportes y Auditoría
