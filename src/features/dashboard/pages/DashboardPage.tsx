import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Ticket,
  Clock,
  CheckCircle2,
  AlertTriangle,
  Plus,
  ArrowRight,
  Inbox,
  AlertCircle,
  CheckCheck,
  RotateCcw,
  GripVertical,
  BarChart3,
  ChevronRight,
} from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import { Button } from '@shared/ui/button'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { StatusBadge } from '@shared/components/StatusBadge'
import { PriorityBadge } from '@shared/components/PriorityBadge'
import { useAuthStore } from '@store/auth.store'
import { ROUTES, ticketDetailPath } from '@constants/index'
import { cn } from '@lib/utils'
import { DashboardSkeleton } from '@shared/components/PageSkeletons'
import { ticketService, type TicketListItemDto } from '@features/tickets/services/ticketService'
import type { TicketStatus, TicketPriority } from '@types-app/index'
import {
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  Tooltip,
  Legend,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  AreaChart,
  Area,
  RadarChart,
  PolarGrid,
  PolarAngleAxis,
  Radar,
  ComposedChart,
  Line,
  LineChart,
} from 'recharts'
import { DndContext, closestCenter, PointerSensor, useSensor, useSensors } from '@dnd-kit/core'
import type { DragEndEvent } from '@dnd-kit/core'
import { SortableContext, useSortable, arrayMove, rectSortingStrategy } from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'
import { useDashboardResumen } from '../hooks/useDashboard'

// ── Tipo local para tickets recientes ────────────────────────────────────────

interface DashboardTicket {
  id: string
  code: string
  title: string
  status: TicketStatus
  priority: TicketPriority
  sucursalId: string
  sucursal: string
  areaId: string
  area: string
  tipo: string
  assignedTo?: { id: string; fullName: string }
  createdAt: string
}

function mapTicket(dto: TicketListItemDto): DashboardTicket {
  if (!dto)
    return {
      id: '',
      code: '',
      title: '',
      status: 'sin_asignar' as TicketStatus,
      priority: 'baja' as TicketPriority,
      sucursalId: '',
      sucursal: '',
      areaId: '',
      area: '',
      tipo: '',
      createdAt: '',
    }
  return {
    id: dto.id ?? '',
    code: dto.codigo ?? '',
    title: dto.titulo ?? '',
    status: String(dto.estado ?? '').toLowerCase() as TicketStatus,
    priority: String(dto.prioridadEfectiva ?? '').toLowerCase() as TicketPriority,
    sucursalId: dto.sucursalId ?? '',
    sucursal: dto.sucursalNombre ?? '',
    areaId: dto.areaId ?? '',
    area: dto.areaNombre ?? '',
    tipo: dto.tipo ?? '',
    assignedTo: dto.asignadoANombre
      ? { id: dto.tecnicoId ?? '', fullName: dto.asignadoANombre }
      : undefined,
    createdAt: dto.fechaCreacion ?? dto.id ?? '',
  }
}

// ── Constantes de estilo para gráficos ─────────────────────────────────────────

const TOOLTIP_STYLE = {
  backgroundColor: '#111113',
  border: '1px solid #26262B',
  borderRadius: 8,
  color: '#eaeaea',
  fontSize: 11,
}

const TICK_STYLE = { fill: '#6b7280', fontSize: 10 }
const GRID_COLOR = '#26262B'

const COLORS = {
  sin_asignar: '#6b7280',
  asignado: '#3b82f6',
  en_proceso: '#f97316',
  pendiente_validacion: '#f59e0b',
  cerrado: '#22c55e',
  reabierto: '#ef4444',
  en_espera: '#a855f7',
  cancelado: '#374151',
  baja: '#22c55e',
  media: '#f59e0b',
  alta: '#f97316',
  critica: '#ef4444',
  primary: '#3b82f6',
  success: '#22c55e',
} as const

const TIPO_COLORS = [
  '#3b82f6',
  '#f97316',
  '#22c55e',
  '#f59e0b',
  '#ef4444',
  '#8b5cf6',
  '#ec4899',
  '#06b6d4',
]

const ESTADO_COLOR_MAP: Record<string, string> = {
  SIN_ASIGNAR: COLORS.sin_asignar,
  ASIGNADO: COLORS.asignado,
  EN_PROCESO: COLORS.en_proceso,
  PENDIENTE_VALIDACION: COLORS.pendiente_validacion,
  CERRADO: COLORS.cerrado,
  REABIERTO: COLORS.reabierto,
  EN_ESPERA: COLORS.en_espera,
  CANCELADO: COLORS.cancelado,
}

const PRIORIDAD_COLOR_MAP: Record<string, string> = {
  BAJA: COLORS.baja,
  MEDIA: COLORS.media,
  ALTA: COLORS.alta,
  CRITICA: COLORS.critica,
}

// ── Widget order ────────────────────────────────────────────────────────────────

const DEFAULT_ORDER = [
  'status',
  'priority',
  'trend',
  'sucursal',
  'areas',
  'tipos',
  'responsable',
  'radar',
  'semanal',
]

