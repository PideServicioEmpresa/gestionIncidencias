import { NavLink, useLocation, useNavigate } from 'react-router-dom'
import { LayoutDashboard, Ticket, Plus, Bell, User } from 'lucide-react'
import { cn } from '@lib/utils'
import { ROUTES } from '@constants/index'
import { getUnreadNotifications } from '@mocks/data'

interface MobileNavItem {
  label: string
  to: string
  icon: React.ComponentType<{ className?: string }>
  isAction?: boolean
}

export function MobileNav() {
  const location = useLocation()
  const navigate = useNavigate()
  const unreadCount = getUnreadNotifications().length

  const navItems: MobileNavItem[] = [
    { label: 'Inicio', to: ROUTES.DASHBOARD, icon: LayoutDashboard },
    { label: 'Tickets', to: ROUTES.TICKETS, icon: Ticket },
    { label: 'Nuevo', to: ROUTES.TICKETS_NEW, icon: Plus, isAction: true },
    { label: 'Notificaciones', to: ROUTES.NOTIFICATIONS, icon: Bell },
    { label: 'Perfil', to: ROUTES.PROFILE, icon: User },
  ]

  return (
    <nav className="fixed bottom-0 left-0 right-0 z-50 grid h-16 grid-cols-5 border-t bg-background/95 backdrop-blur-sm supports-[backdrop-filter]:bg-background/80 lg:hidden">
      {navItems.map((item) => {
        const isActive =
          item.to === ROUTES.DASHBOARD
            ? location.pathname === item.to
            : location.pathname.startsWith(item.to)

        if (item.isAction) {
          return (
            <button
              key={item.to}
              onClick={() => navigate(item.to)}
              className="flex flex-col items-center justify-center"
              aria-label="Crear nuevo ticket"
            >
              <span className="flex -translate-y-3 flex-col items-center gap-0.5">
                <span className="flex h-12 w-12 items-center justify-center rounded-full bg-primary shadow-lg shadow-primary/30 transition-transform active:scale-95">
                  <Plus className="h-5 w-5 text-primary-foreground" />
                </span>
                <span className="text-[10px] font-medium text-muted-foreground">{item.label}</span>
              </span>
            </button>
          )
        }

        return (
          <NavLink
            key={item.to}
            to={item.to}
            className="flex flex-col items-center justify-center gap-0.5 py-2"
          >
            <div className="relative">
              <item.icon
                className={cn(
                  'h-5 w-5 transition-colors',
                  isActive ? 'text-primary' : 'text-muted-foreground',
                )}
              />
              {item.to === ROUTES.NOTIFICATIONS && unreadCount > 0 && (
                <span className="absolute -right-1 -top-1 flex h-3.5 min-w-3.5 items-center justify-center rounded-full bg-destructive px-0.5 text-[8px] font-bold text-destructive-foreground">
                  {unreadCount > 99 ? '99+' : unreadCount}
                </span>
              )}
            </div>
            <span
              className={cn(
                'text-[10px] font-medium transition-colors',
                isActive ? 'text-primary' : 'text-muted-foreground',
              )}
            >
              {item.label}
            </span>
          </NavLink>
        )
      })}
    </nav>
  )
}
