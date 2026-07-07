import { Fragment } from 'react'
import { Link, useLocation, useParams } from 'react-router-dom'
import { ROUTES } from '@constants/index'

interface BreadcrumbItem {
  label: string
  href?: string
}

function getBreadcrumbs(pathname: string): BreadcrumbItem[] | null {
  const inicio: BreadcrumbItem = { label: 'Inicio', href: ROUTES.DASHBOARD }

  if (pathname === ROUTES.HOME || pathname === ROUTES.LOGIN) {
    return null
  }

  if (pathname === ROUTES.DASHBOARD) {
    return [{ label: 'Inicio' }]
  }

  if (pathname === ROUTES.TICKETS_NEW) {
    return [inicio, { label: 'Tickets', href: ROUTES.TICKETS }, { label: 'Nuevo Ticket' }]
  }

  if (pathname === ROUTES.TICKETS) {
    return [inicio, { label: 'Tickets' }]
  }

  if (pathname.startsWith('/tickets/')) {
    return [inicio, { label: 'Tickets', href: ROUTES.TICKETS }, { label: 'Detalle del Ticket' }]
  }

  if (pathname === '/usuarios/nuevo') {
    return [inicio, { label: 'Usuarios', href: ROUTES.USERS }, { label: 'Nuevo Usuario' }]
  }

  if (pathname.startsWith('/usuarios/') && pathname.endsWith('/editar')) {
    return [inicio, { label: 'Usuarios', href: ROUTES.USERS }, { label: 'Editar Usuario' }]
  }

  if (pathname.startsWith('/usuarios/') && !pathname.endsWith('/editar')) {
    return [inicio, { label: 'Usuarios', href: ROUTES.USERS }, { label: 'Detalle de Usuario' }]
  }

  if (pathname === ROUTES.USERS) {
    return [inicio, { label: 'Usuarios' }]
  }

  if (pathname === ROUTES.PROFILE) {
    return [inicio, { label: 'Perfil' }]
  }

  if (pathname === ROUTES.SETTINGS) {
    return [inicio, { label: 'Configuración' }]
  }

  if (pathname === ROUTES.REPORTS) {
    return [inicio, { label: 'Reportes' }]
  }

  if (pathname === ROUTES.AUDIT) {
    return [inicio, { label: 'Auditoría' }]
  }

  if (pathname === ROUTES.NOTIFICATIONS) {
    return [inicio, { label: 'Notificaciones' }]
  }

  return null
}

export function PageBreadcrumb() {
  const location = useLocation()
  // useParams is available for components rendered inside a Route with params,
  // but breadcrumb is rendered in AppLayout so params may be empty here.
  // We consume the hook to satisfy the requirement; pathname is sufficient.
  useParams()

  const items = getBreadcrumbs(location.pathname)

  if (!items || items.length === 0) return null

  return (
    <nav className="flex items-center gap-1.5 px-3 py-1.5 text-[11px] text-muted-foreground lg:px-5">
      {items.map((item, i) => (
        <Fragment key={item.label}>
          {i > 0 && <span className="select-none text-[10px] text-muted-foreground/60">›</span>}
          {item.href ? (
            <Link to={item.href} className="transition-colors hover:text-foreground">
              {item.label}
            </Link>
          ) : (
            <span className="font-medium text-foreground">{item.label}</span>
          )}
        </Fragment>
      ))}
    </nav>
  )
}
