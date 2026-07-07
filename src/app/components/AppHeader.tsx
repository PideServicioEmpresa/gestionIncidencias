import { Menu, Bell, LogOut, User, Settings, Command } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@shared/ui/button'
import { Badge } from '@shared/ui/badge'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@shared/ui/dropdown-menu'
import { ROUTES } from '@constants/index'
import { useAuthStore } from '@store/auth.store'
import { getUnreadNotifications } from '@mocks/data'

interface AppHeaderProps {
  onMenuClick?: () => void
  title?: string
  onCommandOpen?: () => void
}

export function AppHeader({ onMenuClick, title, onCommandOpen }: AppHeaderProps) {
  const navigate = useNavigate()
  const { user, clearAuth } = useAuthStore()
  const unreadCount = getUnreadNotifications().length

  const handleLogout = () => {
    clearAuth()
    navigate(ROUTES.LOGIN)
  }

  return (
    <header className="flex h-header shrink-0 items-center justify-between border-b bg-background px-4 lg:px-6">
      {/* Left: hamburger (mobile) + title */}
      <div className="flex items-center gap-3">
        {onMenuClick && (
          <Button
            variant="ghost"
            size="icon"
            className="lg:hidden"
            onClick={onMenuClick}
            aria-label="Abrir menú"
          >
            <Menu className="h-5 w-5" />
          </Button>
        )}
        {title && <h1 className="text-sm font-semibold text-foreground">{title}</h1>}
      </div>

      {/* Right: command button + notifications + avatar */}
      <div className="flex items-center gap-1">
        {/* Command Menu trigger */}
        <Button
          variant="outline"
          size="sm"
          className="h-7 gap-1.5 px-2 text-xs text-muted-foreground"
          onClick={onCommandOpen}
          aria-label="Abrir menú de comandos"
        >
          <Command className="h-3 w-3" />
          <span className="hidden sm:inline">Ctrl+K</span>
        </Button>

        {/* Notifications bell */}
        <Button
          variant="ghost"
          size="icon"
          className="relative hidden lg:flex"
          onClick={() => navigate(ROUTES.NOTIFICATIONS)}
          aria-label="Notificaciones"
        >
          <Bell className="h-5 w-5" />
          {unreadCount > 0 && (
            <Badge className="absolute -right-0.5 -top-0.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-destructive px-1 text-[9px] font-bold text-destructive-foreground">
              {unreadCount > 99 ? '99+' : unreadCount}
            </Badge>
          )}
        </Button>

        {/* User dropdown */}
        {user && (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" className="flex items-center gap-2 px-2">
                <div className="flex h-7 w-7 items-center justify-center rounded-full bg-primary/20 text-[10px] font-bold text-primary">
                  {user.nombre.charAt(0)}
                  {user.apellido?.charAt(0) ?? ''}
                </div>
                <span className="hidden max-w-20 truncate text-xs font-medium lg:block">
                  {user.nombre}
                </span>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-52">
              <DropdownMenuLabel className="font-normal">
                <p className="text-sm font-semibold">
                  {user.nombre} {user.apellido}
                </p>
                <p className="text-xs text-muted-foreground">{user.correo}</p>
              </DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => navigate(ROUTES.PROFILE)}>
                <User className="mr-2 h-4 w-4" />
                Mi perfil
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => navigate(ROUTES.SETTINGS)}>
                <Settings className="mr-2 h-4 w-4" />
                Configuración
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={handleLogout} className="text-destructive">
                <LogOut className="mr-2 h-4 w-4" />
                Cerrar sesión
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        )}
      </div>
    </header>
  )
}