function getSavedOrder(): string[] {
  try {
    const saved = sessionStorage.getItem('ps-dashboard-order')
    return saved ? (JSON.parse(saved) as string[]) : DEFAULT_ORDER
  } catch {
    return DEFAULT_ORDER
  }
}

// ── ChartCard ───────────────────────────────────────────────────────────────────

function ChartCard({
  title,
  description,
  children,
  className,
  viewAllPath,
}: {
  title: string
  description?: string
  children: React.ReactNode
  className?: string
  viewAllPath?: string
}) {
  const navigate = useNavigate()
  return (
    <Card className={cn('ps-glow-card border-border/60 bg-card', className)}>
      <CardHeader className="flex-row items-start justify-between px-3 pb-2 pt-3">
        <div>
          <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            {title}
          </CardTitle>
          {description && <CardDescription className="text-[10px]">{description}</CardDescription>}
        </div>
        {viewAllPath && (
          <button
            onClick={() => navigate(viewAllPath)}
            className="flex shrink-0 items-center gap-0.5 text-xs text-primary hover:underline"
          >
            Ver todos
            <ChevronRight className="h-3 w-3" />
          </button>
        )}
      </CardHeader>
      <CardContent className="p-3 pt-0">{children}</CardContent>
    </Card>
  )
}

// ── SortableWidget ──────────────────────────────────────────────────────────────

function SortableWidget({
  id,
  children,
  colSpan = 1,
  editMode,
}: {
  id: string
  children: React.ReactNode
  colSpan?: number
  editMode: boolean
}) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id,
  })

  return (
    <div
      ref={setNodeRef}
      style={{ transform: CSS.Transform.toString(transform), transition }}
      className={cn(
        'relative',
        colSpan === 2 && 'col-span-1 lg:col-span-2',
        isDragging && 'z-50 opacity-75 ring-1 ring-primary',
      )}
    >
      {editMode && (
        <button
          className="absolute left-2 top-2 z-10 cursor-grab rounded p-1 text-muted-foreground/60 hover:text-muted-foreground active:cursor-grabbing"
          {...listeners}
          {...attributes}
        >
          <GripVertical className="h-3.5 w-3.5" />
        </button>
      )}
      {children}
    </div>
  )
}

// ── StatCard (Worker) ───────────────────────────────────────────────────────────

interface StatCardProps {
  title: string
  value: number
  description?: string
  icon: React.ComponentType<{ className?: string }>
  iconColor: string
  onClick?: () => void
}

