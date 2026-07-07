import { Shield, Search, Download, User, Ticket, Settings, LogIn } from 'lucide-react'
import { useState } from 'react'
import { toast } from 'sonner'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { Badge } from '@shared/ui/badge'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { cn } from '@lib/utils'

type AuditAction = 'login' | 'ticket' | 'user' | 'config'

interface AuditEntry {
  id: string
  action: string
  type: AuditAction
  user: string
  details: string
  ip: string
  timestamp: string
  result: 'success' | 'error'
}

const AUDIT_ENTRIES: AuditEntry[] = [
  {
    id: 'a1',
    action: 'Inicio de sesión',
    type: 'login',
    user: 'jperez',
    details: 'Acceso desde Chrome 130 · Windows 11',
    ip: '192.168.1.105',
    timestamp: '2026-06-29T13:00:00',
    result: 'success',
  },
  {
    id: 'a2',
    action: 'Ticket asignado',
    type: 'ticket',
    user: 'jperez',
    details: 'PS-0009 asignado a María López',
    ip: '192.168.1.105',
    timestamp: '2026-06-29T12:55:00',
    result: 'success',
  },
  {
    id: 'a3',
    action: 'Estado de ticket actualizado',
    type: 'ticket',
    user: 'mlopez',
    details: 'PS-0001 cambiado a En Proceso',
    ip: '192.168.1.112',
    timestamp: '2026-06-29T10:30:00',
    result: 'success',
  },
  {
    id: 'a4',
    action: 'Inicio de sesión fallido',
    type: 'login',
    user: 'unknown@empresa.com',
    details: 'Credenciales incorrectas · 3er intento',
    ip: '192.168.1.88',
    timestamp: '2026-06-29T10:00:00',
    result: 'error',
  },
  {
    id: 'a5',
    action: 'Usuario desactivado',
    type: 'user',
    user: 'gcbarrionuevo',
    details: 'Luisa Torres (ltorres) desactivada',
    ip: '192.168.1.100',
    timestamp: '2026-06-28T17:00:00',
    result: 'success',
  },
  {
    id: 'a6',
    action: 'Ticket creado',
    type: 'ticket',
    user: 'aramirez',
    details: 'PS-0013: Escáner no detectado por Windows',
    ip: '192.168.1.130',
    timestamp: '2026-06-29T10:00:00',
    result: 'success',
  },
  {
    id: 'a7',
    action: 'Configuración de sistema modificada',
    type: 'config',
    user: 'gcbarrionuevo',
    details: 'Tiempo de sesión cambiado de 60 a 120 min',
    ip: '192.168.1.100',
    timestamp: '2026-06-28T11:00:00',
    result: 'success',
  },
  {
    id: 'a8',
    action: 'Ticket reabierto',
    type: 'ticket',
    user: 'cgarcia',
    details: 'PS-0008 reabierto por recurrencia del problema',
    ip: '192.168.1.221',
    timestamp: '2026-06-29T07:00:00',
    result: 'success',
  },
  {
    id: 'a9',
    action: 'Inicio de sesión',
    type: 'login',
    user: 'mlopez',
    details: 'Acceso desde Safari · iPhone 15',
    ip: '192.168.1.201',
    timestamp: '2026-06-29T08:00:00',
    result: 'success',
  },
  {
    id: 'a10',
    action: 'Rol de usuario modificado',
    type: 'user',
    user: 'gcbarrionuevo',
    details: 'Pedro Flores promovido de Usuario a Trabajador',
    ip: '192.168.1.100',
    timestamp: '2026-06-27T14:00:00',
    result: 'success',
  },
]

const TYPE_CONFIG: Record<AuditAction, { icon: typeof Shield; color: string; bg: string }> = {
  login: {
    icon: LogIn,
    color: 'text-blue-600 dark:text-blue-400',
    bg: 'bg-blue-100 dark:bg-blue-900/30',
  },
  ticket: { icon: Ticket, color: 'text-primary', bg: 'bg-primary/10' },
  user: {
    icon: User,
    color: 'text-purple-600 dark:text-purple-400',
    bg: 'bg-purple-100 dark:bg-purple-900/30',
  },
  config: {
    icon: Settings,
    color: 'text-orange-600 dark:text-orange-400',
    bg: 'bg-orange-100 dark:bg-orange-900/30',
  },
}

