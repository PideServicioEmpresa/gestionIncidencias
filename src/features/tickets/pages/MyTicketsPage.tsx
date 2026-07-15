import { useState, useMemo, useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import {
  Search,
  Plus,
  Filter,
  ChevronLeft,
  ChevronRight,
  ChevronDown,
  X,
  UserCheck,
  MoreVertical,
  Pencil,
  Eye,
  History,
  CalendarDays,
} from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent } from '@shared/ui/card'
import { Badge } from '@shared/ui/badge'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { Textarea } from '@shared/ui/textarea'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@shared/ui/dialog'
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetDescription } from '@shared/ui/sheet'
import { cn } from '@lib/utils'
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuLabel,
} from '@shared/ui/dropdown-menu'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@shared/ui/table'
import { StatusBadge } from '@shared/components/StatusBadge'
import { PriorityBadge } from '@shared/components/PriorityBadge'
import { FormField } from '@shared/components/FormField'
import { EmptyState } from '@shared/components/EmptyState'
import { TicketListSkeleton } from '@shared/components/PageSkeletons'
import { useAuthStore } from '@store/auth.store'
import { ROUTES, ticketDetailPath } from '@constants/index'
import { useTiposServicio, useSucursales, useAreas, useTecnicos } from '../hooks/useCatalogos'
import type { TicketStatus, TicketPriority } from '@types-app/index'
import {
  useTickets,
  useAsignarTicket,
  useCambiarPrioridad,
  useCambiarArea,
  useTicketHistorial,
} from '../hooks/useTickets'
import type { TicketListItemDto, TicketListParams } from '../services/ticketService'

// ── Normalizadores de valores backend → frontend ──────────────────────────────

function normalizeEstado(estado: string): TicketStatus {
  return estado.toLowerCase() as TicketStatus
}

function normalizePrioridad(prioridad: string): TicketPriority {
  return prioridad.toLowerCase() as TicketPriority
}

// ── EditTicketSheet ───────────────────────────────────────────────────────────

interface EditTicketSaveParams {
  ticketId: string
  nuevaAreaId?: string
  nuevaPrioridad?: string
}

interface EditTicketSheetProps {
  ticket: TicketListItemDto | null
  onClose: () => void
  onSave: (params: EditTicketSaveParams) => void
}

const PRIORITY_OPTIONS: {
  value: TicketPriority
  label: string
  dot: string
  selectedClass: string
}[] = [
  {
    value: 'baja',
    label: 'Baja',
    dot: 'bg-blue-400',
    selectedClass: 'border-blue-500 bg-blue-500/10 text-blue-400',
  },
  {
    value: 'media',
    label: 'Media',
    dot: 'bg-yellow-400',
    selectedClass: 'border-yellow-500 bg-yellow-500/10 text-yellow-400',
  },
  {
    value: 'alta',
    label: 'Alta',
    dot: 'bg-orange-400',
    selectedClass: 'border-orange-500 bg-orange-500/10 text-orange-400',
  },
  {
    value: 'critica',
    label: 'Crítica',
    dot: 'bg-red-400',
    selectedClass: 'border-red-500 bg-red-500/10 text-red-400',
  },
]

