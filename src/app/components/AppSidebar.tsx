import { NavLink, useLocation } from 'react-router-dom'
import {
  LayoutDashboard,
  Ticket,
  Bell,
  Users,
  Settings,
  FileBarChart,
  Shield,
  Plus,
  ChevronRight,
} from 'lucide-react'
import { cn } from '@lib/utils'
import { ROUTES } from '@constants/index'
import { useAuthStore } from '@store/auth.store'
import { ScrollArea } from '@shared/ui/scroll-area'
import { Badge } from '@shared/ui/badge'
import { getUnreadNotifications } from '@mocks/data'

interface NavItem {
  label: string
  to: string
  icon: React.ComponentType<{ className?: string }>
  badge?: number
  roles?: string[]
}

const workerNav: NavItem[] = [
  { label: 'Dashboard', to: ROUTES.DASHBOARD, icon: LayoutDashboard },
  { label: 'Mis tickets', to: ROUTES.TICKETS, icon: Ticket },
  { label: 'Notificaciones', to: ROUTES.NOTIFICATIONS, icon: Bell },
]

const adminNav: NavItem[] = [
  { label: 'Dashboard', to: ROUTES.DASHBOARD, icon: LayoutDashboard },
  { label: 'Tickets', to: ROUTES.TICKETS, icon: Ticket },
  { label: 'Usuarios', to: ROUTES.USERS, icon: Users },
  { label: 'Notificaciones', to: ROUTES.NOTIFICATIONS, icon: Bell },
  { label: 'Reportes', to: ROUTES.REPORTS, icon: FileBarChart },
  { label: 'Auditoría', to: ROUTES.AUDIT, icon: Shield },
  { label: 'Configuración', to: ROUTES.SETTINGS, icon: Settings },
]

function NavItemRow({ item, unread }: { item: NavItem; unread: number }) {
  const location = useLocation()
  const isActive =
    item.to === ROUTES.DASHBOARD
      ? location.pathname === item.to
      : location.pathname.startsWith(item.to)

  return (
    <NavLink
      to={item.to}
      className={cn(
        'group flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-all',
        isActive
          ? 'bg-primary text-primary-foreground shadow-sm'
          : 'text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground',
      )}
    >
      <item.icon className="h-4 w-4 shrink-0" />
      <span className="flex-1 truncate">{item.label}</span>
      {item.to === ROUTES.NOTIFICATIONS && unread > 0 && (
        <Badge
          className={cn(
            'h-5 min-w-5 px-1 text-[10px] font-bold',
            isActive ? 'bg-primary-foreground text-primary' : 'bg-primary text-primary-foreground',
          )}
        >
          {unread > 9 ? '9+' : unread}
        </Badge>
      )}
      {isActive && !item.badge && <ChevronRight className="h-3 w-3 opacity-60" />}
    </NavLink>
  )
}

export function AppSidebar() {
  const user = useAuthStore((s) => s.user)
  const isAdmin = user?.rol === 'admin' || user?.rol === 'superadmin'
  const navItems = isAdmin ? adminNav : workerNav
  const unreadCount = getUnreadNotifications().length

  return (
    <aside className="bg-sidebar flex h-full w-sidebar flex-col border-r">
      {/* Logo */}
      <div className="flex h-header shrink-0 items-center gap-3 border-b px-4">
        <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
          <Ticket className="h-4 w-4 text-primary-foreground" />
        </div>
        <div className="flex flex-col leading-none">
          <span className="text-sidebar-foreground text-sm font-bold tracking-tight">Pide</span>
          <span className="text-xs font-semibold text-primary">Servicio</span>
        </div>
      </div>

      {/* Navigation */}
      <ScrollArea className="flex-1 px-3 py-4">
        <nav className="flex flex-col gap-1">
          {navItems.map((item) => (
            <NavItemRow key={item.to} item={item} unread={unreadCount} />
          ))}
        </nav>

        {/* Quick action */}
        <div className="mt-6 border-t pt-4">
          <NavLink
            to={ROUTES.TICKETS_NEW}
            className="flex items-center gap-3 rounded-lg bg-primary/10 px-3 py-2.5 text-sm font-semibold text-primary transition-all hover:bg-primary hover:text-primary-foreground"
          >
            <Plus className="h-4 w-4 shrink-0" />
            <span>Nuevo ticket</span>
          </NavLink>
        </div>
      </ScrollArea>

      {/* User footer */}
      {user && (
        <div className="shrink-0 border-t p-3">
          <NavLink
            to={ROUTES.PROFILE}
            className="hover:bg-sidebar-accent flex items-center gap-3 rounded-lg px-2 py-2 transition-all"
          >
            <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/20 text-xs font-bold text-primary">
              {user.nombre.charAt(0)}
              {user.apellido?.charAt(0) ?? ''}
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-sidebar-foreground truncate text-xs font-semibold">
                {user.nombre} {user.apellido}
              </p>
              <p className="truncate text-[10px] capitalize text-muted-foreground">{user.rol}</p>
            </div>
          </NavLink>
        </div>
      )}
    </aside>
  )
}