function formatTimestamp(ts: string) {
  return new Date(ts).toLocaleString('es-PE', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

export function AuditPage() {
  const [search, setSearch] = useState('')
  const [typeFilter, setTypeFilter] = useState<AuditAction | 'all'>('all')

  const filtered = AUDIT_ENTRIES.filter((e) => {
    const matchSearch =
      !search ||
      e.action.toLowerCase().includes(search.toLowerCase()) ||
      e.user.toLowerCase().includes(search.toLowerCase()) ||
      e.details.toLowerCase().includes(search.toLowerCase())
    const matchType = typeFilter === 'all' || e.type === typeFilter
    return matchSearch && matchType
  })

  const handleExport = () => {
    const promise = new Promise<void>((resolve) => {
      setTimeout(() => resolve(), 2000)
    })
    toast.promise(promise, {
      loading: 'Generando export...',
      success: 'Auditoría exportada como audit-log.xlsx',
      error: 'Error al exportar',
    })
  }

  return (
    <div className="space-y-4 p-3 lg:p-4">
      {/* Header */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="flex items-center gap-2 text-base font-semibold tracking-tight">
            <Shield className="h-5 w-5 text-primary" />
            Auditoría
          </h2>
          <p className="text-xs text-muted-foreground">
            Registro completo de acciones del sistema.
          </p>
        </div>
        <Button variant="outline" size="sm" onClick={handleExport}>
          <Download className="mr-2 h-4 w-4" />
          Exportar log
        </Button>
      </div>

      {/* Summary */}
      <div className="grid grid-cols-2 gap-2 sm:grid-cols-4">
        {[
          { label: 'Total eventos', value: AUDIT_ENTRIES.length },
          { label: 'Ingresos', value: AUDIT_ENTRIES.filter((e) => e.type === 'login').length },
          { label: 'Tickets', value: AUDIT_ENTRIES.filter((e) => e.type === 'ticket').length },
          { label: 'Errores', value: AUDIT_ENTRIES.filter((e) => e.result === 'error').length },
        ].map((stat) => (
          <Card key={stat.label}>
            <CardContent className="p-3">
              <p className="text-xs font-medium text-muted-foreground">{stat.label}</p>
              <p className="text-2xl font-bold">{stat.value}</p>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Search + type filter — 70/30 */}
      <div className="flex gap-2">
        <div className="relative flex-[7]">
          <Search className="absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Buscar por acción, usuario o detalle..."
            className="h-8 pl-9 text-xs"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <div className="flex-[3]">
          <Select value={typeFilter} onValueChange={(v) => setTypeFilter(v as AuditAction | 'all')}>
            <SelectTrigger className="h-8 w-full text-xs">
              <SelectValue placeholder="Todos los tipos" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Todos los tipos</SelectItem>
              <SelectItem value="login">Acceso</SelectItem>
              <SelectItem value="ticket">Ticket</SelectItem>
              <SelectItem value="user">Usuario</SelectItem>
              <SelectItem value="config">Configuración</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      {/* Log entries */}
      <Card>
        <CardHeader className="px-3 pb-2 pt-3">
          <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            Log de eventos
          </CardTitle>
          <CardDescription>Últimas 10 acciones del sistema</CardDescription>
        </CardHeader>
        <CardContent className="divide-y p-0">
          {filtered.map((entry) => {
            const config = TYPE_CONFIG[entry.type]
            const Icon = config.icon
            return (
              <div key={entry.id} className="flex items-start gap-3 px-4 py-2.5">
                <div
                  className={cn(
                    'mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg',
                    config.bg,
                  )}
                >
                  <Icon className={cn('h-3.5 w-3.5', config.color)} />
                </div>
                <div className="min-w-0 flex-1">
                  <div className="flex flex-wrap items-center gap-2">
                    <span className="text-xs font-medium">{entry.action}</span>
                    <Badge
                      variant="outline"
                      className={cn(
                        'text-[10px]',
                        entry.result === 'error' && 'border-destructive text-destructive',
                      )}
                    >
                      {entry.result === 'success' ? 'OK' : 'Error'}
                    </Badge>
                  </div>
                  <p className="mt-0.5 text-xs text-muted-foreground">{entry.details}</p>
                  <div className="mt-0.5 flex flex-wrap gap-x-3 text-[11px] text-muted-foreground">
                    <span className="font-medium">@{entry.user}</span>
                    <span>{entry.ip}</span>
                    <span>{formatTimestamp(entry.timestamp)}</span>
                  </div>
                </div>
              </div>
            )
          })}
        </CardContent>
      </Card>
    </div>
  )
}
