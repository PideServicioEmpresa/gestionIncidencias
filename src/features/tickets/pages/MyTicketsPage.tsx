import { useState, useMemo } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { Search, Plus, Filter, ChevronLeft, ChevronRight, ArrowUpDown } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent } from '@shared/ui/card'
import { Badge } from '@shared/ui/badge'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { StatusBadge } from '@shared/components/StatusBadge'
import { PriorityBadge } from '@shared/components/PriorityBadge'
import { EmptyState } from '@shared/components/EmptyState'
import { useAuthStore } from '@store/auth.store'
import { MOCK_TICKETS, getTicketsByUser } from '@mocks/data'
import { ROUTES, ticketDetailPath } from '@constants/index'
import type { TicketStatus, TicketPriority } from '@types-app/index'

const PAGE_SIZE = 8

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
  const [page, setPage] = useState(1)

  const filtered = useMemo(() => {
    const source = isAdmin ? MOCK_TICKETS : user ? getTicketsByUser(user.id) : []
    return source.filter((t) => {
      const matchSearch =
        !search ||
        t.title.toLowerCase().includes(search.toLowerCase()) ||
        t.code.toLowerCase().includes(search.toLowerCase()) ||
        t.area.toLowerCase().includes(search.toLowerCase())
      const matchStatus = statusFilter === 'all' || t.status === statusFilter
      const matchPriority = priorityFilter === 'all' || t.priority === priorityFilter
      return matchSearch && matchStatus && matchPriority
    })
  }, [isAdmin, user, search, statusFilter, priorityFilter])

  const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE))
  const paginated = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  const handleFilterChange = () => setPage(1)

  return (
    <div className="space-y-3 p-3 lg:p-5">
      {/* Page header */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-lg font-semibold tracking-tight">
            {isAdmin ? 'Gestión de tickets' : 'Mis tickets'}
          </h2>
          <p className="text-sm text-muted-foreground">
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
          <button
            className="ml-auto text-primary/70 hover:text-primary"
            onClick={() => {
              setStatusFilter('all')
              setPriorityFilter('all')
            }}
          >
            Limpiar
          </button>
        </div>
      )}

      {/* Filters */}
      <div className="flex flex-col gap-1.5 sm:flex-row">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Buscar por título, código o área..."
            className="h-8 pl-9 text-xs"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value)
              handleFilterChange()
            }}
          />
        </div>
        <Select
          value={statusFilter}
          onValueChange={(v) => {
            setStatusFilter(v as TicketStatus | 'all')
            handleFilterChange()
          }}
        >
          <SelectTrigger className="h-8 w-full text-xs sm:w-44">
            <Filter className="mr-2 h-3.5 w-3.5 text-muted-foreground" />
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
        <Select
          value={priorityFilter}
          onValueChange={(v) => {
            setPriorityFilter(v as TicketPriority | 'all')
            handleFilterChange()
          }}
        >
          <SelectTrigger className="h-8 w-full text-xs sm:w-40">
            <ArrowUpDown className="mr-2 h-3.5 w-3.5 text-muted-foreground" />
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

      {/* Tickets list — card layout */}
      {paginated.length === 0 ? (
        <EmptyState
          icon={Search}
          title="Sin resultados"
          description="No encontramos tickets con los filtros seleccionados."
          action={
            <Button
              variant="outline"
              onClick={() => {
                setSearch('')
                setStatusFilter('all')
                setPriorityFilter('all')
              }}
            >
              Limpiar filtros
            </Button>
          }
        />
      ) : (
        <div className="grid gap-2">
          {/* Table header — hidden on mobile */}
          <div className="hidden grid-cols-[auto_1fr_auto_auto_auto] items-center gap-4 rounded-lg border bg-muted/40 px-4 py-2 text-xs font-medium text-muted-foreground lg:grid">
            <span>Código</span>
            <span>Título · Área</span>
            <span className="text-center">Prioridad</span>
            <span className="text-center">Estado</span>
            <span>Asignado a</span>
          </div>

          {paginated.map((ticket) => (
            <Card
              key={ticket.id}
              className="cursor-pointer transition-all hover:border-primary/30 hover:shadow-md"
              onClick={() => navigate(ticketDetailPath(ticket.id))}
            >
              <CardContent className="p-3">
                {/* Mobile layout */}
                <div className="flex items-start gap-3 lg:hidden">
                  <div className="min-w-0 flex-1">
                    <div className="flex flex-wrap items-center gap-2">
                      <span className="font-mono text-xs text-muted-foreground">{ticket.code}</span>
                      <PriorityBadge priority={ticket.priority} showIcon />
                    </div>
                    <p className="mt-1 font-medium leading-snug">{ticket.title}</p>
                    <p className="mt-0.5 text-xs text-muted-foreground">
                      {ticket.sucursal} · {ticket.area}
                    </p>
                    {ticket.assignedTo && (
                      <p className="mt-0.5 text-xs text-muted-foreground">
                        Asignado a: {ticket.assignedTo.fullName}
                      </p>
                    )}
                  </div>
                  <StatusBadge status={ticket.status} />
                </div>

                {/* Desktop layout */}
                <div className="hidden grid-cols-[auto_1fr_auto_auto_auto] items-center gap-4 lg:grid">
                  <span className="font-mono text-xs text-muted-foreground">{ticket.code}</span>
                  <div className="min-w-0">
                    <p className="truncate text-sm font-medium">{ticket.title}</p>
                    <p className="text-xs text-muted-foreground">
                      {ticket.sucursal} · {ticket.area}
                    </p>
                  </div>
                  <div className="flex justify-center">
                    <PriorityBadge priority={ticket.priority} />
                  </div>
                  <StatusBadge status={ticket.status} />
                  <div className="text-right">
                    {ticket.assignedTo ? (
                      <div className="flex items-center justify-end gap-1.5">
                        <div className="flex h-7 w-7 items-center justify-center rounded-full bg-primary/20 text-[10px] font-bold text-primary">
                          {ticket.assignedTo.initials}
                        </div>
                        <span className="max-w-24 truncate text-xs">
                          {ticket.assignedTo.fullName}
                        </span>
                      </div>
                    ) : (
                      <Badge variant="outline" className="text-xs">
                        Sin asignar
                      </Badge>
                    )}
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
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
    </div>
  )
}
