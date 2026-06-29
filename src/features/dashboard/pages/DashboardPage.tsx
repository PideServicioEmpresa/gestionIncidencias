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
} from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { StatusBadge } from '@shared/components/StatusBadge'
import { PriorityBadge } from '@shared/components/PriorityBadge'
import { useAuthStore } from '@store/auth.store'
import {
  MOCK_TICKETS,
  MOCK_TREND_DATA,
  TICKET_STATUS_COUNTS,
  getTicketsByUser,
  MOCK_SUCURSALES,
  MOCK_USERS,
  TICKET_TYPES,
} from '@mocks/data'
import { ROUTES, ticketDetailPath } from '@constants/index'
import { cn } from '@lib/utils'
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
} from 'recharts'
import { DndContext, closestCenter, PointerSensor, useSensor, useSensors } from '@dnd-kit/core'
import type { DragEndEvent } from '@dnd-kit/core'
import { SortableContext, useSortable, arrayMove, rectSortingStrategy } from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'

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
  baja: '#22c55e',
  media: '#f59e0b',
  alta: '#f97316',
  critica: '#ef4444',
  primary: '#3b82f6',
  success: '#22c55e',
} as const

const TICKET_TYPE_COLORS = [
  '#3b82f6',
  '#f97316',
  '#22c55e',
  '#f59e0b',
  '#ef4444',
  '#8b5cf6',
  '#ec4899',
  '#06b6d4',
]

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
}: {
  title: string
  description?: string
  children: React.ReactNode
  className?: string
}) {
  return (
    <Card className={cn('border-border/60 bg-card', className)}>
      <CardHeader className="pb-2">
        <CardTitle className="text-xs font-semibold uppercase tracking-widest text-muted-foreground">
          {title}
        </CardTitle>
        {description && <CardDescription className="text-[10px]">{description}</CardDescription>}
      </CardHeader>
      <CardContent className="pb-4">{children}</CardContent>
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

// ── Worker Dashboard ────────────────────────────────────────────────────────────

function WorkerDashboard() {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const myTickets = user ? getTicketsByUser(user.id) : []
  const myOpen = myTickets.filter((t) => t.status !== 'cerrado')
  const myInProgress = myTickets.filter((t) => t.status === 'en_proceso')
  const myClosed = myTickets.filter((t) => t.status === 'cerrado')
  const myCritical = myTickets.filter((t) => t.priority === 'critica' && t.status !== 'cerrado')
  const recentTickets = myTickets.slice(0, 5)

  return (
    <div className="space-y-4 p-3 lg:p-5">
      {/* Saludo */}
      <div className="flex flex-col gap-1 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-lg font-semibold">Hola, {user?.nombre}</h2>
          <p className="text-sm text-muted-foreground">
            Aquí tienes el resumen de tus tickets de hoy.
          </p>
        </div>
        <Button onClick={() => navigate(ROUTES.TICKETS_NEW)} className="mt-3 sm:mt-0">
          <Plus className="mr-2 h-4 w-4" />
          Nuevo ticket
        </Button>
      </div>

      {/* Stats */}
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

      {/* Tickets recientes */}
      <Card>
        <CardHeader className="flex-row items-center justify-between pb-3">
          <div>
            <CardTitle className="text-base">Mis tickets recientes</CardTitle>
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
  const [widgetOrder, setWidgetOrder] = useState<string[]>(getSavedOrder)
  const [editMode, setEditMode] = useState(false)

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

  // ── Cálculo de KPIs ───────────────────────────────────────────────────────────

  const totalAbiertos =
    TICKET_STATUS_COUNTS.sin_asignar +
    TICKET_STATUS_COUNTS.asignado +
    TICKET_STATUS_COUNTS.en_proceso +
    TICKET_STATUS_COUNTS.pendiente_validacion +
    TICKET_STATUS_COUNTS.reabierto

  const criticos = MOCK_TICKETS.filter(
    (t) => t.priority === 'critica' && t.status !== 'cerrado',
  ).length

  const cerrados = TICKET_STATUS_COUNTS.cerrado
  const tasaResolucion = Math.round((cerrados / MOCK_TICKETS.length) * 100)

  // ── Datos para los gráficos ───────────────────────────────────────────────────

  const statusData = [
    {
      name: 'Sin asignar',
      value: TICKET_STATUS_COUNTS.sin_asignar,
      color: COLORS.sin_asignar,
      status: 'sin_asignar',
    },
    {
      name: 'Asignados',
      value: TICKET_STATUS_COUNTS.asignado,
      color: COLORS.asignado,
      status: 'asignado',
    },
    {
      name: 'En proceso',
      value: TICKET_STATUS_COUNTS.en_proceso,
      color: COLORS.en_proceso,
      status: 'en_proceso',
    },
    {
      name: 'Pend. validación',
      value: TICKET_STATUS_COUNTS.pendiente_validacion,
      color: COLORS.pendiente_validacion,
      status: 'pendiente_validacion',
    },
    {
      name: 'Cerrados',
      value: TICKET_STATUS_COUNTS.cerrado,
      color: COLORS.cerrado,
      status: 'cerrado',
    },
    {
      name: 'Reabiertos',
      value: TICKET_STATUS_COUNTS.reabierto,
      color: COLORS.reabierto,
      status: 'reabierto',
    },
  ]

  const priorityData = [
    {
      name: 'Baja',
      value: MOCK_TICKETS.filter((t) => t.priority === 'baja').length,
      color: COLORS.baja,
      priority: 'baja',
    },
    {
      name: 'Media',
      value: MOCK_TICKETS.filter((t) => t.priority === 'media').length,
      color: COLORS.media,
      priority: 'media',
    },
    {
      name: 'Alta',
      value: MOCK_TICKETS.filter((t) => t.priority === 'alta').length,
      color: COLORS.alta,
      priority: 'alta',
    },
    {
      name: 'Crítica',
      value: MOCK_TICKETS.filter((t) => t.priority === 'critica').length,
      color: COLORS.critica,
      priority: 'critica',
    },
  ]

  const sucursalData = MOCK_SUCURSALES.filter((s) => s.activo).map((s) => ({
    name: s.name.length > 12 ? s.name.substring(0, 12) + '...' : s.name,
    fullName: s.name,
    value: MOCK_TICKETS.filter((t) => t.sucursalId === s.id).length,
    sucursalId: s.id,
  }))

  const uniqueAreas = [...new Set(MOCK_TICKETS.map((t) => t.area))]
  const areasData = uniqueAreas
    .map((area) => ({
      name: area.length > 14 ? area.substring(0, 14) + '.' : area,
      abiertos: MOCK_TICKETS.filter((t) => t.area === area && t.status !== 'cerrado').length,
      cerrados: MOCK_TICKETS.filter((t) => t.area === area && t.status === 'cerrado').length,
    }))
    .filter((a) => a.abiertos + a.cerrados > 0)

  const tiposData = TICKET_TYPES.map((type, i) => ({
    name: type.length > 16 ? type.substring(0, 16) + '.' : type,
    value: MOCK_TICKETS.filter((t) => t.type === type).length,
    color: TICKET_TYPE_COLORS[i % TICKET_TYPE_COLORS.length],
  })).filter((t) => t.value > 0)

  const responsableData = MOCK_USERS.filter((u) => u.rol === 'worker' && u.activo)
    .map((u) => ({
      name: u.name + ' ' + (u.apellido?.charAt(0) ?? '') + '.',
      total: MOCK_TICKETS.filter((t) => t.assignedTo?.id === u.id).length,
    }))
    .filter((r) => r.total > 0)
    .sort((a, b) => b.total - a.total)

  const radarData = ['Sin asignar', 'En proceso', 'Cerrados', 'Críticos'].map((metric) => {
    const entry: Record<string, string | number> = { metric }
    MOCK_SUCURSALES.filter((s) => s.activo).forEach((s) => {
      const tickets = MOCK_TICKETS.filter((t) => t.sucursalId === s.id)
      const parts = s.name.split(' ')
      const shortName = parts.length > 1 ? parts[1] : parts[0]
      if (metric === 'Sin asignar')
        entry[shortName] = tickets.filter((t) => t.status === 'sin_asignar').length
      if (metric === 'En proceso')
        entry[shortName] = tickets.filter((t) => t.status === 'en_proceso').length
      if (metric === 'Cerrados')
        entry[shortName] = tickets.filter((t) => t.status === 'cerrado').length
      if (metric === 'Críticos')
        entry[shortName] = tickets.filter((t) => t.priority === 'critica').length
    })
    return entry
  })

  const radarKeys = MOCK_SUCURSALES.filter((s) => s.activo).map((s) => {
    const parts = s.name.split(' ')
    return parts.length > 1 ? parts[1] : parts[0]
  })
  const radarColors = ['#3b82f6', '#f97316', '#22c55e', '#8b5cf6']

  const weeklyData = [0, 1, 2, 3].map((w) => {
    const week = MOCK_TREND_DATA.slice(w * 4, (w + 1) * 4)
    return {
      semana: `Sem ${w + 1}`,
      creados: week.reduce((s, d) => s + d.creados, 0),
      resueltos: week.reduce((s, d) => s + d.resueltos, 0),
    }
  })

  // ── Definición de widgets ─────────────────────────────────────────────────────

  const WIDGET_DEFS: Record<
    string,
    { title: string; description?: string; colSpan: number; chart: React.ReactNode }
  > = {
    status: {
      title: 'Estado del sistema',
      description: 'Distribución por estado actual',
      colSpan: 1,
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
      description: 'Total de tickets por nivel de prioridad',
      colSpan: 1,
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
      chart: (
        <ResponsiveContainer width="100%" height={200}>
          <AreaChart data={MOCK_TREND_DATA} margin={{ top: 4, right: 8, bottom: 0, left: -20 }}>
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
      title: 'Por sucursal',
      description: 'Total de tickets por sede activa',
      colSpan: 1,
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
      title: 'Distribución por área',
      description: 'Abiertos vs. cerrados por área',
      colSpan: 2,
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
      title: 'Por tipo de solicitud',
      description: 'Distribución por categoría',
      colSpan: 1,
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
      description: 'Tickets asignados por trabajador',
      colSpan: 1,
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
      title: 'Comparativa sucursales',
      description: 'Métricas normalizadas por sede',
      colSpan: 1,
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
    },
    {
      label: 'Críticos',
      value: criticos,
      icon: AlertTriangle,
      color: criticos > 0 ? 'text-red-400' : 'text-muted-foreground',
      bg: criticos > 0 ? 'bg-red-500/15' : 'bg-muted/40',
    },
    {
      label: 'Cerrados hoy',
      value: cerrados,
      icon: CheckCheck,
      color: 'text-green-400',
      bg: 'bg-green-500/15',
    },
    {
      label: 'Tasa resolución',
      value: tasaResolucion,
      suffix: '%',
      icon: AlertCircle,
      color: 'text-amber-400',
      bg: 'bg-amber-500/15',
    },
  ]

  return (
    <div className="space-y-4 p-3 lg:p-5">
      {/* Header */}
      <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-lg font-semibold">Panel de Administración</h2>
          <p className="text-sm text-muted-foreground">
            {new Date().toLocaleDateString('es-PE', {
              weekday: 'long',
              year: 'numeric',
              month: 'long',
              day: 'numeric',
            })}
          </p>
        </div>
        <div className="flex items-center gap-2">
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
            <Card key={kpi.label} className="border-border/60">
              <CardContent className="p-3">
                <div className="flex items-center justify-between">
                  <p className="text-[10px] font-semibold uppercase tracking-widest text-muted-foreground">
                    {kpi.label}
                  </p>
                  <div className={cn('rounded p-1', kpi.bg)}>
                    <Icon className={cn('h-3 w-3', kpi.color)} />
                  </div>
                </div>
                <p className="mt-1.5 text-2xl font-bold tabular-nums">
                  {kpi.value}
                  {kpi.suffix && (
                    <span className="ml-0.5 text-sm font-normal text-muted-foreground">
                      {kpi.suffix}
                    </span>
                  )}
                </p>
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
            value: TICKET_STATUS_COUNTS.sin_asignar,
            icon: Inbox,
            color: 'text-slate-400',
            status: 'sin_asignar',
          },
          {
            label: 'Asignados',
            value: TICKET_STATUS_COUNTS.asignado,
            icon: CheckCircle2,
            color: 'text-blue-400',
            status: 'asignado',
          },
          {
            label: 'En proceso',
            value: TICKET_STATUS_COUNTS.en_proceso,
            icon: Clock,
            color: 'text-orange-400',
            status: 'en_proceso',
          },
          {
            label: 'Pend. valid.',
            value: TICKET_STATUS_COUNTS.pendiente_validacion,
            icon: AlertCircle,
            color: 'text-amber-400',
            status: 'pendiente_validacion',
          },
          {
            label: 'Cerrados',
            value: TICKET_STATUS_COUNTS.cerrado,
            icon: CheckCheck,
            color: 'text-green-400',
            status: 'cerrado',
          },
          {
            label: 'Reabiertos',
            value: TICKET_STATUS_COUNTS.reabierto,
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
              <p className="text-lg font-bold tabular-nums">{s.value}</p>
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
                  <ChartCard title={widget.title} description={widget.description}>
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
            <CardTitle className="text-base">Tickets recientes</CardTitle>
            <CardDescription>Últimos 5 tickets del sistema</CardDescription>
          </div>
          <Button variant="ghost" size="sm" onClick={() => navigate(ROUTES.TICKETS)}>
            Gestionar todos
            <ArrowRight className="ml-1 h-3.5 w-3.5" />
          </Button>
        </CardHeader>
        <CardContent className="p-0">
          <div className="divide-y">
            {MOCK_TICKETS.slice(0, 5).map((ticket) => (
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

// ── Entry Point ─────────────────────────────────────────────────────────────────

export function DashboardPage() {
  const user = useAuthStore((s) => s.user)
  const isAdmin = user?.rol === 'admin' || user?.rol === 'superadmin'
  return isAdmin ? <AdminDashboard /> : <WorkerDashboard />
}