function StatCard({ title, value, description, icon: Icon, iconColor, onClick }: StatCardProps) {
  return (
    <Card
      className={onClick ? 'cursor-pointer transition-shadow hover:shadow-md' : ''}
      onClick={onClick}
    >
      <CardContent className="p-4 lg:p-5">
        <div className="flex items-start justify-between">
          <div className="space-y-1">
            <p className="text-xs font-medium text-muted-foreground">{title}</p>
            <p className="text-2xl font-semibold">{value}</p>
            {description && <p className="text-xs text-muted-foreground">{description}</p>}
          </div>
          <div className={cn('rounded-lg p-2.5', iconColor)}>
            <Icon className="h-4 w-4" />
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

// ── Sparkline mini-chart ────────────────────────────────────────────────────────

function Sparkline({ data, color }: { data: number[]; color: string }) {
  const points = data.map((v, i) => ({ v, i }))
  if (points.length === 0) return null
  return (
    <div className="hidden items-end justify-end lg:flex">
      <LineChart width={60} height={24} data={points}>
        <Line
          type="monotone"
          dataKey="v"
          stroke={color}
          strokeWidth={2}
          dot={false}
          isAnimationActive={false}
        />
      </LineChart>
    </div>
  )
}

// ── Worker Dashboard ────────────────────────────────────────────────────────────

function WorkerDashboard() {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)

  const { data: myTicketsData, isLoading } = useQuery({
    queryKey: ['dashboard', 'worker-tickets', user?.id],
    queryFn: () => ticketService.listar({ solicitanteId: user?.id, tamanoPagina: 100 }),
    enabled: !!user?.id,
  })

  const myTickets = (myTicketsData?.items ?? []).map(mapTicket)
  const myOpen = myTickets.filter((t) => t.status !== 'cerrado')
  const myInProgress = myTickets.filter((t) => t.status === 'en_proceso')
  const myClosed = myTickets.filter((t) => t.status === 'cerrado')
  const myCritical = myTickets.filter((t) => t.priority === 'critica' && t.status !== 'cerrado')
  const recentTickets = myTickets
    .slice()
    .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
    .slice(0, 5)

  if (isLoading) return <DashboardSkeleton />

  return (
    <div className="space-y-4 p-3 lg:p-4">
      <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-base font-semibold tracking-tight">Hola, {user?.nombre}</h2>
          <p className="text-xs text-muted-foreground">
            Aquí tienes el resumen de tus tickets de hoy.
          </p>
        </div>
        <Button onClick={() => navigate(ROUTES.TICKETS_NEW)} className="mt-3 sm:mt-0">
          <Plus className="mr-2 h-4 w-4" />
          Nuevo ticket
        </Button>
      </div>

      <div className="grid grid-cols-2 gap-3 lg:grid-cols-4">
        <StatCard
          title="Mis tickets abiertos"
          value={myOpen.length}
          icon={Ticket}
          iconColor="bg-blue-500/20 text-blue-400"
          onClick={() => navigate(ROUTES.TICKETS)}
        />
        <StatCard
          title="En proceso"
          value={myInProgress.length}
          icon={Clock}
          iconColor="bg-orange-500/20 text-orange-400"
          onClick={() => navigate(ROUTES.TICKETS)}
        />
        <StatCard
          title="Cerrados"
          value={myClosed.length}
          description="histórico total"
          icon={CheckCircle2}
          iconColor="bg-green-500/20 text-green-400"
        />
        <StatCard
          title="Críticos abiertos"
          value={myCritical.length}
          icon={AlertTriangle}
          iconColor={
            myCritical.length > 0 ? 'bg-red-500/20 text-red-400' : 'bg-muted text-muted-foreground'
          }
          onClick={myCritical.length > 0 ? () => navigate(ROUTES.TICKETS) : undefined}
        />
      </div>

      <Card>
        <CardHeader className="flex-row items-center justify-between pb-3">
          <div>
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Mis tickets recientes
            </CardTitle>
            <CardDescription>Últimos tickets registrados o asignados</CardDescription>
          </div>
          <Button variant="ghost" size="sm" onClick={() => navigate(ROUTES.TICKETS)}>
            Ver todos
            <ArrowRight className="ml-1 h-3.5 w-3.5" />
          </Button>
        </CardHeader>
        <CardContent className="p-0">
          {recentTickets.length === 0 ? (
            <div className="flex flex-col items-center justify-center gap-2 py-10 text-center">
              <Ticket className="h-8 w-8 text-muted-foreground/40" />
              <p className="text-sm text-muted-foreground">Aún no tienes tickets registrados.</p>
              <Button size="sm" onClick={() => navigate(ROUTES.TICKETS_NEW)}>
                <Plus className="mr-1.5 h-3.5 w-3.5" />
                Crear tu primer ticket
              </Button>
            </div>
          ) : (
            <div className="divide-y">
              {recentTickets.map((ticket) => (
                <button
                  key={ticket.id}
                  className="flex w-full items-start gap-3 px-4 py-2.5 text-left transition-colors hover:bg-muted/50"
                  onClick={() => navigate(ticketDetailPath(ticket.id))}
                >
                  <div className="min-w-0 flex-1">
                    <div className="flex items-center gap-2">
                      <span className="shrink-0 font-mono text-xs text-muted-foreground">
                        {ticket.code}
                      </span>
                      <PriorityBadge priority={ticket.priority} showIcon={false} />
                    </div>
                    <p className="mt-0.5 truncate text-sm font-medium">{ticket.title}</p>
                    <p className="text-xs text-muted-foreground">
                      {ticket.sucursal} · {ticket.area}
                    </p>
                  </div>
                  <StatusBadge status={ticket.status} />
                </button>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}

// ── Admin Dashboard ─────────────────────────────────────────────────────────────

function AdminDashboard() {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const isAdmin = user?.rol === 'admin' || user?.rol === 'superadmin'
  const [widgetOrder, setWidgetOrder] = useState<string[]>(getSavedOrder)
  const [editMode, setEditMode] = useState(false)
  const [selectedSucursalId, setSelectedSucursalId] = useState<string>('general')
  const [selectedAreaId, setSelectedAreaId] = useState<string>('general')

  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 8 } }))

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event
    if (!over || active.id === over.id) return
    const oldIdx = widgetOrder.indexOf(active.id as string)
    const newIdx = widgetOrder.indexOf(over.id as string)
    const newOrder = arrayMove(widgetOrder, oldIdx, newIdx)
    setWidgetOrder(newOrder)
    sessionStorage.setItem('ps-dashboard-order', JSON.stringify(newOrder))
  }

  // ── Datos del backend (un único endpoint de analytics) ────────────────────────

  const sucursalParam = selectedSucursalId !== 'general' ? selectedSucursalId : undefined
  const {
    data: resumen,
    isLoading,
    error: resumenError,
  } = useDashboardResumen(sucursalParam ? { sucursalId: sucursalParam } : undefined)

  // Tickets recientes (lista separada para la tabla inferior)
  const { data: recientesData } = useQuery({
    queryKey: ['dashboard', 'recientes', sucursalParam],
    queryFn: () =>
      ticketService.listar({
        tamanoPagina: 10,
        ...(sucursalParam ? { sucursalId: sucursalParam } : {}),
      }),
  })
  const recentTickets = (recientesData?.items ?? []).map(mapTicket)

  if (isLoading) return <DashboardSkeleton />
  if (resumenError || !resumen)
    return (
      <div className="flex flex-1 flex-col items-center justify-center gap-2 py-20 text-center">
        <p className="text-sm font-medium text-destructive">No se pudo cargar el dashboard.</p>
        <p className="text-xs text-muted-foreground">
          {resumenError instanceof Error
            ? resumenError.message
            : 'Error al obtener los datos analíticos.'}
        </p>
      </div>
    )

  // ── KPIs del backend ──────────────────────────────────────────────────────────

  const totalAbiertos = resumen.totalAbiertos
  const criticos = resumen.criticos
  const cerradosHoy = resumen.cerradosHoy
  const tasaResolucion = resumen.tasaResolucionPct

  // Contadores de estado para el mini-row
  const getEstado = (estado: string) =>
    resumen.porEstado.find((e) => e.estado === estado)?.total ?? 0

  // ── Opciones de filtro derivadas del backend ──────────────────────────────────

  const sucursalOptions = resumen.porSucursal.map((s) => ({
    id: s.sucursalId,
    name: s.sucursalNombre,
  }))

  const areaOptions =
    selectedSucursalId !== 'general'
      ? resumen.porArea
          .filter((a) => a.sucursalId === selectedSucursalId)
          .map((a) => ({ id: a.areaId, name: a.areaNombre }))
      : []

  function handleSucursalChange(val: string) {
    setSelectedSucursalId(val)
    setSelectedAreaId('general')
  }

  // ── Datos para gráficos — directo del backend ─────────────────────────────────

  const statusData = (resumen.porEstado ?? [])
    .filter((e) => e != null && e.estado != null)
    .map((e) => ({
      name: estadoLabel(e.estado),
      value: e.total,
      color: ESTADO_COLOR_MAP[e.estado] ?? '#6b7280',
      status: String(e.estado).toLowerCase(),
    }))

  const priorityData = (resumen.porPrioridad ?? [])
    .filter((p) => p != null && p.prioridad != null)
    .map((p) => ({
      name: prioridadLabel(p.prioridad),
      value: p.total,
      color: PRIORIDAD_COLOR_MAP[p.prioridad] ?? '#6b7280',
      priority: String(p.prioridad).toLowerCase(),
    }))

  const sucursalData = resumen.porSucursal.map((s) => ({
    name:
      s.sucursalNombre.length > 12 ? s.sucursalNombre.substring(0, 12) + '...' : s.sucursalNombre,
    fullName: s.sucursalNombre,
    value: s.total,
    sucursalId: s.sucursalId,
  }))

  // Filtro local por área en el gráfico de distribución por área
  const areaFiltrada =
    selectedAreaId !== 'general'
      ? resumen.porArea.filter((a) => a.areaId === selectedAreaId)
      : resumen.porArea

  const areasData = areaFiltrada.map((a) => ({
    name: a.areaNombre.length > 14 ? a.areaNombre.substring(0, 14) + '.' : a.areaNombre,
    abiertos: a.abiertos,
    cerrados: a.cerrados,
  }))

  const tiposData = resumen.porTipoServicio.map((t, i) => ({
    name:
      t.tipoServicioNombre.length > 16
        ? t.tipoServicioNombre.substring(0, 16) + '.'
        : t.tipoServicioNombre,
    value: t.total,
    color: TIPO_COLORS[i % TIPO_COLORS.length],
  }))

  const responsableData = resumen.porTecnico.map((t) => ({
    name: t.tecnicoNombre.split(' ')[0] ?? t.tecnicoNombre,
    fullName: t.tecnicoNombre,
    total: t.total,
  }))

  // Radar por sucursal (datos del backend)
  const radarKeys = resumen.porSucursal.slice(0, 4).map((s) => {
    const parts = s.sucursalNombre.split(' ')
    return parts.length > 1 ? parts[1] : parts[0]
  })
  const radarColors = ['#3b82f6', '#f97316', '#22c55e', '#8b5cf6']
  const radarData = ['Sin asignar', 'En proceso', 'Cerrados', 'Críticos'].map((metric) => {
    const entry: Record<string, string | number> = { metric }
    resumen.porSucursal.slice(0, 4).forEach((s, idx) => {
      const key = radarKeys[idx]
      const areasSuc = resumen.porArea.filter((a) => a.sucursalId === s.sucursalId)
      if (metric === 'Sin asignar') entry[key] = getEstado('SIN_ASIGNAR')
      if (metric === 'En proceso') entry[key] = getEstado('EN_PROCESO')
      if (metric === 'Cerrados') entry[key] = areasSuc.reduce((acc, a) => acc + a.cerrados, 0)
      if (metric === 'Críticos') entry[key] = resumen.criticos
    })
    return entry
  })

  // ── Datos de tendencias del backend ───────────────────────────────────────────

  const trendData = resumen.tendencia16Dias
  const weeklyData = resumen.tendenciaSemanal

  // ── Sparklines del backend ────────────────────────────────────────────────────

  const sparkTrend = resumen.sparkAbiertos
  const sparkCriticos = resumen.sparkCriticos
  const sparkCerrados = resumen.sparkCerrados
  const sparkTasa = resumen.sparkAbiertos.map((v, i) => {
    const cerr = resumen.sparkCerrados[i] ?? 0
    return v + cerr > 0 ? Math.round((cerr / (v + cerr)) * 100) : 0
  })

  // ── Definición de widgets ─────────────────────────────────────────────────────

  const WIDGET_DEFS: Record<
    string,
    {
      title: string
      description?: string
      colSpan: number
      viewAllPath?: string
      chart: React.ReactNode
    }
  > = {
    status: {
      title: 'Estado del sistema',
      description: 'Distribución por estado actual',
      colSpan: 1,
      viewAllPath: ROUTES.TICKETS,
      chart: (
        <ResponsiveContainer width="100%" height={220}>
          <PieChart>
            <Pie
              data={statusData}
              dataKey="value"
              innerRadius={55}
              outerRadius={85}
              paddingAngle={2}
              onClick={(data: { status: string }) =>
                navigate(`${ROUTES.TICKETS}?status=${data.status}`)
              }
              className="cursor-pointer"
            >
              {statusData.map((entry, idx) => (
                <Cell key={idx} fill={entry.color} />
              ))}
            </Pie>
            <Tooltip contentStyle={TOOLTIP_STYLE} />
            <Legend
              iconSize={8}
              iconType="circle"
              wrapperStyle={{ fontSize: 10, color: '#6b7280' }}
            />
          </PieChart>
        </ResponsiveContainer>
      ),
    },

    priority: {
      title: 'Por prioridad',
      description: 'Tickets activos por nivel de prioridad',
      colSpan: 1,
      viewAllPath: ROUTES.TICKETS,
      chart: (
        <ResponsiveContainer width="100%" height={220}>
          <BarChart data={priorityData} margin={{ top: 4, right: 8, bottom: 4, left: -20 }}>
            <CartesianGrid stroke={GRID_COLOR} strokeDasharray="3 3" vertical={false} />
            <XAxis dataKey="name" tick={TICK_STYLE} tickLine={false} axisLine={false} />
            <YAxis tick={TICK_STYLE} tickLine={false} axisLine={false} />
            <Tooltip contentStyle={TOOLTIP_STYLE} />
            <Bar
              dataKey="value"
              name="Tickets"
              radius={[4, 4, 0, 0]}
              onClick={(data: { priority: string }) =>
                navigate(`${ROUTES.TICKETS}?priority=${data.priority}`)
              }
              className="cursor-pointer"
            >
              {priorityData.map((entry, idx) => (
                <Cell key={idx} fill={entry.color} />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      ),
    },

    trend: {
      title: 'Tendencia 16 días',
      description: 'Tickets creados vs. resueltos',
      colSpan: 2,
      viewAllPath: ROUTES.REPORTS,
      chart: (
        <ResponsiveContainer width="100%" height={200}>
          <AreaChart data={trendData} margin={{ top: 4, right: 8, bottom: 0, left: -20 }}>
            <defs>
              <linearGradient id="gradCreados" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor={COLORS.primary} stopOpacity={0.25} />
                <stop offset="95%" stopColor={COLORS.primary} stopOpacity={0} />
              </linearGradient>
              <linearGradient id="gradResueltos" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor={COLORS.success} stopOpacity={0.25} />
                <stop offset="95%" stopColor={COLORS.success} stopOpacity={0} />
              </linearGradient>
            </defs>
            <CartesianGrid stroke={GRID_COLOR} strokeDasharray="3 3" />
            <XAxis
              dataKey="fecha"
              tick={TICK_STYLE}
              tickLine={false}
              axisLine={false}
              interval={3}
            />
            <YAxis tick={TICK_STYLE} tickLine={false} axisLine={false} />
            <Tooltip contentStyle={TOOLTIP_STYLE} />
            <Legend iconSize={8} wrapperStyle={{ fontSize: 10, color: '#6b7280' }} />
            <Area
              type="monotone"
              dataKey="creados"
              name="Creados"
              stroke={COLORS.primary}
              strokeWidth={2}
              fill="url(#gradCreados)"
              dot={false}
            />
            <Area
              type="monotone"
              dataKey="resueltos"
              name="Resueltos"
              stroke={COLORS.success}
              strokeWidth={2}
              fill="url(#gradResueltos)"
              dot={false}
              strokeDasharray="4 2"
            />
          </AreaChart>
        </ResponsiveContainer>
      ),
    },

    sucursal: {
      title: 'Por empresa',
      description: 'Total de tickets por empresa activa',
      colSpan: 1,
      viewAllPath: ROUTES.TICKETS,
      chart: (
        <ResponsiveContainer width="100%" height={220}>
          <BarChart
            data={sucursalData}
            layout="vertical"
            margin={{ top: 4, right: 16, bottom: 4, left: 4 }}
          >
            <CartesianGrid stroke={GRID_COLOR} strokeDasharray="3 3" horizontal={false} />
            <XAxis type="number" tick={TICK_STYLE} tickLine={false} axisLine={false} />
            <YAxis
              dataKey="name"
              type="category"
              tick={TICK_STYLE}
              tickLine={false}
              axisLine={false}
              width={72}
            />
            <Tooltip contentStyle={TOOLTIP_STYLE} />
            <Bar
              dataKey="value"
              name="Tickets"
              fill={COLORS.primary}
              radius={[0, 4, 4, 0]}
              onClick={(data: { sucursalId: string }) =>
                navigate(`${ROUTES.TICKETS}?sucursal=${data.sucursalId}`)
              }
              className="cursor-pointer"
            />
          </BarChart>
        </ResponsiveContainer>
      ),
    },

    areas: {
      title: 'Distribución por sucursal',
      description: 'Abiertos vs. cerrados por sucursal',
      colSpan: 2,
      viewAllPath: ROUTES.TICKETS,
      chart: (
        <ResponsiveContainer width="100%" height={200}>
          <BarChart
            data={areasData}
            layout="vertical"
            margin={{ top: 4, right: 16, bottom: 4, left: 4 }}
          >
            <CartesianGrid stroke={GRID_COLOR} strokeDasharray="3 3" horizontal={false} />
            <XAxis type="number" tick={TICK_STYLE} tickLine={false} axisLine={false} />
            <YAxis
              dataKey="name"
              type="category"
              tick={TICK_STYLE}
              tickLine={false}
              axisLine={false}
              width={88}
            />
            <Tooltip contentStyle={TOOLTIP_STYLE} />
            <Legend iconSize={8} wrapperStyle={{ fontSize: 10, color: '#6b7280' }} />
            <Bar
              dataKey="abiertos"
              name="Abiertos"
              fill={COLORS.alta}
              radius={[0, 2, 2, 0]}
              stackId="a"
            />
            <Bar
              dataKey="cerrados"
              name="Cerrados"
              fill={COLORS.success}
              radius={[0, 4, 4, 0]}
              stackId="a"
            />
          </BarChart>
        </ResponsiveContainer>
      ),
    },

    tipos: {
      title: 'Por tipo de servicio',
      description: 'Distribución por categoría',
      colSpan: 1,
      viewAllPath: ROUTES.TICKETS,
      chart: (
        <ResponsiveContainer width="100%" height={220}>
          <PieChart>
            <Pie
              data={tiposData}
              dataKey="value"
              outerRadius={80}
              paddingAngle={2}
              onClick={() => navigate(ROUTES.TICKETS)}
              className="cursor-pointer"
            >
              {tiposData.map((entry, idx) => (
                <Cell key={idx} fill={entry.color} />
              ))}
            </Pie>
            <Tooltip contentStyle={TOOLTIP_STYLE} />
            <Legend
              iconSize={8}
              iconType="circle"
              wrapperStyle={{ fontSize: 9, color: '#6b7280' }}
            />
          </PieChart>
        </ResponsiveContainer>
      ),
    },

    responsable: {
      title: 'Por responsable',
      description: 'Tickets activos asignados por técnico',
      colSpan: 1,
      viewAllPath: ROUTES.USERS,
      chart: (
        <ResponsiveContainer width="100%" height={220}>
          <BarChart
            data={responsableData}
            layout="vertical"
            margin={{ top: 4, right: 16, bottom: 4, left: 4 }}
          >
            <CartesianGrid stroke={GRID_COLOR} strokeDasharray="3 3" horizontal={false} />
            <XAxis type="number" tick={TICK_STYLE} tickLine={false} axisLine={false} />
            <YAxis
              dataKey="name"
              type="category"
              tick={TICK_STYLE}
              tickLine={false}
              axisLine={false}
              width={64}
            />
            <Tooltip contentStyle={TOOLTIP_STYLE} />
            <Bar dataKey="total" name="Asignados" fill={COLORS.asignado} radius={[0, 4, 4, 0]} />
          </BarChart>
        </ResponsiveContainer>
      ),
    },

    radar: {
      title: 'Comparativa empresas',
      description: 'Métricas normalizadas por empresa',
      colSpan: 1,
      viewAllPath: ROUTES.REPORTS,
      chart: (
        <ResponsiveContainer width="100%" height={220}>
          <RadarChart data={radarData} margin={{ top: 8, right: 16, bottom: 8, left: 16 }}>
            <PolarGrid stroke={GRID_COLOR} />
            <PolarAngleAxis dataKey="metric" tick={TICK_STYLE} />
            <Tooltip contentStyle={TOOLTIP_STYLE} />
            {radarKeys.map((key, idx) => (
              <Radar
                key={key}
                name={key}
                dataKey={key}
                stroke={radarColors[idx % radarColors.length]}
                fill={radarColors[idx % radarColors.length]}
                fillOpacity={0.12}
                strokeWidth={1.5}
              />
            ))}
            <Legend iconSize={8} wrapperStyle={{ fontSize: 10, color: '#6b7280' }} />
          </RadarChart>
        </ResponsiveContainer>
      ),
    },

    semanal: {
      title: 'Tendencia semanal',
      description: 'Barras = creados · Línea = resueltos acumulado',
      colSpan: 1,
      viewAllPath: ROUTES.REPORTS,
      chart: (
        <ResponsiveContainer width="100%" height={220}>
          <ComposedChart data={weeklyData} margin={{ top: 4, right: 8, bottom: 4, left: -20 }}>
            <CartesianGrid stroke={GRID_COLOR} strokeDasharray="3 3" vertical={false} />
            <XAxis dataKey="semana" tick={TICK_STYLE} tickLine={false} axisLine={false} />
            <YAxis tick={TICK_STYLE} tickLine={false} axisLine={false} />
            <Tooltip contentStyle={TOOLTIP_STYLE} />
            <Legend iconSize={8} wrapperStyle={{ fontSize: 10, color: '#6b7280' }} />
            <Bar
              dataKey="creados"
              name="Creados"
              fill={COLORS.primary}
              radius={[4, 4, 0, 0]}
              opacity={0.85}
            />
            <Line
              type="monotone"
              dataKey="resueltos"
              name="Resueltos"
              stroke={COLORS.success}
              strokeWidth={2}
              dot={{ fill: COLORS.success, r: 3 }}
            />
          </ComposedChart>
        </ResponsiveContainer>
      ),
    },
  }

  // ── KPI Cards ─────────────────────────────────────────────────────────────────

  const kpiCards = [
    {
      label: 'Total abiertos',
      value: totalAbiertos,
      icon: Ticket,
      color: 'text-blue-400',
      bg: 'bg-blue-500/15',
      sparkData: sparkTrend,
      sparkColor: COLORS.primary,
    },
    {
      label: 'Críticos',
      value: criticos,
      icon: AlertTriangle,
      color: criticos > 0 ? 'text-red-400' : 'text-muted-foreground',
      bg: criticos > 0 ? 'bg-red-500/15' : 'bg-muted/40',
      sparkData: sparkCriticos,
      sparkColor: COLORS.critica,
    },
    {
      label: 'Cerrados hoy',
      value: cerradosHoy,
      icon: CheckCheck,
      color: 'text-green-400',
      bg: 'bg-green-500/15',
      sparkData: sparkCerrados,
      sparkColor: COLORS.success,
    },
    {
      label: 'Tasa resolución',
      value: tasaResolucion,
      suffix: '%',
      icon: AlertCircle,
      color: 'text-amber-400',
      bg: 'bg-amber-500/15',
      sparkData: sparkTasa,
      sparkColor: COLORS.pendiente_validacion,
    },
  ]

  return (
    <div className="space-y-4 p-3 lg:p-4">
      {/* Header */}
      <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-base font-semibold tracking-tight">Panel de Administración</h2>
          <p className="text-xs text-muted-foreground">
            {new Date().toLocaleDateString('es-PE', {
              weekday: 'long',
              year: 'numeric',
              month: 'long',
              day: 'numeric',
            })}
          </p>
        </div>
        <div className="flex flex-wrap items-center gap-2">
          {isAdmin && (
            <>
              {/* Filtro por empresa (sucursal en BD) */}
              <Select value={selectedSucursalId} onValueChange={handleSucursalChange}>
                <SelectTrigger className="h-9 w-auto min-w-[140px] text-xs">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="general">General (todas)</SelectItem>
                  {sucursalOptions.map((s) => (
                    <SelectItem key={s.id} value={s.id}>
                      {s.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {/* Filtro por sucursal (área en BD), activo solo si hay empresa seleccionada */}
              <Select
                value={selectedAreaId}
                onValueChange={setSelectedAreaId}
                disabled={selectedSucursalId === 'general'}
              >
                <SelectTrigger className="h-9 w-auto min-w-[140px] text-xs">
                  <SelectValue
                    placeholder={
                      selectedSucursalId === 'general' ? 'Todas las sucursales' : 'Todas'
                    }
                  />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="general">Todas las sucursales</SelectItem>
                  {areaOptions.map((a) => (
                    <SelectItem key={a.id} value={a.id}>
                      {a.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </>
          )}
          <Button
            variant={editMode ? 'default' : 'outline'}
            size="sm"
            onClick={() => setEditMode((v) => !v)}
          >
            <BarChart3 className="mr-1.5 h-3.5 w-3.5" />
            {editMode ? 'Guardar orden' : 'Editar orden'}
          </Button>
          <Button size="sm" onClick={() => navigate(ROUTES.TICKETS_NEW)}>
            <Plus className="mr-1.5 h-4 w-4" />
            Nuevo ticket
          </Button>
        </div>
      </div>

      {/* KPI mini cards */}
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
        {kpiCards.map((kpi) => {
          const Icon = kpi.icon
          return (
            <Card key={kpi.label} className="ps-glow-card border-border/60">
              <CardContent className="p-3">
                <div className="flex items-center justify-between">
                  <p className="text-[10px] font-semibold uppercase tracking-widest text-muted-foreground">
                    {kpi.label}
                  </p>
                  <div className={cn('rounded p-1', kpi.bg)}>
                    <Icon className={cn('h-3 w-3', kpi.color)} />
                  </div>
                </div>
                <div className="mt-1.5 flex items-end justify-between">
                  <p className="text-2xl font-semibold tabular-nums">
                    {kpi.value}
                    {'suffix' in kpi && kpi.suffix && (
                      <span className="ml-0.5 text-sm font-normal text-muted-foreground">
                        {kpi.suffix}
                      </span>
                    )}
                  </p>
                  <Sparkline data={kpi.sparkData} color={kpi.sparkColor} />
                </div>
              </CardContent>
            </Card>
          )
        })}
      </div>

      {/* Status rápido — mini row */}
      <div className="grid grid-cols-3 gap-2 lg:grid-cols-6">
        {[
          {
            label: 'Sin asignar',
            value: getEstado('SIN_ASIGNAR'),
            icon: Inbox,
            color: 'text-slate-400',
            status: 'sin_asignar',
          },
          {
            label: 'Asignados',
            value: getEstado('ASIGNADO'),
            icon: CheckCircle2,
            color: 'text-blue-400',
            status: 'asignado',
          },
          {
            label: 'En proceso',
            value: getEstado('EN_PROCESO'),
            icon: Clock,
            color: 'text-orange-400',
            status: 'en_proceso',
          },
          {
            label: 'Pend. valid.',
            value: getEstado('PENDIENTE_VALIDACION'),
            icon: AlertCircle,
            color: 'text-amber-400',
            status: 'pendiente_validacion',
          },
          {
            label: 'Cerrados',
            value: getEstado('CERRADO'),
            icon: CheckCheck,
            color: 'text-green-400',
            status: 'cerrado',
          },
          {
            label: 'Reabiertos',
            value: getEstado('REABIERTO'),
            icon: RotateCcw,
            color: 'text-red-400',
            status: 'reabierto',
          },
        ].map((s) => {
          const Icon = s.icon
          return (
            <button
              key={s.label}
              className="rounded-lg border border-border/50 bg-card p-2.5 text-left transition-colors hover:bg-muted/40"
              onClick={() => navigate(`${ROUTES.TICKETS}?status=${s.status}`)}
            >
              <Icon className={cn('mb-1 h-3.5 w-3.5', s.color)} />
              <p className="text-lg font-semibold tabular-nums">{s.value}</p>
              <p className="text-[10px] text-muted-foreground">{s.label}</p>
            </button>
          )
        })}
      </div>

      {/* Grid de widgets con Drag & Drop */}
      <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
        <SortableContext items={widgetOrder} strategy={rectSortingStrategy}>
          <div className="grid grid-cols-1 gap-3 lg:grid-cols-2">
            {widgetOrder.map((id) => {
              const widget = WIDGET_DEFS[id]
              if (!widget) return null
              return (
                <SortableWidget key={id} id={id} colSpan={widget.colSpan} editMode={editMode}>
                  <ChartCard
                    title={widget.title}
                    description={widget.description}
                    viewAllPath={widget.viewAllPath}
                  >
                    {widget.chart}
                  </ChartCard>
                </SortableWidget>
              )
            })}
          </div>
        </SortableContext>
      </DndContext>

      {/* Tickets recientes del sistema */}
      <Card>
        <CardHeader className="flex-row items-center justify-between pb-3">
          <div>
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Tickets recientes
            </CardTitle>
            <CardDescription>Últimos 10 tickets del sistema</CardDescription>
          </div>
          <Button variant="ghost" size="sm" onClick={() => navigate(ROUTES.TICKETS)}>
            Gestionar todos
            <ArrowRight className="ml-1 h-3.5 w-3.5" />
          </Button>
        </CardHeader>
        <CardContent className="p-0">
          <div className="divide-y">
            {recentTickets.map((ticket) => (
              <button
                key={ticket.id}
                className="flex w-full items-start gap-3 px-4 py-2.5 text-left transition-colors hover:bg-muted/50"
                onClick={() => navigate(ticketDetailPath(ticket.id))}
              >
                <div className="min-w-0 flex-1">
                  <div className="flex flex-wrap items-center gap-2">
                    <span className="shrink-0 font-mono text-xs text-muted-foreground">
                      {ticket.code}
                    </span>
                    <PriorityBadge priority={ticket.priority} showIcon />
                  </div>
                  <p className="mt-0.5 truncate text-sm font-medium">{ticket.title}</p>
                  <p className="text-xs text-muted-foreground">
                    {ticket.sucursal} · {ticket.area}
                    {ticket.assignedTo && ` · ${ticket.assignedTo.fullName}`}
                  </p>
                </div>
                <StatusBadge status={ticket.status} />
              </button>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

// ── Helpers de etiquetas ──────────────────────────────────────────────────────

function estadoLabel(estado: string): string {
  const labels: Record<string, string> = {
    SIN_ASIGNAR: 'Sin asignar',
    ASIGNADO: 'Asignados',
    EN_PROCESO: 'En proceso',
    PENDIENTE_VALIDACION: 'Pend. validación',
    CERRADO: 'Cerrados',
    REABIERTO: 'Reabiertos',
    EN_ESPERA: 'En espera',
    CANCELADO: 'Cancelados',
  }
  return labels[estado] ?? estado
}

function prioridadLabel(prioridad: string): string {
  const labels: Record<string, string> = {
    BAJA: 'Baja',
    MEDIA: 'Media',
    ALTA: 'Alta',
    CRITICA: 'Crítica',
  }
  return labels[prioridad] ?? prioridad
}

// ── Entry Point ─────────────────────────────────────────────────────────────────

export function DashboardPage() {
  const user = useAuthStore((s) => s.user)
  const isAdmin = user?.rol === 'admin' || user?.rol === 'superadmin'
  return isAdmin ? <AdminDashboard /> : <WorkerDashboard />
}
