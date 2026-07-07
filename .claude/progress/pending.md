# Tareas Pendientes

> Se actualiza tras cada tarea y commit. Nunca se elimina trabajo completado; se mueve a `completed.md`.

---

## Bloqueante actual — DOBLE BLOQUEO

**1. COMMIT PENDIENTE DE AUTORIZACIÓN:**
Los cambios de las iteraciones 3.5, 3.6, 3.7, 3.8, 3.9, 3.10, 3.11, 3.12 y 3.13 están implementados en el árbol de trabajo pero NO commiteados.
**NO ejecutar git add, git commit ni git push sin autorización expresa del usuario.**
El usuario debe indicar explícitamente "haz el commit" o "autorizo el commit" para proceder.

**2. NO INICIAR FASE 4 (Base de Datos / Supabase) SIN APROBACIÓN EXPLÍCITA DEL USUARIO.**
La serie de subfases 3.x está completa (3.0 → 3.13). Primero se commitea, luego el usuario da
aprobación visual final, y solo entonces se avanza a Fase 4.
**Supabase/PostgreSQL: NO iniciar hasta recibir aprobación explícita del usuario.**

---

## Iteraciones 3.5 → 3.13 — Implementadas, pendientes de commit

Los cambios están en el árbol de trabajo (working tree). Archivos modificados incluyen:

- `src/app/layouts/AppLayout.tsx` — ScrollToTop + título fallback "Pidde Servicio"
- `src/app/layouts/AuthLayout.tsx` — nombre de marca "Pidde Servicio" en panel desktop y mobile
- `src/app/components/AppSidebar.tsx` — nombre "Pidde Servicio", badge notif rojo bg-destructive, cap 99+
- `src/app/components/MobileNav.tsx` — migrado a grid-cols-5 (centrado perfecto), badge notif rojo bg-destructive, cap 99+
- `src/app/components/AppHeader.tsx` — badge notif rojo bg-destructive, cap 99+
- `src/styles/globals.css` — glow animations + ps-glow-card intensificado (0.45/0.3)
- `src/shared/ui/card.tsx` — borde azul semitransparente por defecto (border-primary/25)
- `src/features/auth/pages/LoginPage.tsx` — login premium + labels obligatorio/opcional completados
- `src/features/dashboard/pages/DashboardPage.tsx` — glow cards en todas las chart cards, "Ver todos" en todas las secciones
- `src/features/tickets/pages/MyTicketsPage.tsx` — sort fecha, filtros fecha, tabla PC, EditTicketSheet lateral, filtros mobile 2 cols, menú de acciones reestructurado por rol, historial de cambios Dialog, helper getAssignedBy, "Por:" en tarjeta mobile y tabla desktop, columna "F. creación"
- `src/features/tickets/pages/TicketDetailPage.tsx` — history tracking, PC layout
- `src/features/tickets/pages/CreateTicketPage.tsx` — "Otros" condicional, evidencias, 2 cols PC
- `src/features/notifications/pages/NotificationsPage.tsx` — Undo, Gmail styles, 2 cols PC
- `src/features/profile/pages/ProfilePage.tsx` — foto FileReader, ojo passwords, grupos PC, badges de rol con colores semánticos
- `src/features/settings/pages/SettingsPage.tsx` — tooltips, 2 cols PC, labels obligatorio/opcional completados
- `src/features/users/pages/UsersPage.tsx` — campo Estado, nomenclatura, labels obligatorio/opcional en modales
- `src/mocks/data.ts` — nomenclatura Empresa/Sucursal/Tipo de Servicio
- `src/constants/index.ts` — nomenclatura actualizada
- `src/shared/components/PageBreadcrumb.tsx` — NUEVO
- `src/shared/components/CommandMenu.tsx` — NUEVO
- `src/shared/components/PageSkeletons.tsx` — NUEVO
- `index.html` — title y apple-mobile-web-app-title actualizados a "Pidde Servicio"; favicon cambiado a /logo.jpeg

---

## Fase 4 — Base de Datos con Supabase (bloqueada — requiere aprobación explícita del usuario)

NO iniciar hasta:
1. Commit de iteraciones 3.5 + 3.6 autorizado y ejecutado.
2. Aprobación visual final del usuario.

### Trabajo a realizar
- [ ] Migraciones SQL: tablas usuarios, empresas, sucursales, tickets, comentarios, evidencias, notificaciones, auditoria
- [ ] Índices y claves foráneas
- [ ] Row Level Security (RLS) por rol
- [ ] Funciones y triggers PostgreSQL (historial, auditoría, contadores)
- [ ] Seeds de datos de prueba reales
- [ ] Supabase conectado al proyecto React

---

## Mejoras opcionales implementadas en 3.5/3.6 (ya resueltas)

- [x] Vista desktop del Ticket Detail (columna derecha con metadata + acciones) → ancho completo implementado
- [x] Filtro por sucursal en el Dashboard Admin → filtros empresa/sucursal implementados
- [x] Tabla en lugar de cards para la lista de tickets en desktop → tabla PC implementada en MyTicketsPage
- [x] Animaciones de transición entre rutas → ScrollToTop implementado
- [x] Sidebar colapsable en desktop → pendiente (no solicitado aún)
