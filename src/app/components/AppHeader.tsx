import { useState } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import {
  Menu,
  Bell,
  LogOut,
  User,
  Settings,
  Command,
  CheckCheck,
  Info,
  AlertCircle,
  Ticket,
  MessageSquare,
  Building2,
} from 'lucide-react'
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
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
} from '@shared/ui/sheet'
import { ROUTES, ticketDetailPath } from '@constants/index'
import { useAuthStore } from '@store/auth.store'
import {
  useConteoNotificaciones,
  useNotificaciones,
  useMarcarLeida,
} from '@features/notifications/hooks/useNotificaciones'
import { authService } from '@features/auth/services/authService'
import { SearchableSelect } from '@shared/components/SearchableSelect'
import { cn } from '@lib/utils'

const ROLES_MULTI_SUCURSAL = ['tecnico', 'trabajador', 'usuario']

interface AppHeaderProps {
  onMenuClick?: () => void
  title?: string
  onCommandOpen?: () => void
}

const NOTIF_TYPE_ICON: Record<string, React.ElementType> = {
  'ticket.nuevo': Ticket,
  'ticket.asignado': CheckCheck,
  'ticket.reasignado': CheckCheck,
  'ticket.pendiente_validacion': AlertCircle,
  'ticket.cerrado': AlertCircle,
  'ticket.rechazado': AlertCircle,
  'ticket.comentario': MessageSquare,
}

function timeAgoShort(dateStr: string): string {
  const diff = Math.floor((Date.now() - new Date(dateStr).getTime()) / 1000)
  if (diff < 60) return 'ahora'
  if (diff < 3600) return `${Math.floor(diff / 60)}m`
  if (diff < 86400) return `${Math.floor(diff / 3600)}h`
  return `${Math.floor(diff / 86400)}d`
}

