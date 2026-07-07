import { useState, useMemo, useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import {
  Search,
  Plus,
  Filter,
  ChevronLeft,
  ChevronRight,
  X,
  UserCheck,
  MoreVertical,
  Pencil,
  Eye,
  History,
} from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent } from '@shared/ui/card'
import { Badge } from '@shared/ui/badge'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { Textarea } from '@shared/ui/textarea'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@shared/ui/dialog'
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@shared/ui/sheet'
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
import { MOCK_TICKETS, MOCK_USERS, MOCK_SUCURSALES, MOCK_AREAS, TICKET_TYPES } from '@mocks/data'
import type { MockTicket } from '@mocks/data'
import { ROUTES, ticketDetailPath } from '@constants/index'
import type { TicketStatus, TicketPriority } from '@types-app/index'

// ---------------------------------------------------------------------------
// EditTicketSheet
// ---------------------------------------------------------------------------

interface EditTicketSheetProps {
  ticket: MockTicket | null
  onClose: () => void
  onSave: (updated: MockTicket, historyEntry: string) => void
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
  const [form, setForm] = useState({
    type: ticket?.type ?? '',
    title: ticket?.title ?? '',
    sucursalId: ticket?.sucursalId ?? '',
    areaId: ticket?.areaId ?? '',
    priority: (ticket?.priority ?? 'media') as TicketPriority,
    description: ticket?.description ?? '',
    status: (ticket?.status ?? 'sin_asignar') as TicketStatus,
    assignedTo: ticket?.assignedTo?.id ?? '',
  })
  const [errors, setErrors] = useState<Partial<Record<string, string>>>({})

  const areasFiltered = useMemo(
    () => MOCK_AREAS.filter((a) => a.sucursalId === form.sucursalId && a.activo),
    [form.sucursalId],
  )

  const sheetWorkers = MOCK_USERS.filter((u) => u.rol === 'worker' && u.activo)