function EditTicketSheet({ ticket, onClose, onSave }: EditTicketSheetProps) {
  const user = useAuthStore((s) => s.user)
  const tiposServicioQuery = useTiposServicio(user?.empresaId)
  const tiposServicio = tiposServicioQuery.data ?? []
  const sucursalesQuery = useSucursales(user?.empresaId)
  const sucursales = (sucursalesQuery.data ?? []).filter((s) => s.activa)
  const areasQuery = useAreas(user?.empresaId)
  const areas = (areasQuery.data ?? []).filter((a) => a.activa)

  const initialPriority = ticket
    ? normalizePrioridad(ticket.prioridadEfectiva)
    : ('media' as TicketPriority)

  const [form, setForm] = useState({
    type: ticket?.tipo ?? '',
    title: ticket?.titulo ?? '',
    sucursalId: ticket?.sucursalId ?? '',
    areaId: ticket?.areaId ?? '',
    priority: initialPriority,
    description: '',
  })
  const [errors, setErrors] = useState<Partial<Record<string, string>>>({})

  // Areas shown in the select — no client-side sucursal filter since
  // AreaResumenDto does not expose sucursalId
  const areasFiltered = useMemo(() => areas, [areas])

  useEffect(() => {
    if (ticket) {
      setForm({
        type: ticket.tipo ?? '',
        title: ticket.titulo,
        sucursalId: ticket.sucursalId,
        areaId: ticket.areaId,
        priority: normalizePrioridad(ticket.prioridadEfectiva),
        description: '',
      })
      setErrors({})
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [ticket?.id])

  function handleField(field: string, value: string) {
    setForm((prev) => ({
      ...prev,
      [field]: value,
      ...(field === 'sucursalId' ? { areaId: '' } : {}),
    }))
    if (errors[field]) setErrors((prev) => ({ ...prev, [field]: undefined }))
  }

  function validate() {
    const next: Partial<Record<string, string>> = {}
    if (!form.sucursalId) next.sucursalId = 'Selecciona una empresa.'
    if (!form.areaId) next.areaId = 'Selecciona una sucursal.'
    return Object.keys(next).length === 0 ? null : next
  }

  function handleSave() {
    const errs = validate()
    if (errs) {
      setErrors(errs)
      return
    }
    if (!ticket) return

    const params: EditTicketSaveParams = { ticketId: ticket.id }
    if (form.areaId !== ticket.areaId) params.nuevaAreaId = form.areaId
    if (form.priority !== normalizePrioridad(ticket.prioridadEfectiva))
      params.nuevaPrioridad = form.priority.toUpperCase()

    onSave(params)
    onClose()
  }

  return (
    <Sheet open={!!ticket} onOpenChange={(open) => !open && onClose()}>
      <SheetContent side="right" className="flex w-full flex-col gap-0 p-0 sm:max-w-lg">
        {/* Header */}
        <SheetHeader className="border-b px-5 pb-4 pr-12 pt-5">
          <SheetTitle className="text-base font-semibold">Modificar ticket</SheetTitle>
          {ticket && (
            <p className="flex flex-wrap items-center gap-1.5 text-xs text-muted-foreground">
              <span className="rounded bg-muted px-1.5 py-0.5 font-mono text-[10px]">
                {ticket.codigo}
              </span>
              <span>{ticket.tipo}</span>
            </p>
          )}
        </SheetHeader>

        {/* Scrollable body */}
        <div className="flex-1 overflow-y-auto">
          {/* Sección primaria */}
          <div className="space-y-3 border-b bg-primary/5 px-5 py-4">
            <p className="text-[10px] font-semibold uppercase tracking-widest text-primary/70">
              Información principal
            </p>

            <FormField label="Tipo de servicio">
              <Select value={form.type} disabled>
                <SelectTrigger className="h-9 text-sm">
                  <SelectValue placeholder="—" />
                </SelectTrigger>
                <SelectContent>
                  {tiposServicio.map((t) => (
                    <SelectItem key={t.id} value={t.id}>
                      {t.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FormField>

            <FormField label="Título">
              <Input
                className="h-9 text-sm"
                value={form.title}
                readOnly
                disabled
                placeholder="Título del ticket"
              />
            </FormField>

            <div className="grid grid-cols-2 gap-3">
              <FormField label="Empresa" required error={errors.sucursalId}>
                <Select
                  value={form.sucursalId}
                  onValueChange={(v) => handleField('sucursalId', v)}
                  disabled={sucursalesQuery.isLoading}
                >
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue
                      placeholder={sucursalesQuery.isLoading ? 'Cargando...' : 'Selecciona'}
                    />
                  </SelectTrigger>
                  <SelectContent>
                    {sucursales.map((s) => (
                      <SelectItem key={s.id} value={s.id}>
                        {s.nombre}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </FormField>

              <FormField label="Sucursal" required error={errors.areaId}>
                <Select
                  value={form.areaId}
                  onValueChange={(v) => handleField('areaId', v)}
                  disabled={!form.sucursalId || areasQuery.isLoading}
                >
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue
                      placeholder={
                        !form.sucursalId
                          ? 'Primero empresa'
                          : areasQuery.isLoading
                            ? 'Cargando...'
                            : 'Selecciona'
                      }
                    />
                  </SelectTrigger>
                  <SelectContent>
                    {areasFiltered.map((a) => (
                      <SelectItem key={a.id} value={a.id}>
                        {a.nombre}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </FormField>
            </div>

            {/* Prioridad — botones visuales */}
            <FormField label="Prioridad" required>
              <div className="grid grid-cols-4 gap-2">
                {PRIORITY_OPTIONS.map((opt) => (
                  <button
                    key={opt.value}
                    type="button"
                    onClick={() => handleField('priority', opt.value)}
                    className={cn(
                      'flex flex-col items-center gap-1.5 rounded-lg border-2 px-2 py-2.5 text-xs font-semibold transition-all',
                      form.priority === opt.value
                        ? opt.selectedClass
                        : 'border-border bg-muted/30 text-muted-foreground hover:bg-muted/60',
                    )}
                  >
                    <span className={cn('h-2 w-2 rounded-full', opt.dot)} />
                    {opt.label}
                  </button>
                ))}
              </div>
            </FormField>
          </div>

          {/* Sección secundaria */}
          <div className="space-y-3 px-5 py-4">
            <p className="text-[10px] font-semibold uppercase tracking-widest text-muted-foreground">
              Campos adicionales
            </p>

            <FormField label="Descripción">
              <Textarea
                className="text-sm"
                rows={3}
                value={form.description}
                onChange={(e) => handleField('description', e.target.value)}
                placeholder="Describe el problema o solicitud"
              />
            </FormField>
          </div>
        </div>

        {/* Footer fijo */}
        <div className="border-t bg-background px-5 py-3">
          <div className="flex justify-end gap-2">
            <Button variant="outline" size="sm" onClick={onClose}>
              Cancelar
            </Button>
            <Button size="sm" onClick={handleSave}>
              Guardar cambios
            </Button>
          </div>
        </div>
      </SheetContent>
    </Sheet>
  )
}

// ── MyTicketsPage ─────────────────────────────────────────────────────────────

export function MyTicketsPage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const user = useAuthStore((s) => s.user)
  const isAdmin = user?.rol === 'admin' || user?.rol === 'superadmin'

  // Filtros de UI (estado local)
  const [search, setSearch] = useState('')
  // debouncedSearch se actualiza 300 ms después de que el usuario deja de escribir,
  // evitando una request por cada tecla en el input de búsqueda.
  const [debouncedSearch, setDebouncedSearch] = useState('')
  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(search), 300)
    return () => clearTimeout(timer)
  }, [search])
  const [statusFilter, setStatusFilter] = useState<TicketStatus | 'all'>(() => {
    const s = searchParams.get('status')
    return (s as TicketStatus) ?? 'all'
  })
  const [priorityFilter, setPriorityFilter] = useState<TicketPriority | 'all'>(() => {
    const p = searchParams.get('priority')
    return (p as TicketPriority) ?? 'all'
  })
  const [dateFrom, setDateFrom] = useState('')
  const [dateTo, setDateTo] = useState('')
  const [page, setPage] = useState(1)

  // Parámetros para el servidor — se reconstruyen al cambiar filtros.
  // Usa debouncedSearch (no search) para no lanzar un request por cada tecla.
  const queryParams = useMemo<TicketListParams>(
    () => ({
      pagina: page,
      tamanoPagina: 20,
      ...(debouncedSearch ? { busqueda: debouncedSearch } : {}),
      ...(statusFilter !== 'all' ? { estado: statusFilter.toUpperCase() } : {}),
      ...(priorityFilter !== 'all' ? { prioridad: priorityFilter.toUpperCase() } : {}),
      ...(dateFrom ? { fechaDesde: dateFrom } : {}),
      ...(dateTo ? { fechaHasta: dateTo } : {}),
    }),
    [page, debouncedSearch, statusFilter, priorityFilter, dateFrom, dateTo],
  )

  const { data, isLoading, error } = useTickets(queryParams)
  const tickets = data?.items ?? []
  const totalRegistros = data?.totalRegistros ?? 0
  const totalPages = data?.totalPaginas ?? 1

  // Mutations
  const asignarTicket = useAsignarTicket()
  const cambiarPrioridad = useCambiarPrioridad()
  const cambiarArea = useCambiarArea()

  // Modal: asignar responsable
  const [assignTicketId, setAssignTicketId] = useState<string | null>(null)
  const [selectedAssignWorker, setSelectedAssignWorker] = useState('')

  // Sheet: modificar ticket
  const [editTicket, setEditTicket] = useState<TicketListItemDto | null>(null)

  // Dialog: historial
  const [historyTicketId, setHistoryTicketId] = useState<string | null>(null)
  const historialQuery = useTicketHistorial(historyTicketId ?? '')

  // Técnicos para asignación — datos reales del backend
  const tecnicosQuery = useTecnicos(user?.empresaId)
  const workers = tecnicosQuery.data ?? []

  const handleFilterChange = () => setPage(1)

  const clearFilters = () => {
    setSearch('')
    setStatusFilter('all')
    setPriorityFilter('all')
    setDateFrom('')
    setDateTo('')
    setPage(1)
  }

  const handleAssign = () => {
    if (!selectedAssignWorker || !assignTicketId) return
    asignarTicket.mutate(
      { ticketId: assignTicketId, tecnicoId: selectedAssignWorker },
      {
        onSuccess: () => {
          setSelectedAssignWorker('')
          setAssignTicketId(null)
        },
        onError: (err: Error) => {
          toast.error(err.message ?? 'No se pudo asignar el ticket. Verifica el estado del ticket.')
        },
      },
    )
  }

  const handleSaveTicket = ({ ticketId, nuevaAreaId, nuevaPrioridad }: EditTicketSaveParams) => {
    if (!nuevaAreaId && !nuevaPrioridad) {
      toast.info('Sin cambios para guardar.')
      return
    }
    if (nuevaAreaId) cambiarArea.mutate({ ticketId, nuevaAreaId })
    if (nuevaPrioridad) cambiarPrioridad.mutate({ ticketId, nuevaPrioridad })
  }

  const assigningTicket = assignTicketId ? tickets.find((t) => t.id === assignTicketId) : null

  useEffect(() => {
    if (error) {
      toast.error((error as Error).message)
    }
  }, [error])

  if (isLoading) return <TicketListSkeleton />

  return (
    <div className="space-y-3 p-3 lg:p-4">
      {/* Page header */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-base font-semibold tracking-tight">
            {isAdmin ? 'Gestión de tickets' : 'Mis tickets'}
          </h2>
          <p role="status" aria-live="polite" className="text-xs text-muted-foreground">
            {totalRegistros} ticket{totalRegistros !== 1 ? 's' : ''} encontrado
            {totalRegistros !== 1 ? 's' : ''}
          </p>
        </div>
        <Button onClick={() => navigate(ROUTES.TICKETS_NEW)}>
          <Plus className="mr-2 h-4 w-4" />
          Nuevo ticket
        </Button>
      </div>

      {/* Banner: filtro activo desde el dashboard */}
      {(searchParams.get('status') || searchParams.get('priority')) && (
        <div className="flex items-center gap-2 rounded-lg bg-primary/10 px-3 py-1.5 text-xs text-primary">
          <Filter className="h-3 w-3" />
          Filtrado desde el dashboard
          <Button
            variant="ghost"
            size="sm"
            className="ml-auto h-6 px-2 text-xs text-primary/70 hover:text-primary"
            onClick={() => {
              setStatusFilter('all')
              setPriorityFilter('all')
            }}
          >
            Limpiar
          </Button>
        </div>
      )}

      {/* Filters — fila única en desktop, 2 columnas en mobile */}
      <div className="flex flex-col gap-2 lg:flex-row lg:flex-wrap lg:items-end">
        {/* Search — full width en mobile, flex-1 en desktop */}
        <div className="relative lg:min-w-[180px] lg:flex-1">
          <Search className="absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Buscar por título, código o sucursal..."
            className="h-8 w-full pl-9 text-xs"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value)
              handleFilterChange()
            }}
          />
        </div>

        {/* Filtros: grid 2 columnas en mobile, inline en desktop vía contents */}
        <div className="grid grid-cols-2 gap-2 lg:contents">
          {/* Estado */}
          <div className="flex flex-col gap-1">
            <span className="text-[10px] font-medium text-muted-foreground">Estado</span>
            <Select
              value={statusFilter}
              onValueChange={(v) => {
                setStatusFilter(v as TicketStatus | 'all')
                handleFilterChange()
              }}
            >
              <SelectTrigger className="h-8 w-full text-xs lg:w-36">
                <SelectValue placeholder="Estado" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todos los estados</SelectItem>
                <SelectItem value="sin_asignar">Sin asignar</SelectItem>
                <SelectItem value="asignado">Asignado</SelectItem>
                <SelectItem value="en_proceso">En proceso</SelectItem>
                <SelectItem value="en_espera">En espera</SelectItem>
                <SelectItem value="pendiente_validacion">Pend. validación</SelectItem>
                <SelectItem value="cerrado">Cerrado</SelectItem>
                <SelectItem value="reabierto">Reabierto</SelectItem>
                <SelectItem value="cancelado">Cancelado</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Prioridad */}
          <div className="flex flex-col gap-1">
            <span className="text-[10px] font-medium text-muted-foreground">Prioridad</span>
            <Select
              value={priorityFilter}
              onValueChange={(v) => {
                setPriorityFilter(v as TicketPriority | 'all')
                handleFilterChange()
              }}
            >
              <SelectTrigger className="h-8 w-full text-xs lg:w-32">
                <SelectValue placeholder="Prioridad" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Todas las prioridades</SelectItem>
                <SelectItem value="critica">Crítica</SelectItem>
                <SelectItem value="alta">Alta</SelectItem>
                <SelectItem value="media">Media</SelectItem>
                <SelectItem value="baja">Baja</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Desde */}
          <div className="flex flex-col gap-1">
            <span className="text-[10px] font-medium text-muted-foreground">Desde</span>
            <div className="relative">
              <CalendarDays className="pointer-events-none absolute left-2 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
              <Input
                type="date"
                className="h-10 w-full pl-7 text-xs lg:h-8 lg:w-[148px]"
                value={dateFrom}
                onChange={(e) => {
                  setDateFrom(e.target.value)
                  handleFilterChange()
                }}
              />
            </div>
          </div>

          {/* Hasta */}
          <div className="flex flex-col gap-1">
            <span className="text-[10px] font-medium text-muted-foreground">Hasta</span>
            <div className="relative">
              <CalendarDays className="pointer-events-none absolute left-2 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
              <Input
                type="date"
                className="h-10 w-full pl-7 text-xs lg:h-8 lg:w-[148px]"
                value={dateTo}
                onChange={(e) => {
                  setDateTo(e.target.value)
                  handleFilterChange()
                }}
              />
            </div>
          </div>
        </div>

        {/* Limpiar */}
        <Button
          size="sm"
          className="h-8 w-full text-xs lg:w-auto lg:shrink-0"
          onClick={clearFilters}
        >
          <X className="mr-1 h-3 w-3" />
          Limpiar
        </Button>
      </div>

      {/* Content */}
      {tickets.length === 0 ? (
        <EmptyState
          icon={Search}
          title="Sin resultados"
          description="No encontramos tickets con los filtros seleccionados."
          action={
            <Button variant="outline" onClick={clearFilters}>
              Limpiar filtros
            </Button>
          }
        />
      ) : (
        <>
          {/* ── MOBILE: card list ─────────────────────────────────────────── */}
          <div className="grid gap-2 lg:hidden">
            {tickets.map((ticket) => (
              <Card
                key={ticket.id}
                className="cursor-pointer transition-all hover:border-primary/30 hover:shadow-md"
                onClick={() => navigate(ticketDetailPath(ticket.id))}
              >
                <CardContent className="p-3">
                  <div className="flex items-start gap-3">
                    <div className="min-w-0 flex-1">
                      <div className="flex flex-wrap items-center gap-2">
                        <span className="font-mono text-xs text-muted-foreground">
                          {ticket.codigo}
                        </span>
                        <PriorityBadge
                          priority={normalizePrioridad(ticket.prioridadEfectiva)}
                          showIcon
                        />
                      </div>
                      <p className="mt-1 text-xs font-medium leading-snug">{ticket.titulo}</p>
                      <p className="mt-0.5 text-xs text-muted-foreground">
                        {ticket.sucursalNombre} · {ticket.areaNombre}
                      </p>
                      <p className="mt-0.5 text-xs text-muted-foreground">{ticket.tipo}</p>
                      {ticket.asignadoANombre && (
                        <div className="mt-0.5 space-y-0.5">
                          <p className="text-xs text-muted-foreground">
                            <span className="font-medium text-foreground/70">Asignado a:</span>{' '}
                            {ticket.asignadoANombre}
                          </p>
                        </div>
                      )}
                    </div>
                    <div className="flex flex-col items-end gap-1.5">
                      <div className="flex items-center gap-1.5">
                        <StatusBadge status={normalizeEstado(ticket.estado)} />
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-7 w-7 shrink-0"
                              onClick={(e) => e.stopPropagation()}
                              aria-label={`Acciones para ticket ${ticket.codigo}`}
                            >
                              <MoreVertical className="h-4 w-4" aria-hidden="true" />
                            </Button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="end" side="bottom">
                            <DropdownMenuLabel>Acciones</DropdownMenuLabel>
                            <DropdownMenuItem
                              onClick={(e) => {
                                e.stopPropagation()
                                navigate(ticketDetailPath(ticket.id))
                              }}
                            >
                              <Eye className="mr-2 h-3.5 w-3.5" />
                              Ver detalle
                            </DropdownMenuItem>
                            {isAdmin && (
                              <>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem
                                  onClick={(e) => {
                                    e.stopPropagation()
                                    setEditTicket(ticket)
                                  }}
                                >
                                  <Pencil className="mr-2 h-3.5 w-3.5" />
                                  Modificar ticket
                                </DropdownMenuItem>
                                {['sin_asignar', 'reabierto'].includes(
                                  ticket.estado.toLowerCase(),
                                ) && (
                                  <DropdownMenuItem
                                    onClick={(e) => {
                                      e.stopPropagation()
                                      setAssignTicketId(ticket.id)
                                    }}
                                  >
                                    <UserCheck className="mr-2 h-3.5 w-3.5" />
                                    Asignar responsable
                                  </DropdownMenuItem>
                                )}
                              </>
                            )}
                            <DropdownMenuSeparator />
                            <DropdownMenuItem
                              onClick={(e) => {
                                e.stopPropagation()
                                setHistoryTicketId(ticket.id)
                              }}
                            >
                              <History className="mr-2 h-3.5 w-3.5" />
                              Ver historial de cambios
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>

          {/* ── DESKTOP: table ────────────────────────────────────────────── */}
          <div className="hidden lg:block">
            <div className="rounded-lg border">
              <Table>
                <TableHeader>
                  <TableRow className="text-[10px] uppercase tracking-wide text-muted-foreground">
                    <TableHead className="h-8 font-semibold">Código</TableHead>
                    <TableHead className="h-8 font-semibold">Tipo de Servicio</TableHead>
                    <TableHead className="h-8 font-semibold">Empresa</TableHead>
                    <TableHead className="h-8 text-center font-semibold">Prioridad</TableHead>
                    <TableHead className="h-8 text-center font-semibold">Estado</TableHead>
                    <TableHead className="h-8 text-center font-semibold">Asignado a</TableHead>
                    <TableHead className="h-8 whitespace-nowrap font-semibold">
                      F. creación
                    </TableHead>
                    <TableHead className="h-8 whitespace-nowrap font-semibold">Acciones</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {tickets.map((ticket) => (
                    <TableRow
                      key={ticket.id}
                      className="cursor-pointer hover:bg-muted/40"
                      onClick={() => navigate(ticketDetailPath(ticket.id))}
                    >
                      <TableCell className="py-2">
                        <span className="font-mono text-xs text-muted-foreground">
                          {ticket.codigo}
                        </span>
                      </TableCell>
                      <TableCell className="py-2">
                        <p className="max-w-[220px] truncate text-xs font-medium">
                          {ticket.titulo}
                        </p>
                        <p className="text-[10px] text-muted-foreground">{ticket.tipo}</p>
                      </TableCell>
                      <TableCell className="py-2">
                        <p className="text-xs">{ticket.sucursalNombre}</p>
                        <p className="text-[10px] text-muted-foreground">{ticket.areaNombre}</p>
                      </TableCell>
                      <TableCell className="py-2 text-center">
                        <div className="flex justify-center">
                          <PriorityBadge priority={normalizePrioridad(ticket.prioridadEfectiva)} />
                        </div>
                      </TableCell>
                      <TableCell className="py-2 text-center">
                        <div className="flex justify-center">
                          <StatusBadge status={normalizeEstado(ticket.estado)} />
                        </div>
                      </TableCell>
                      <TableCell className="py-2 text-center">
                        {ticket.asignadoANombre ? (
                          <div className="flex flex-col items-center gap-0.5">
                            <div className="flex items-center gap-1.5">
                              <div className="flex h-6 w-6 items-center justify-center rounded-full bg-primary/20 text-[9px] font-bold text-primary">
                                {ticket.asignadoANombre
                                  .split(' ')
                                  .map((n) => n?.[0] ?? '')
                                  .join('')
                                  .slice(0, 2)
                                  .toUpperCase()}
                              </div>
                              <span className="max-w-24 truncate text-xs">
                                {ticket.asignadoANombre}
                              </span>
                            </div>
                          </div>
                        ) : (
                          <Badge variant="outline" className="text-xs">
                            Sin asignar
                          </Badge>
                        )}
                      </TableCell>
                      <TableCell className="py-2">
                        <span className="text-xs text-muted-foreground">
                          {new Date(ticket.fechaCreacion).toLocaleDateString('es-PE', {
                            day: '2-digit',
                            month: '2-digit',
                            year: '2-digit',
                          })}
                        </span>
                      </TableCell>
                      <TableCell className="py-2 text-center">
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-7 w-7"
                              onClick={(e) => e.stopPropagation()}
                              aria-label={`Acciones para ticket ${ticket.codigo}`}
                            >
                              <MoreVertical className="h-4 w-4" aria-hidden="true" />
                            </Button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="end" side="bottom">
                            <DropdownMenuLabel>Acciones</DropdownMenuLabel>
                            <DropdownMenuItem
                              onClick={(e) => {
                                e.stopPropagation()
                                navigate(ticketDetailPath(ticket.id))
                              }}
                            >
                              <Eye className="mr-2 h-3.5 w-3.5" />
                              Ver detalle
                            </DropdownMenuItem>
                            {isAdmin && (
                              <>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem
                                  onClick={(e) => {
                                    e.stopPropagation()
                                    setEditTicket(ticket)
                                  }}
                                >
                                  <Pencil className="mr-2 h-3.5 w-3.5" />
                                  Modificar ticket
                                </DropdownMenuItem>
                                {['sin_asignar', 'reabierto'].includes(
                                  ticket.estado.toLowerCase(),
                                ) && (
                                  <DropdownMenuItem
                                    onClick={(e) => {
                                      e.stopPropagation()
                                      setAssignTicketId(ticket.id)
                                    }}
                                  >
                                    <UserCheck className="mr-2 h-3.5 w-3.5" />
                                    Asignar responsable
                                  </DropdownMenuItem>
                                )}
                              </>
                            )}
                            <DropdownMenuSeparator />
                            <DropdownMenuItem
                              onClick={(e) => {
                                e.stopPropagation()
                                setHistoryTicketId(ticket.id)
                              }}
                            >
                              <History className="mr-2 h-3.5 w-3.5" />
                              Ver historial de cambios
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </div>
        </>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-xs text-muted-foreground">
            Página {page} de {totalPages} · {totalRegistros} resultados
          </p>
          <div className="flex items-center gap-1.5">
            <Button
              variant="outline"
              size="icon"
              className="h-9 w-9"
              disabled={page === 1}
              onClick={() => setPage((p) => p - 1)}
              aria-label="Página anterior"
            >
              <ChevronLeft className="h-4 w-4" aria-hidden="true" />
            </Button>
            <Button
              variant="outline"
              size="icon"
              className="h-9 w-9"
              disabled={page === totalPages}
              onClick={() => setPage((p) => p + 1)}
              aria-label="Página siguiente"
            >
              <ChevronRight className="h-4 w-4" aria-hidden="true" />
            </Button>
          </div>
        </div>
      )}

      {/* Edit ticket sheet */}
      <EditTicketSheet
        ticket={editTicket}
        onClose={() => setEditTicket(null)}
        onSave={handleSaveTicket}
      />

      {/* History sheet — bottom drawer para mejor UX mobile */}
      <Sheet open={!!historyTicketId} onOpenChange={(open) => !open && setHistoryTicketId(null)}>
        <SheetContent side="bottom" className="flex h-[85dvh] flex-col rounded-t-2xl p-0">
          {(() => {
            const t = tickets.find((tk) => tk.id === historyTicketId)
            const entries = historialQuery.data ?? []
            return (
              <>
                {/* Handle visual */}
                <div className="flex justify-center pt-3">
                  <div className="h-1 w-10 rounded-full bg-border" />
                </div>

                <SheetHeader className="shrink-0 px-5 pb-3 pt-4">
                  <SheetTitle className="text-base font-semibold">Historial de cambios</SheetTitle>
                  {t && (
                    <SheetDescription className="text-xs">
                      {t.codigo} · {t.titulo}
                      {entries.length > 0 && (
                        <span className="ml-2 rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-medium text-primary">
                          {entries.length} {entries.length === 1 ? 'evento' : 'eventos'}
                        </span>
                      )}
                    </SheetDescription>
                  )}
                </SheetHeader>

                {/* Contenedor relativo — overflow-hidden para que el gradiente quede fijo */}
                <div className="relative min-h-0 flex-1 overflow-hidden">
                  {historialQuery.isLoading ? (
                    <p className="py-8 text-center text-sm text-muted-foreground">
                      Cargando historial...
                    </p>
                  ) : entries.length === 0 ? (
                    <div className="flex flex-col items-center gap-2 py-10">
                      <History className="h-8 w-8 text-muted-foreground/30" />
                      <p className="text-sm text-muted-foreground">Sin registros de historial.</p>
                    </div>
                  ) : (
                    <>
                      {/*
                        absolute inset-0 da dimensiones exactas al scroll en iOS Safari.
                        h-full dentro de flex no garantiza altura definida en iOS.
                        overscroll-contain evita que el scroll se propague a la página.
                      */}
                      <div
                        className="absolute inset-0 overflow-y-auto overscroll-contain pb-20"
                        style={{ WebkitOverflowScrolling: 'touch' }}
                      >
                        {entries.map((entry, i) => {
                          const actor = entry.actorNombre ?? 'Sistema'
                          const fecha = new Date(entry.createdAt).toLocaleString('es-PE', {
                            day: '2-digit',
                            month: 'short',
                            year: 'numeric',
                            hour: '2-digit',
                            minute: '2-digit',
                          })
                          const detalle =
                            entry.comentarioTexto ??
                            (entry.estadoAnterior && entry.estadoNuevo
                              ? `${entry.estadoAnterior} → ${entry.estadoNuevo}`
                              : null)
                          return (
                            <div
                              key={entry.id}
                              className={cn(
                                'flex items-start gap-3 px-5 py-3.5',
                                i < entries.length - 1 && 'border-b border-border/50',
                              )}
                            >
                              <div className="mt-0.5 flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                                <History className="h-4 w-4 text-primary" />
                              </div>
                              <div className="min-w-0 flex-1">
                                <p className="text-sm font-semibold leading-tight">
                                  {entry.tipoEvento.replace(/_/g, ' ')}
                                </p>
                                {detalle && (
                                  <p className="mt-0.5 text-xs text-foreground/70">{detalle}</p>
                                )}
                                <p className="mt-1 text-xs text-muted-foreground">
                                  {actor} · {fecha}
                                </p>
                              </div>
                            </div>
                          )
                        })}
                      </div>

                      {/* Gradiente fijo al fondo + flecha de scroll */}
                      <div className="pointer-events-none absolute bottom-0 left-0 right-0 flex flex-col items-center pb-3 pt-8 [background:linear-gradient(to_top,hsl(var(--background))_40%,transparent)]">
                        <ChevronDown className="h-5 w-5 animate-bounce text-muted-foreground/60" />
                      </div>
                    </>
                  )}
                </div>
              </>
            )
          })()}
        </SheetContent>
      </Sheet>

      {/* Assign modal */}
      <Dialog
        open={!!assignTicketId}
        onOpenChange={(open) => {
          if (!open) {
            setAssignTicketId(null)
            setSelectedAssignWorker('')
          }
        }}
      >
        <DialogContent className="ps-glow-modal w-[calc(100%-2rem)] max-w-sm gap-4 rounded-2xl p-5">
          <DialogHeader className="pb-0">
            <DialogTitle className="text-base font-semibold">Asignar trabajador</DialogTitle>
          </DialogHeader>

          {assigningTicket && (
            <div className="bg-primary/8 flex items-start gap-3 rounded-xl border border-primary/20 px-4 py-3">
              <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-primary/15">
                <UserCheck className="h-4 w-4 text-primary" />
              </div>
              <div className="min-w-0 pt-0.5">
                <p className="truncate text-sm font-semibold leading-tight">
                  {assigningTicket.titulo}
                </p>
                <p className="mt-0.5 text-xs font-medium text-primary/70">
                  {assigningTicket.codigo}
                </p>
              </div>
            </div>
          )}

          <div className="space-y-1.5">
            <label className="text-xs font-medium">
              Trabajador <span className="text-destructive">*</span>
            </label>
            <Select value={selectedAssignWorker} onValueChange={setSelectedAssignWorker}>
              <SelectTrigger className="h-11">
                <SelectValue placeholder="Selecciona un trabajador" />
              </SelectTrigger>
              <SelectContent>
                {workers.map((w) => (
                  <SelectItem key={w.id} value={w.id}>
                    <div className="flex items-center gap-2">
                      <span className="flex h-6 w-6 items-center justify-center rounded-full bg-primary/20 text-[10px] font-semibold text-primary">
                        {w.nombreCompleto
                          .split(' ')
                          .map((p) => p[0] ?? '')
                          .join('')
                          .slice(0, 2)
                          .toUpperCase()}
                      </span>
                      <span className="text-sm">{w.nombreCompleto}</span>
                    </div>
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="flex gap-2 pt-1">
            <Button
              variant="outline"
              className="h-11 flex-1"
              onClick={() => {
                setAssignTicketId(null)
                setSelectedAssignWorker('')
              }}
            >
              Cancelar
            </Button>
            <Button
              className="h-11 flex-1"
              disabled={!selectedAssignWorker || asignarTicket.isPending}
              onClick={handleAssign}
            >
              {asignarTicket.isPending ? 'Asignando...' : 'Asignar'}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  )
}