export function AppHeader({ onMenuClick, title, onCommandOpen }: AppHeaderProps) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const user = useAuthStore((s) => s.user)
  const sucursalActiva = useAuthStore((s) => s.sucursalActiva)
  const sucursalesDisponibles = useAuthStore((s) => s.sucursalesDisponibles)
  const cambiarSucursalActiva = useAuthStore((s) => s.cambiarSucursalActiva)

  const isAdmin = user?.rol === 'admin' || user?.rol === 'superadmin'
  const esMultiSucursal = user?.rol ? ROLES_MULTI_SUCURSAL.includes(user.rol) : false

  const [sheetOpen, setSheetOpen] = useState(false)
  const [sucursalSeleccionada, setSucursalSeleccionada] = useState<string>(sucursalActiva ?? '')

  const opcionesSucursal = sucursalesDisponibles
    .filter((s) => s.activo)
    .map((s) => ({ value: s.sucursalId, label: s.sucursalNombre }))

  const nombreSucursalActiva =
    sucursalesDisponibles.find((s) => s.sucursalId === sucursalActiva)?.sucursalNombre ?? null

  const handleAbrirSheet = () => {
    setSucursalSeleccionada(sucursalActiva ?? '')
    setSheetOpen(true)
  }

  const handleConfirmarSucursal = () => {
    if (!sucursalSeleccionada) return
    cambiarSucursalActiva(sucursalSeleccionada)
    setSheetOpen(false)
    // Forzar recarga de todos los listados con el nuevo header X-Sucursal-Activa
    queryClient.invalidateQueries()
  }

  const { data: conteoData } = useConteoNotificaciones()
  const unreadCount = conteoData?.sinLeer ?? 0

  const { data: notifsData } = useNotificaciones({ tamanoPagina: 6 })
  const { mutate: marcarLeida } = useMarcarLeida()

  const recientes = (notifsData?.items ?? []).slice(0, 5)

  const handleNotifClick = (id: string, ticketId: string | null) => {
    marcarLeida(id)
    if (ticketId) navigate(ticketDetailPath(ticketId))
    else navigate(ROUTES.NOTIFICATIONS)
  }

  const handleLogout = async () => {
    await authService.logout()
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

        {/* Notifications bell — dropdown con recientes */}
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              variant="ghost"
              size="icon"
              className="relative"
              aria-label={
                unreadCount > 0 ? `Notificaciones — ${unreadCount} sin leer` : 'Notificaciones'
              }
            >
              <Bell className="h-5 w-5" aria-hidden="true" />
              {unreadCount > 0 && (
                <Badge
                  aria-hidden="true"
                  className="absolute -right-0.5 -top-0.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-destructive px-1 text-[9px] font-bold text-destructive-foreground"
                >
                  {unreadCount > 99 ? '99+' : unreadCount}
                </Badge>
              )}
            </Button>
          </DropdownMenuTrigger>

          <DropdownMenuContent align="end" className="w-80 p-0">
            <DropdownMenuLabel className="flex items-center justify-between px-3 py-2.5">
              <span className="text-sm font-semibold">Notificaciones</span>
              {unreadCount > 0 && (
                <Badge variant="secondary" className="text-[10px]">
                  {unreadCount} sin leer
                </Badge>
              )}
            </DropdownMenuLabel>
            <DropdownMenuSeparator className="m-0" />

            {recientes.length === 0 ? (
              <div className="flex flex-col items-center gap-1 px-3 py-6 text-center">
                <Bell className="h-6 w-6 text-muted-foreground/50" />
                <p className="text-xs text-muted-foreground">Sin notificaciones</p>
              </div>
            ) : (
              recientes.map((n) => {
                const Icon = NOTIF_TYPE_ICON[n.tipoEvento ?? ''] ?? Info
                const isUnread = !n.esLeida
                return (
                  <DropdownMenuItem
                    key={n.id}
                    className={cn(
                      'flex cursor-pointer items-start gap-2.5 px-3 py-2.5 focus:bg-accent',
                      isUnread && 'bg-primary/5',
                    )}
                    onClick={() => handleNotifClick(n.id, n.ticketId)}
                  >
                    <div
                      className={cn(
                        'mt-0.5 flex h-6 w-6 shrink-0 items-center justify-center rounded-md',
                        isUnread ? 'bg-primary/15' : 'bg-muted',
                      )}
                    >
                      <Icon
                        className={cn(
                          'h-3 w-3',
                          isUnread ? 'text-primary' : 'text-muted-foreground',
                        )}
                      />
                    </div>
                    <div className="min-w-0 flex-1">
                      <p
                        className={cn(
                          'truncate text-xs leading-snug',
                          isUnread
                            ? 'font-semibold text-foreground'
                            : 'font-medium text-foreground/75',
                        )}
                      >
                        {n.titulo}
                      </p>
                      <p className="mt-0.5 line-clamp-1 text-[11px] text-muted-foreground">
                        {n.cuerpo}
                      </p>
                    </div>
                    <span className="shrink-0 text-[10px] text-muted-foreground">
                      {timeAgoShort(n.createdAt)}
                    </span>
                  </DropdownMenuItem>
                )
              })
            )}

            <DropdownMenuSeparator className="m-0" />
            <DropdownMenuItem
              className="justify-center py-2 text-xs font-medium text-primary focus:text-primary"
              onClick={() => navigate(ROUTES.NOTIFICATIONS)}
            >
              Ver todas las notificaciones
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>

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
            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuLabel className="font-normal">
                <p className="text-sm font-semibold">
                  {user.nombre} {user.apellido}
                </p>
                <p className="text-xs text-muted-foreground">{user.correo}</p>
                {esMultiSucursal && nombreSucursalActiva && (
                  <p className="mt-1 flex items-center gap-1 text-xs text-muted-foreground">
                    <Building2 className="h-3 w-3 shrink-0" />
                    <span className="truncate">{nombreSucursalActiva}</span>
                  </p>
                )}
              </DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => navigate(ROUTES.PROFILE)}>
                <User className="mr-2 h-4 w-4" />
                Mi perfil
              </DropdownMenuItem>
              {esMultiSucursal && (
                <DropdownMenuItem onClick={handleAbrirSheet}>
                  <Building2 className="mr-2 h-4 w-4" />
                  Cambiar sucursal
                </DropdownMenuItem>
              )}
              {isAdmin && (
                <DropdownMenuItem onClick={() => navigate(ROUTES.SETTINGS)}>
                  <Settings className="mr-2 h-4 w-4" />
                  Configuración
                </DropdownMenuItem>
              )}
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={handleLogout} className="text-destructive">
                <LogOut className="mr-2 h-4 w-4" />
                Cerrar sesión
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        )}
      </div>

      {/* Sheet de cambio de sucursal — solo roles multi-sucursal */}
      <Sheet open={sheetOpen} onOpenChange={setSheetOpen}>
        <SheetContent side="bottom" className="mx-auto max-w-sm rounded-t-xl pb-8">
          <SheetHeader className="mb-4 text-left">
            <SheetTitle>Cambiar sucursal</SheetTitle>
            <SheetDescription>Selecciona la sucursal en la que vas a operar.</SheetDescription>
          </SheetHeader>
          <SearchableSelect
            options={opcionesSucursal}
            value={sucursalSeleccionada}
            onChange={setSucursalSeleccionada}
            placeholder="Seleccionar sucursal..."
            searchPlaceholder="Buscar sucursal..."
            emptyMessage="No se encontró la sucursal."
          />
          <SheetFooter className="mt-4 flex-col gap-2 sm:flex-col">
            <Button
              className="w-full"
              disabled={!sucursalSeleccionada || sucursalSeleccionada === sucursalActiva}
              onClick={handleConfirmarSucursal}
            >
              Confirmar
            </Button>
            <Button variant="outline" className="w-full" onClick={() => setSheetOpen(false)}>
              Cancelar
            </Button>
          </SheetFooter>
        </SheetContent>
      </Sheet>
    </header>
  )
}
