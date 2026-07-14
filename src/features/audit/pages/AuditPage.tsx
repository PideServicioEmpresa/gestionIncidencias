import { Shield, Search, Download, User, Ticket, Settings, LogIn, SearchX } from 'lucide-react'
import { useState, useMemo, useEffect } from 'react'
import { toast } from 'sonner'
import { useQuery } from '@tanstack/react-query'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { Badge } from '@shared/ui/badge'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { cn } from '@lib/utils'
import { EmptyState } from '@shared/components/EmptyState'
import { auditoriaService, type AuditLogDto } from '../services/auditoriaService'

// ── Tipos locales ─────────────────────────────────────────────────────────────

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

// ── Mapeo de tabla a tipo de acción ──────────────────────────────────────────

function resolveType(tabla: string, accion: string): AuditAction {
  const t = tabla.toLowerCase()
  const a = accion.toLowerCase()
  if (t.includes('sesion') || t.includes('auth') || a.includes('login') || a.includes('sesion'))
    return 'login'
  if (t.includes('ticket')) return 'ticket'
  if (t.includes('usuario') || t.includes('user')) return 'user'
  if (t.includes('configuracion') || t.includes('config') || t.includes('parametro'))
    return 'config'
  return 'ticket'
}

function mapToEntry(dto: AuditLogDto): AuditEntry {
  const type = resolveType(dto.tabla, dto.accion)
  const details = [dto.valoresNuevos, dto.valoresAnteriores].find((v) => v) ?? dto.tabla
  return {
    id: dto.id,
    action: dto.accion,
    type,
    user: dto.usuarioId ? dto.usuarioId.slice(0, 8) + '...' : 'Sistema',
    details,
    ip: dto.ipAddress ?? '—',
    timestamp: dto.createdAt,
    result: 'success',
  }
}

// ── Mapping de filtro UI → tabla backend ─────────────────────────────────────

const TYPE_TO_TABLA: Record<AuditAction, string> = {
  login: 'sesiones',
  ticket: 'tickets',
  user: 'usuarios',
  config: 'configuracion',
}

// ── Configuración visual por tipo ─────────────────────────────────────────────

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

  // Parámetros para el backend
  const tablaParam = typeFilter !== 'all' ? TYPE_TO_TABLA[typeFilter] : undefined

  const {
    data: queryData,
    isLoading,
    error: auditoriaError,
  } = useQuery({
    queryKey: ['auditoria', tablaParam],
    queryFn: () => auditoriaService.listar({ tabla: tablaParam, tamanoPagina: 50 }),
    retry: false,
  })

  useEffect(() => {
    if (auditoriaError) toast.error(auditoriaError.message)
  }, [auditoriaError])

  // Mapear DTOs a la shape local y aplicar búsqueda de texto
  const entries: AuditEntry[] = useMemo(() => {
    return (queryData?.items ?? []).map(mapToEntry)
  }, [queryData])

  const filtered = useMemo(() => {
    if (!search) return entries
    const q = search.toLowerCase()
    return entries.filter(
      (e) =>
        e.action.toLowerCase().includes(q) ||
        e.user.toLowerCase().includes(q) ||
        e.details.toLowerCase().includes(q),
    )
  }, [entries, search])

  // Estadísticas de resumen basadas en datos cargados
  const totalEventos = queryData?.totalRegistros ?? entries.length
  const ingresos = entries.filter((e) => e.type === 'login').length
  const tickets = entries.filter((e) => e.type === 'ticket').length
  const errores = entries.filter((e) => e.result === 'error').length

  const handleExport = () => {
    const promise = new Promise<void>((resolve) => {
      setTimeout(() => resolve(), 2000)
    })
    toast.promise(promise, {
      loading: 'Generando export...',
      success: 'Auditoria exportada como audit-log.xlsx',
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
          { label: 'Total eventos', value: totalEventos },
          { label: 'Ingresos', value: ingresos },
          { label: 'Tickets', value: tickets },
          { label: 'Errores', value: errores },
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
          <CardDescription>
            {isLoading
              ? 'Cargando...'
              : `${filtered.length} evento${filtered.length !== 1 ? 's' : ''} mostrado${filtered.length !== 1 ? 's' : ''}`}
          </CardDescription>
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
          {!isLoading && filtered.length === 0 && (
            <EmptyState
              icon={SearchX}
              title="Sin resultados"
              description="No hay registros de auditoría que coincidan con la búsqueda."
              size="sm"
            />
          )}
        </CardContent>
      </Card>
    </div>
  )
}
