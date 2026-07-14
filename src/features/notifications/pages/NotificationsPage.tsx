import { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Bell,
  BellOff,
  Ticket,
  CheckCheck,
  MessageSquare,
  AlertCircle,
  Info,
  MailOpen,
  MoreHorizontal,
  MoreVertical,
  BellRing,
  Trash2,
} from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@shared/ui/button'
import { Badge } from '@shared/ui/badge'
import { Card, CardContent, CardHeader } from '@shared/ui/card'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@shared/ui/dropdown-menu'
import { EmptyState } from '@shared/components/EmptyState'
import { ticketDetailPath } from '@constants/index'
import { cn } from '@lib/utils'
import { useNotificaciones, useMarcarLeida, useMarcarTodasLeidas } from '../hooks/useNotificaciones'
import type { NotificacionDto } from '../services/notificacionService'

// ── Tipo interno de la UI ─────────────────────────────────────────────────────

interface LocalNotification {
  id: string
  type: 'ticket_new' | 'ticket_assigned' | 'ticket_status' | 'comment' | 'system'
  title: string
  body: string
  ticketId: string | null
  ticketCode?: string
  read: boolean
  createdAt: string
}

function mapDtoToLocal(dto: NotificacionDto): LocalNotification {
  // El backend no provee un campo "tipo de categoría" visual.
  // TODO: inferir tipo desde un campo adicional cuando el backend lo soporte.
  return {
    id: dto.id,
    type: 'system',
    title: dto.titulo,
    body: dto.cuerpo,
    ticketId: dto.ticketId,
    ticketCode: undefined,
    read: dto.esLeida,
    createdAt: dto.createdAt,
  }
}

// ── Configuración visual por tipo ─────────────────────────────────────────────

const TYPE_CONFIG = {
  ticket_new: {
    icon: Ticket,
    color: 'text-blue-600 dark:text-blue-400',
    bg: 'bg-blue-100 dark:bg-blue-900/30',
    label: 'Nuevo ticket',
  },
  ticket_assigned: {
    icon: CheckCheck,
    color: 'text-primary',
    bg: 'bg-primary/10',
    label: 'Asignación',
  },
  ticket_status: {
    icon: AlertCircle,
    color: 'text-orange-600 dark:text-orange-400',
    bg: 'bg-orange-100 dark:bg-orange-900/30',
    label: 'Cambio de estado',
  },
  comment: {
    icon: MessageSquare,
    color: 'text-purple-600 dark:text-purple-400',
    bg: 'bg-purple-100 dark:bg-purple-900/30',
    label: 'Comentario',
  },
  system: {
    icon: Info,
    color: 'text-muted-foreground',
    bg: 'bg-muted',
    label: 'Sistema',
  },
}

function timeAgo(dateStr: string): string {
  const now = new Date()
  const date = new Date(dateStr)
  const diff = Math.floor((now.getTime() - date.getTime()) / 1000)
  if (diff < 60) return 'Hace un momento'
  if (diff < 3600) return `Hace ${Math.floor(diff / 60)} min`
  if (diff < 86400) return `Hace ${Math.floor(diff / 3600)} h`
  return `Hace ${Math.floor(diff / 86400)} días`
}