  useEffect(() => {
    if (ticket) {
      setForm({
        type: ticket.type,
        title: ticket.title,
        sucursalId: ticket.sucursalId,
        areaId: ticket.areaId,
        priority: ticket.priority,
        description: ticket.description,
        status: ticket.status,
        assignedTo: ticket.assignedTo?.id ?? '',
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
    if (!form.type) next.type = 'Selecciona un tipo de servicio.'
    if (!form.title.trim()) next.title = 'Ingresa un título.'
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

    const assignedUser = form.assignedTo
      ? MOCK_USERS.find((u) => u.id === form.assignedTo)
      : undefined

    const updated: MockTicket = {
      ...ticket,
      type: form.type,
      title: form.title.trim(),
      sucursalId: form.sucursalId,
      sucursal: MOCK_SUCURSALES.find((s) => s.id === form.sucursalId)?.name ?? ticket.sucursal,
      areaId: form.areaId,
      area: MOCK_AREAS.find((a) => a.id === form.areaId)?.name ?? ticket.area,
      priority: form.priority,
      description: form.description.trim(),
      status: form.status,
      assignedTo: assignedUser
        ? { id: assignedUser.id, fullName: assignedUser.fullName, initials: assignedUser.initials }
        : ticket.assignedTo,
      updatedAt: new Date().toISOString(),
    }

    const parts: string[] = []
    if (form.type !== ticket.type) parts.push('Tipo de servicio actualizado.')
    if (form.title.trim() !== ticket.title) parts.push('Título actualizado.')
    if (form.sucursalId !== ticket.sucursalId) parts.push('Empresa actualizada.')
    if (form.areaId !== ticket.areaId) parts.push('Sucursal actualizada.')
    if (form.priority !== ticket.priority) parts.push('Prioridad actualizada.')
    if (form.description.trim() !== ticket.description) parts.push('Descripción actualizada.')
    if (form.status !== ticket.status) parts.push('Estado actualizado.')

    const historyEntry =
      parts.length > 0 ? 'Ticket modificado: ' + parts.join(' ') : 'Ticket modificado.'

    onSave(updated, historyEntry)
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
                {ticket.code}
              </span>
              <span>{ticket.type}</span>
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

            <FormField label="Tipo de servicio" required error={errors.type}>
              <Select value={form.type} onValueChange={(v) => handleField('type', v)}>
                <SelectTrigger className="h-9 text-sm">
                  <SelectValue placeholder="Selecciona un tipo de servicio" />
                </SelectTrigger>
                <SelectContent>
                  {TICKET_TYPES.map((t) => (
                    <SelectItem key={t} value={t}>
                      {t}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FormField>

            <FormField label="Título" required error={errors.title}>
              <Input
                className="h-9 text-sm"
                value={form.title}
                onChange={(e) => handleField('title', e.target.value)}
                placeholder="Título del ticket"
              />
            </FormField>

            <div className="grid grid-cols-2 gap-3">
              <FormField label="Empresa" required error={errors.sucursalId}>
                <Select value={form.sucursalId} onValueChange={(v) => handleField('sucursalId', v)}>
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue placeholder="Selecciona" />
                  </SelectTrigger>
                  <SelectContent>
                    {MOCK_SUCURSALES.filter((s) => s.activo).map((s) => (
                      <SelectItem key={s.id} value={s.id}>
                        {s.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </FormField>

              <FormField label="Sucursal" required error={errors.areaId}>
                <Select
                  value={form.areaId}
                  onValueChange={(v) => handleField('areaId', v)}
                  disabled={!form.sucursalId}
                >
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue placeholder={form.sucursalId ? 'Selecciona' : 'Primero empresa'} />
                  </SelectTrigger>
                  <SelectContent>
                    {areasFiltered.map((a) => (
                      <SelectItem key={a.id} value={a.id}>
                        {a.name}
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

            <div className="grid grid-cols-2 gap-3">
              <FormField label="Estado">
                <Select value={form.status} onValueChange={(v) => handleField('status', v)}>
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="sin_asignar">Sin asignar</SelectItem>
                    <SelectItem value="asignado">Asignado</SelectItem>
                    <SelectItem value="en_proceso">En proceso</SelectItem>
                    <SelectItem value="pendiente_validacion">Pend. validación</SelectItem>
                    <SelectItem value="cerrado">Cerrado</SelectItem>
                    <SelectItem value="reabierto">Reabierto</SelectItem>
                  </SelectContent>
                </Select>
              </FormField>

              <FormField label="Asignado a">
                <Select value={form.assignedTo} onValueChange={(v) => handleField('assignedTo', v)}>
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue placeholder="Sin asignar" />
                  </SelectTrigger>
                  <SelectContent>
                    {sheetWorkers.map((u) => (
                      <SelectItem key={u.id} value={u.id}>
                        {u.fullName}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </FormField>
            </div>
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

const PAGE_SIZE = 8

function getAssignedBy(ticket: MockTicket) {
  return ticket.history.find((h) => h.action === 'Ticket asignado')?.author ?? null
}

export function MyTicketsPage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const user = useAuthStore((s) => s.user)
  const isAdmin = user?.rol === 'admin' || user?.rol === 'superadmin'

  const [search, setSearch] = useState('')
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

  // Mutable local ticket state for assign action
  const [localTickets, setLocalTickets] = useState<MockTicket[]>(() => [...MOCK_TICKETS])

  // Assign modal state
  const [assignTicketId, setAssignTicketId] = useState<string | null>(null)
  const [selectedAssignWorker, setSelectedAssignWorker] = useState('')

  // Edit ticket modal state
  const [editTicket, setEditTicket] = useState<MockTicket | null>(null)

  // History dialog state
  const [historyTicketId, setHistoryTicketId] = useState<string | null>(null)

  const workers = useMemo(() => MOCK_USERS.filter((u) => u.rol === 'worker' && u.activo), [])

  const filtered = useMemo(() => {
    const source: MockTicket[] = isAdmin
      ? localTickets
      : user
        ? localTickets.filter((t) => t.createdBy.id === user.id || t.assignedTo?.id === user.id)
        : []

    return source
      .filter((t) => {
        const matchSearch =
          !search ||
          t.title.toLowerCase().includes(search.toLowerCase()) ||
          t.code.toLowerCase().includes(search.toLowerCase()) ||
          t.area.toLowerCase().includes(search.toLowerCase())
        const matchStatus = statusFilter === 'all' || t.status === statusFilter
        const matchPriority = priorityFilter === 'all' || t.priority === priorityFilter

        const ticketDate = new Date(t.createdAt).getTime()
        const matchFrom = !dateFrom || ticketDate >= new Date(dateFrom).getTime()
        const matchTo = !dateTo || ticketDate <= new Date(dateTo + 'T23:59:59').getTime()

        return matchSearch && matchStatus && matchPriority && matchFrom && matchTo
      })
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
  }, [isAdmin, user, localTickets, search, statusFilter, priorityFilter, dateFrom, dateTo])

  const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE))
  const paginated = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

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
    const worker = MOCK_USERS.find((u) => u.id === selectedAssignWorker)
    if (!worker || !assignTicketId) return

    setLocalTickets((prev) =>
      prev.map((t) =>
        t.id === assignTicketId
          ? {
              ...t,
              status: 'asignado' as TicketStatus,
              assignedTo: { id: worker.id, fullName: worker.fullName, initials: worker.initials },
              updatedAt: new Date().toISOString(),
            }
          : t,
      ),
    )
    toast.success(`Ticket asignado a ${worker.fullName}`)
    setSelectedAssignWorker('')
    setAssignTicketId(null)
  }

  const assigningTicket = assignTicketId ? localTickets.find((t) => t.id === assignTicketId) : null

  const handleSaveTicket = (updated: MockTicket, _historyEntry: string) => {
    setLocalTickets((prev) => prev.map((t) => (t.id === updated.id ? updated : t)))
    toast.success('Ticket modificado correctamente')
  }

  const [isLoading, setIsLoading] = useState(true)
  useEffect(() => {
    const t = setTimeout(() => setIsLoading(false), 600)
    return () => clearTimeout(t)
  }, [])

  if (isLoading) return <TicketListSkeleton />

  return (
    <div className="space-y-3 p-3 lg:p-4">
      {/* Page header */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-base font-semibold tracking-tight">
            {isAdmin ? 'Gestión de tickets' : 'Mis tickets'}
          </h2>
          <p className="text-xs text-muted-foreground">
            {filtered.length} ticket{filtered.length !== 1 ? 's' : ''} encontrado
            {filtered.length !== 1 ? 's' : ''}
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
                <SelectItem value="pendiente_validacion">Pend. validación</SelectItem>
                <SelectItem value="cerrado">Cerrado</SelectItem>
                <SelectItem value="reabierto">Reabierto</SelectItem>
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
            <Input
              type="date"
              className="h-8 w-full text-xs lg:w-[132px]"
              value={dateFrom}
              onChange={(e) => {
                setDateFrom(e.target.value)
                handleFilterChange()
              }}
            />
          </div>

          {/* Hasta */}
          <div className="flex flex-col gap-1">
            <span className="text-[10px] font-medium text-muted-foreground">Hasta</span>
            <Input
              type="date"
              className="h-8 w-full text-xs lg:w-[132px]"
              value={dateTo}
              onChange={(e) => {
                setDateTo(e.target.value)
                handleFilterChange()
              }}
            />
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
      {paginated.length === 0 ? (
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
            {paginated.map((ticket) => (
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
                          {ticket.code}
                        </span>
                        <PriorityBadge priority={ticket.priority} showIcon />
                      </div>
                      <p className="mt-1 text-xs font-medium leading-snug">{ticket.title}</p>
                      <p className="mt-0.5 text-xs text-muted-foreground">
                        {ticket.sucursal} · {ticket.area}
                      </p>
                      <p className="mt-0.5 text-xs text-muted-foreground">{ticket.type}</p>
                      {ticket.assignedTo &&
                        (() => {
                          const assignedBy = getAssignedBy(ticket)
                          return (
                            <div className="mt-0.5 space-y-0.5">
                              <p className="text-xs text-muted-foreground">
                                <span className="font-medium text-foreground/70">Asignado a:</span>{' '}
                                {ticket.assignedTo.fullName}
                              </p>
                              {assignedBy && (
                                <p className="text-xs text-muted-foreground">
                                  <span className="font-medium text-foreground/70">Por:</span>{' '}
                                  {assignedBy.fullName}
                                </p>
                              )}
                            </div>
                          )
                        })()}
                    </div>
                    <div className="flex flex-col items-end gap-1.5">
                      <div className="flex items-center gap-1.5">
                        <StatusBadge status={ticket.status} />
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-7 w-7 shrink-0"
                              onClick={(e) => e.stopPropagation()}
                            >
                              <MoreVertical className="h-4 w-4" />
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
                                <DropdownMenuItem
                                  onClick={(e) => {
                                    e.stopPropagation()
                                    setAssignTicketId(ticket.id)
                                  }}
                                >
                                  <UserCheck className="mr-2 h-3.5 w-3.5" />
                                  Asignar responsable
                                </DropdownMenuItem>
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
                  {paginated.map((ticket) => (
                    <TableRow
                      key={ticket.id}
                      className="cursor-pointer hover:bg-muted/40"
                      onClick={() => navigate(ticketDetailPath(ticket.id))}
                    >
                      <TableCell className="py-2">
                        <span className="font-mono text-xs text-muted-foreground">
                          {ticket.code}
                        </span>
                      </TableCell>
                      <TableCell className="py-2">
                        <p className="max-w-[220px] truncate text-xs font-medium">{ticket.title}</p>
                        <p className="text-[10px] text-muted-foreground">{ticket.type}</p>
                      </TableCell>
                      <TableCell className="py-2">
                        <p className="text-xs">{ticket.sucursal}</p>
                        <p className="text-[10px] text-muted-foreground">{ticket.area}</p>
                      </TableCell>
                      <TableCell className="py-2 text-center">
                        <div className="flex justify-center">
                          <PriorityBadge priority={ticket.priority} />
                        </div>
                      </TableCell>
                      <TableCell className="py-2 text-center">
                        <div className="flex justify-center">
                          <StatusBadge status={ticket.status} />
                        </div>
                      </TableCell>
                      <TableCell className="py-2 text-center">
                        {ticket.assignedTo ? (
                          <div className="flex flex-col items-center gap-0.5">
                            <div className="flex items-center gap-1.5">
                              <div className="flex h-6 w-6 items-center justify-center rounded-full bg-primary/20 text-[9px] font-bold text-primary">
                                {ticket.assignedTo.initials}
                              </div>
                              <span className="max-w-24 truncate text-xs">
                                {ticket.assignedTo.fullName}
                              </span>
                            </div>
                            {(() => {
                              const assignedBy = getAssignedBy(ticket)
                              return assignedBy ? (
                                <p className="text-[10px] text-muted-foreground">
                                  Por: {assignedBy.fullName}
                                </p>
                              ) : null
                            })()}
                          </div>
                        ) : (
                          <Badge variant="outline" className="text-xs">
                            Sin asignar
                          </Badge>
                        )}
                      </TableCell>
                      <TableCell className="py-2">
                        <span className="text-xs text-muted-foreground">
                          {new Date(ticket.createdAt).toLocaleDateString('es-PE', {
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
                            >
                              <MoreVertical className="h-4 w-4" />
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
                                <DropdownMenuItem
                                  onClick={(e) => {
                                    e.stopPropagation()
                                    setAssignTicketId(ticket.id)
                                  }}
                                >
                                  <UserCheck className="mr-2 h-3.5 w-3.5" />
                                  Asignar responsable
                                </DropdownMenuItem>
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
            Página {page} de {totalPages} · {filtered.length} resultados
          </p>
          <div className="flex items-center gap-1.5">
            <Button
              variant="outline"
              size="icon"
              className="h-9 w-9"
              disabled={page === 1}
              onClick={() => setPage((p) => p - 1)}
            >
              <ChevronLeft className="h-4 w-4" />
            </Button>
            <Button
              variant="outline"
              size="icon"
              className="h-9 w-9"
              disabled={page === totalPages}
              onClick={() => setPage((p) => p + 1)}
            >
              <ChevronRight className="h-4 w-4" />
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

      {/* History dialog */}
      <Dialog open={!!historyTicketId} onOpenChange={(open) => !open && setHistoryTicketId(null)}>
        <DialogContent className="ps-glow-modal max-w-md">
          <DialogHeader>
            <DialogTitle className="text-base font-semibold">Historial de cambios</DialogTitle>
          </DialogHeader>
          {(() => {
            const t = localTickets.find((tk) => tk.id === historyTicketId)
            if (!t) return null
            const entries = [
              {
                id: 1,
                action: 'Ticket creado',
                detail: `Tipo: ${t.type} · Prioridad: ${t.priority}`,
                user: t.createdBy.fullName,
                date: t.createdAt,
              },
              ...(t.assignedTo
                ? [
                    {
                      id: 2,
                      action: 'Responsable asignado',
                      detail: `Asignado a: ${t.assignedTo.fullName}`,
                      user: 'Administrador',
                      date: t.updatedAt,
                    },
                  ]
                : []),
              ...(t.status !== 'sin_asignar'
                ? [
                    {
                      id: 3,
                      action: 'Estado actualizado',
                      detail: `Estado: ${t.status.replace(/_/g, ' ')}`,
                      user: 'Administrador',
                      date: t.updatedAt,
                    },
                  ]
                : []),
              ...(t.priority !== 'baja'
                ? [
                    {
                      id: 4,
                      action: 'Prioridad ajustada',
                      detail: `Prioridad: ${t.priority}`,
                      user: 'Administrador',
                      date: t.updatedAt,
                    },
                  ]
                : []),
            ]
            return (
              <div className="space-y-1">
                <p className="mb-2 text-xs text-muted-foreground">
                  {t.code} · {t.title}
                </p>
                <div className="max-h-64 divide-y overflow-y-auto rounded-lg border">
                  {entries.map((entry) => (
                    <div key={entry.id} className="flex items-start gap-3 px-3 py-2.5">
                      <div className="mt-0.5 flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-primary/10">
                        <History className="h-3 w-3 text-primary" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-xs font-medium">{entry.action}</p>
                        <p className="text-[11px] text-muted-foreground">{entry.detail}</p>
                        <p className="mt-0.5 text-[10px] text-muted-foreground">
                          {entry.user} ·{' '}
                          {new Date(entry.date).toLocaleDateString('es-PE', {
                            day: '2-digit',
                            month: 'short',
                            year: 'numeric',
                          })}
                        </p>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )
          })()}
        </DialogContent>
      </Dialog>

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
        <DialogContent className="ps-glow-modal max-w-md">
          <DialogHeader>
            <DialogTitle className="text-base font-semibold">Asignar trabajador</DialogTitle>
          </DialogHeader>
          {assigningTicket && (
            <div className="rounded-lg bg-muted p-3 text-xs">
              <p className="font-medium">{assigningTicket.title}</p>
              <p className="text-muted-foreground">{assigningTicket.code}</p>
            </div>
          )}
          <FormField label="Trabajador" required>
            <Select value={selectedAssignWorker} onValueChange={setSelectedAssignWorker}>
              <SelectTrigger>
                <SelectValue placeholder="Selecciona un trabajador" />
              </SelectTrigger>
              <SelectContent>
                {workers.map((w) => (
                  <SelectItem key={w.id} value={w.id}>
                    <div className="flex items-center gap-2">
                      <span className="flex h-5 w-5 items-center justify-center rounded-full bg-primary/20 text-[9px] font-semibold text-primary">
                        {w.initials}
                      </span>
                      <span>{w.fullName}</span>
                      <Badge variant="outline" className="ml-auto text-[9px]">
                        {w.sucursal}
                      </Badge>
                    </div>
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FormField>
          <DialogFooter>
            <Button
              variant="outline"
              size="sm"
              onClick={() => {
                setAssignTicketId(null)
                setSelectedAssignWorker('')
              }}
            >
              Cancelar
            </Button>
            <Button size="sm" disabled={!selectedAssignWorker} onClick={handleAssign}>
              Asignar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