function NotificationItem({
  notification,
  onNavigate,
  onMarkRead,
  onMarkUnread,
  onDelete,
}: {
  notification: LocalNotification
  onNavigate: (id: string) => void
  onMarkRead: (id: string) => void
  onMarkUnread: (id: string) => void
  onDelete: (id: string) => void
}) {
  const config = TYPE_CONFIG[notification.type]
  const Icon = config.icon
  const isUnread = !notification.read

  return (
    <div
      className={cn(
        'relative flex w-full items-start gap-3 px-3 py-2.5 text-left transition-all',
        isUnread ? 'border-l-2 border-primary bg-primary/5' : 'border-l-2 border-transparent',
      )}
    >
      {/* Area clickeable para navegar */}
      <button
        className="flex flex-1 items-start gap-3 transition-opacity hover:opacity-80"
        onClick={() => {
          if (isUnread) onMarkRead(notification.id)
          if (notification.ticketId) onNavigate(notification.ticketId)
        }}
      >
        {/* Icon */}
        <div
          className={cn('flex h-8 w-8 shrink-0 items-center justify-center rounded-lg', config.bg)}
        >
          <Icon className={cn('h-3.5 w-3.5', config.color)} />
        </div>

        {/* Content */}
        <div className="min-w-0 flex-1 space-y-0.5">
          <div className="flex items-start justify-between gap-2">
            <div className="flex min-w-0 flex-wrap items-center gap-1.5">
              <p
                className={cn(
                  'text-xs',
                  isUnread ? 'font-semibold text-foreground' : 'font-medium text-foreground/80',
                )}
              >
                {notification.title}
              </p>
              {notification.ticketCode && (
                <Badge variant="outline" className="shrink-0 text-[10px]">
                  {notification.ticketCode}
                </Badge>
              )}
            </div>
            <span className="shrink-0 whitespace-nowrap text-[11px] text-muted-foreground">
              {timeAgo(notification.createdAt)}
            </span>
          </div>
          <p
            className={cn(
              'text-left text-xs leading-relaxed',
              isUnread ? 'font-medium text-muted-foreground' : 'text-muted-foreground',
            )}
          >
            {notification.body}
          </p>
        </div>
      </button>

      {/* Acciones — menu contextual de tres puntos */}
      <div className="shrink-0 pt-0.5">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="icon" className="h-7 w-7 text-muted-foreground">
              <MoreHorizontal className="h-3.5 w-3.5" />
              <span className="sr-only">Opciones de notificación</span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-48">
            {isUnread ? (
              <DropdownMenuItem onClick={() => onMarkRead(notification.id)}>
                <MailOpen className="mr-2 h-3.5 w-3.5" />
                Marcar como leída
              </DropdownMenuItem>
            ) : (
              <DropdownMenuItem onClick={() => onMarkUnread(notification.id)}>
                <BellRing className="mr-2 h-3.5 w-3.5" />
                Marcar como no leída
              </DropdownMenuItem>
            )}
            {notification.ticketId && (
              <DropdownMenuItem onClick={() => onNavigate(notification.ticketId!)}>
                <Ticket className="mr-2 h-3.5 w-3.5" />
                Ver ticket
              </DropdownMenuItem>
            )}
            <DropdownMenuSeparator />
            <DropdownMenuItem
              className="text-destructive focus:text-destructive"
              onClick={() => onDelete(notification.id)}
            >
              <Trash2 className="mr-2 h-3.5 w-3.5" />
              Eliminar
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}

type Filter = 'all' | 'unread' | 'read'

export function NotificationsPage() {
  const navigate = useNavigate()
  const [filter, setFilter] = useState<Filter>('all')

  // Estado local para operaciones sin soporte en backend
  const [deletedIds, setDeletedIds] = useState<Set<string>>(new Set())
  const [unreadOverrides, setUnreadOverrides] = useState<Set<string>>(new Set())

  // Datos del backend
  const { data: queryData, isLoading: isLoadingNotifs } = useNotificaciones({ tamanoPagina: 50 })
  const { mutate: marcarLeida } = useMarcarLeida()
  const { mutate: marcarTodasLeidas } = useMarcarTodasLeidas()

  // Combinar datos del backend con estado local
  const notifications: LocalNotification[] = useMemo(() => {
    return (queryData?.items ?? [])
      .filter((dto) => !deletedIds.has(dto.id))
      .map((dto) => ({
        ...mapDtoToLocal(dto),
        // Aplicar override local de "no leída"
        read: unreadOverrides.has(dto.id) ? false : dto.esLeida,
      }))
  }, [queryData, deletedIds, unreadOverrides])

  const unreadCount = notifications.filter((n) => !n.read).length
  const readCount = notifications.length - unreadCount

  const filtered = notifications
    .filter((n) => {
      if (filter === 'unread') return !n.read
      if (filter === 'read') return n.read
      return true
    })
    .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())

  const handleNavigate = (ticketId: string) => {
    navigate(ticketDetailPath(ticketId))
  }

  const handleMarkRead = (id: string) => {
    marcarLeida(id)
    setUnreadOverrides((prev) => {
      const next = new Set(prev)
      next.delete(id)
      return next
    })
  }

  const handleMarkUnread = (id: string) => {
    // TODO: sin endpoint de "marcar como no leída" en el backend — solo local
    setUnreadOverrides((prev) => new Set(prev).add(id))
  }

  const handleDelete = (id: string) => {
    // TODO: sin endpoint de eliminación en el backend — solo local
    const snapshotDeleted = new Set(deletedIds)
    setDeletedIds((prev) => new Set(prev).add(id))

    toast('Notificación eliminada', {
      duration: 4000,
      action: {
        label: 'Deshacer',
        onClick: () => {
          setDeletedIds(snapshotDeleted)
        },
      },
    })
  }

  const handleMarkAllRead = () => {
    setUnreadOverrides(new Set())
    marcarTodasLeidas()
    // El toast de éxito lo maneja el mutation onSuccess
  }

  const handleDeleteAllRead = () => {
    const readIds = notifications.filter((n) => n.read).map((n) => n.id)
    const snapshot = new Set(deletedIds)
    setDeletedIds((prev) => new Set([...prev, ...readIds]))

    toast('Notificaciones leídas eliminadas', {
      duration: 5000,
      action: {
        label: 'Deshacer',
        onClick: () => {
          setDeletedIds(snapshot)
        },
      },
    })
  }

  if (isLoadingNotifs) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center p-4">
        <p className="text-sm text-muted-foreground">Cargando notificaciones...</p>
      </div>
    )
  }

  return (
    <div className="p-3 lg:p-4">
      {/* Layout: columna en mobile, 2 columnas en lg */}
      <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:gap-5">
        {/* === Columna principal: lista === */}
        <div className="flex-1 space-y-4">
          {/* Header */}
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-base font-semibold tracking-tight">Notificaciones</h2>
              {unreadCount > 0 && (
                <p className="text-xs text-muted-foreground">{unreadCount} sin leer</p>
              )}
            </div>
            {/* Acciones globales en header */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="h-8 w-8">
                  <MoreVertical className="h-4 w-4" />
                  <span className="sr-only">Opciones</span>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-52">
                <DropdownMenuItem disabled={unreadCount === 0} onClick={handleMarkAllRead}>
                  <CheckCheck className="mr-2 h-3.5 w-3.5" />
                  Marcar todas como leídas
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem
                  disabled={readCount === 0}
                  className="text-destructive focus:text-destructive"
                  onClick={handleDeleteAllRead}
                >
                  <Trash2 className="mr-2 h-3.5 w-3.5" />
                  Eliminar todas leídas
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>

          {/* Filter tabs */}
          <div className="flex gap-1 rounded-lg bg-muted p-1">
            {(
              [
                { id: 'all', label: 'Todas', count: notifications.length },
                { id: 'unread', label: 'Sin leer', count: unreadCount },
                { id: 'read', label: 'Leidas', count: readCount },
              ] as { id: Filter; label: string; count: number }[]
            ).map((tab) => (
              <button
                key={tab.id}
                onClick={() => setFilter(tab.id)}
                className={cn(
                  'flex flex-1 items-center justify-center gap-1.5 rounded-md px-2.5 py-1 text-xs font-medium transition-all',
                  filter === tab.id
                    ? 'bg-background text-foreground shadow-sm'
                    : 'text-muted-foreground hover:text-foreground',
                )}
              >
                {tab.label}
                <Badge
                  variant={filter === tab.id ? 'default' : 'secondary'}
                  className="h-4 min-w-4 px-1 text-[10px]"
                >
                  {tab.count}
                </Badge>
              </button>
            ))}
          </div>

          {/* Lista de notificaciones */}
          {notifications.length === 0 ? (
            <EmptyState
              icon={Bell}
              title="Todo al día"
              description="No tienes notificaciones nuevas por el momento."
            />
          ) : filtered.length === 0 ? (
            <EmptyState
              icon={BellOff}
              title="Sin notificaciones"
              description={
                filter === 'unread'
                  ? 'No tienes notificaciones sin leer.'
                  : 'No hay notificaciones en esta categoria.'
              }
            />
          ) : (
            <Card>
              <CardContent className="divide-y p-0">
                {filtered.map((notification) => (
                  <NotificationItem
                    key={notification.id}
                    notification={notification}
                    onNavigate={handleNavigate}
                    onMarkRead={handleMarkRead}
                    onMarkUnread={handleMarkUnread}
                    onDelete={handleDelete}
                  />
                ))}
              </CardContent>
            </Card>
          )}
        </div>

        {/* === Columna derecha: panel de resumen (solo en lg) === */}
        <div className="hidden w-64 shrink-0 lg:block">
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <h3 className="text-xs font-medium text-muted-foreground">Resumen</h3>
            </CardHeader>
            <CardContent className="p-3 pt-0">
              <div className="space-y-3">
                {/* Estadisticas */}
                <div className="space-y-2">
                  <div className="flex items-center justify-between rounded-md bg-primary/5 px-3 py-2">
                    <div className="flex items-center gap-2">
                      <div className="h-2 w-2 rounded-full bg-primary" />
                      <span className="text-xs font-medium">Sin leer</span>
                    </div>
                    <span className="text-sm font-semibold text-primary">{unreadCount}</span>
                  </div>

                  <div className="flex items-center justify-between rounded-md bg-muted px-3 py-2">
                    <div className="flex items-center gap-2">
                      <MailOpen className="h-3.5 w-3.5 text-muted-foreground" />
                      <span className="text-xs font-medium text-muted-foreground">Leidas</span>
                    </div>
                    <span className="text-sm font-semibold">{readCount}</span>
                  </div>

                  <div className="flex items-center justify-between rounded-md bg-muted/50 px-3 py-2">
                    <div className="flex items-center gap-2">
                      <Bell className="h-3.5 w-3.5 text-muted-foreground" />
                      <span className="text-xs font-medium text-muted-foreground">Total</span>
                    </div>
                    <span className="text-sm font-semibold">{notifications.length}</span>
                  </div>
                </div>

                {/* Acciones globales en panel lateral */}
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="outline" size="sm" className="w-full">
                      <MoreHorizontal className="mr-2 h-3.5 w-3.5" />
                      Acciones
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end" className="w-52">
                    <DropdownMenuItem disabled={unreadCount === 0} onClick={handleMarkAllRead}>
                      <CheckCheck className="mr-2 h-3.5 w-3.5" />
                      Marcar todas como leídas
                    </DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem
                      disabled={readCount === 0}
                      className="text-destructive focus:text-destructive"
                      onClick={handleDeleteAllRead}
                    >
                      <Trash2 className="mr-2 h-3.5 w-3.5" />
                      Eliminar todas leídas
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
