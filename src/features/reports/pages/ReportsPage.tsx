import { useState, useMemo } from 'react'
import {
  FileBarChart,
  Download,
  Calendar,
  TrendingUp,
  AlertTriangle,
  CheckCircle2,
  Filter,
  X,
  ArrowRight,
  RefreshCw,
} from 'lucide-react'
import { toast } from 'sonner'
import {
  exportarDatosGeneralesPDF,
  exportarReporteMensualPDF,
  exportarRendimientoPDF,
  exportarIncidenciasCriticasPDF,
  exportarCierrePorSucursalPDF,
} from '../utils/reportePDF'
import { Button } from '@shared/ui/button'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { Badge } from '@shared/ui/badge'
import { Input } from '@shared/ui/input'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import {
  ResponsiveContainer,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  PieChart,
  Pie,
  Cell,
  AreaChart,
  Area,
} from 'recharts'

// TODO: conectar con endpoint de analytics cuando esté disponible.
//       Los datos de los gráficos son temporales (placeholders de diseño).
const byType = [
  { tipo: 'Inc. Hardware', cantidad: 5 },
  { tipo: 'Inc. Software', cantidad: 4 },
  { tipo: 'Inc. Red', cantidad: 3 },
  { tipo: 'Solicitud', cantidad: 2 },
  { tipo: 'Mantenimiento', cantidad: 3 },
  { tipo: 'Riesgo', cantidad: 1 },
]

const bySucursal = [
  { sucursal: 'Sede Central', resueltos: 7, pendientes: 5 },
  { sucursal: 'Sucursal Norte', resueltos: 3, pendientes: 2 },
  { sucursal: 'Sucursal Sur', resueltos: 2, pendientes: 3 },
]

const byPriority = [
  { name: 'Baja', value: 2, color: 'hsl(var(--priority-baja))' },
  { name: 'Media', value: 4, color: 'hsl(var(--priority-media))' },
  { name: 'Alta', value: 5, color: 'hsl(var(--priority-alta))' },
  { name: 'Crítica', value: 4, color: 'hsl(var(--priority-critica))' },
]

const byStatus = [
  { name: 'Sin asignar', value: 3, color: 'hsl(var(--ticket-sin-asignar))' },
  { name: 'Asignado', value: 2, color: 'hsl(var(--ticket-asignado))' },
  { name: 'En proceso', value: 4, color: 'hsl(var(--ticket-en-proceso))' },
  { name: 'Pend. validación', value: 2, color: 'hsl(var(--ticket-pendiente))' },
  { name: 'Cerrado', value: 4, color: 'hsl(var(--ticket-cerrado))' },
]

const byWeek = [
  { semana: 'Sem 1', tickets: 3 },
  { semana: 'Sem 2', tickets: 5 },
  { semana: 'Sem 3', tickets: 4 },
  { semana: 'Sem 4', tickets: 7 },
  { semana: 'Sem 5', tickets: 6 },
  { semana: 'Sem 6', tickets: 9 },
]

const AVAILABLE_REPORTS = [
  {
    title: 'Reporte mensual de tickets',
    description: 'Resumen completo de todos los tickets del mes con estadísticas de resolución.',
    icon: Calendar,
    badge: 'Junio 2026',
  },
  {
    title: 'Rendimiento por trabajador',
    description: 'Métricas de tickets resueltos, tiempo promedio y satisfacción por trabajador.',
    icon: TrendingUp,
    badge: 'Disponible',
  },
  {
    title: 'Análisis de incidencias críticas',
    description: 'Detalle de todos los tickets críticos y su tiempo de resolución.',
    icon: AlertTriangle,
    badge: 'Disponible',
  },
  {
    title: 'Índice de cierre por sucursal',
    description:
      'Porcentaje de tickets cerrados por sucursal y empresa en el período seleccionado.',
    icon: CheckCircle2,
    badge: 'Disponible',
  },
]

export function ReportsPage() {
  const [downloading, setDownloading] = useState<string | null>(null)
  const [isRefreshing, setIsRefreshing] = useState(false)

  const handleRefresh = () => {
    setIsRefreshing(true)
    setTimeout(() => setIsRefreshing(false), 800)
  }
  const [filterDesde, setFilterDesde] = useState('')
  const [filterHasta, setFilterHasta] = useState('')
  const [filterEmpresa, setFilterEmpresa] = useState('all')
  const [filterEstado, setFilterEstado] = useState('all')

  const hasFilters = useMemo(
    () =>
      filterDesde !== '' || filterHasta !== '' || filterEmpresa !== 'all' || filterEstado !== 'all',
    [filterDesde, filterHasta, filterEmpresa, filterEstado],
  )

  function handleApplyFilters() {
    toast.success('Filtros aplicados')
  }

  function handleClearFilters() {
    setFilterDesde('')
    setFilterHasta('')
    setFilterEmpresa('all')
    setFilterEstado('all')
    toast.info('Filtros eliminados')
  }

  const GENERADORES: Record<string, () => void> = {
    'Reporte mensual de tickets': exportarReporteMensualPDF,
    'Rendimiento por trabajador': exportarRendimientoPDF,
    'Análisis de incidencias críticas': exportarIncidenciasCriticasPDF,
    'Índice de cierre por sucursal': exportarCierrePorSucursalPDF,
  }

  const handleDownload = (reportTitle: string) => {
    if (downloading) return
    setDownloading(reportTitle)
    const promise = new Promise<void>((resolve, reject) => {
      try {
        const generador = GENERADORES[reportTitle]
        if (generador) {
          generador()
        } else {
          exportarDatosGeneralesPDF({
            desde: filterDesde,
            hasta: filterHasta,
            empresa: filterEmpresa,
            estado: filterEstado,
          })
        }
        resolve()
      } catch (err) {
        reject(err)
      }
    }).finally(() => {
      setDownloading(null)
    })
    toast.promise(promise, {
      loading: 'Generando PDF...',
      success: 'PDF descargado correctamente',
      error: 'Error al generar el PDF',
    })
  }

  return (
    <div className="space-y-4 p-3 lg:p-5">
      {/* Header */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-base font-semibold tracking-tight">Reportes</h2>
          <p className="text-xs text-muted-foreground">
            Análisis y métricas del sistema de tickets.
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8"
            onClick={handleRefresh}
            disabled={isRefreshing}
            title="Actualizar"
          >
            <RefreshCw className={`h-4 w-4 ${isRefreshing ? 'animate-spin' : ''}`} />
            <span className="sr-only">Actualizar</span>
          </Button>
          <Button
            variant="outline"
            disabled={downloading !== null}
            onClick={() => handleDownload('exportar-datos-generales')}
          >
            <Download className="mr-2 h-4 w-4" />
            Exportar datos
          </Button>
        </div>
      </div>

      {/* Filters */}
      <Card>
        <CardHeader className="px-3 pb-2 pt-3">
          <div className="flex items-center gap-2">
            <Filter className="h-3.5 w-3.5 text-muted-foreground" />
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Filtros
            </CardTitle>
            {hasFilters && (
              <button
                onClick={handleClearFilters}
                className="ml-auto flex items-center gap-1 text-xs text-muted-foreground hover:text-foreground"
              >
                <X className="h-3 w-3" />
                Limpiar
              </button>
            )}
          </div>
        </CardHeader>
        <CardContent className="p-3 pt-0">
          <div className="grid grid-cols-2 gap-2 lg:grid-cols-4">
            <div className="flex flex-col gap-1">
              <span className="text-[10px] font-medium text-muted-foreground">Desde</span>
              <Input
                type="date"
                className="h-8 text-xs"
                value={filterDesde}
                onChange={(e) => setFilterDesde(e.target.value)}
              />
            </div>
            <div className="flex flex-col gap-1">
              <span className="text-[10px] font-medium text-muted-foreground">Hasta</span>
              <Input
                type="date"
                className="h-8 text-xs"
                value={filterHasta}
                onChange={(e) => setFilterHasta(e.target.value)}
              />
            </div>
            <div className="flex flex-col gap-1">
              <span className="text-[10px] font-medium text-muted-foreground">Empresa</span>
              <Select value={filterEmpresa} onValueChange={setFilterEmpresa}>
                <SelectTrigger className="h-9 text-xs">
                  <SelectValue placeholder="Todas" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">Todas las empresas</SelectItem>
                  <SelectItem value="s1">Sede Central</SelectItem>
                  <SelectItem value="s2">Sucursal Norte</SelectItem>
                  <SelectItem value="s3">Sucursal Sur</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="flex flex-col gap-1">
              <span className="text-[10px] font-medium text-muted-foreground">Estado</span>
              <Select value={filterEstado} onValueChange={setFilterEstado}>
                <SelectTrigger className="h-9 text-xs">
                  <SelectValue placeholder="Todos" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">Todos los estados</SelectItem>
                  <SelectItem value="sin_asignar">Sin asignar</SelectItem>
                  <SelectItem value="asignado">Asignado</SelectItem>
                  <SelectItem value="en_proceso">En proceso</SelectItem>
                  <SelectItem value="cerrado">Cerrado</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
          <div className="mt-2 flex justify-end">
            <Button size="sm" className="h-7 gap-1.5 text-xs" onClick={handleApplyFilters}>
              <Filter className="h-3 w-3" />
              Aplicar filtros
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Charts row */}
      <div className="grid gap-4 lg:grid-cols-3">
        {/* Bar chart by sucursal */}
        <Card className="lg:col-span-2">
          <CardHeader className="px-3 pb-2 pt-3">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Tickets por empresa
                </CardTitle>
                <CardDescription>Comparativo resueltos vs. pendientes</CardDescription>
              </div>
              <Button
                variant="ghost"
                size="sm"
                className="h-7 gap-1 text-xs text-muted-foreground hover:text-foreground"
              >
                Ver todo
                <ArrowRight className="h-3 w-3" />
              </Button>
            </div>
          </CardHeader>
          <CardContent className="p-3 pt-0">
            <ResponsiveContainer width="100%" height={180}>
              <BarChart data={bySucursal} margin={{ left: -20, right: 8 }}>
                <CartesianGrid strokeDasharray="3 3" className="stroke-border" />
                <XAxis
                  dataKey="sucursal"
                  tick={{ fontSize: 11 }}
                  tickLine={false}
                  axisLine={false}
                />
                <YAxis tick={{ fontSize: 11 }} tickLine={false} axisLine={false} />
                <Tooltip
                  contentStyle={{
                    fontSize: 12,
                    borderRadius: 8,
                    border: '1px solid hsl(var(--border))',
                    background: 'hsl(var(--background))',
                    color: 'hsl(var(--foreground))',
                  }}
                />
                <Legend iconSize={10} wrapperStyle={{ fontSize: 12 }} />
                <Bar
                  dataKey="resueltos"
                  name="Resueltos"
                  fill="hsl(var(--success))"
                  radius={[4, 4, 0, 0]}
                />
                <Bar
                  dataKey="pendientes"
                  name="Pendientes"
                  fill="hsl(var(--primary))"
                  radius={[4, 4, 0, 0]}
                />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>

        {/* Pie chart by priority */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Por prioridad
                </CardTitle>
                <CardDescription>Distribución total</CardDescription>
              </div>
              <Button
                variant="ghost"
                size="sm"
                className="h-7 gap-1 text-xs text-muted-foreground hover:text-foreground"
              >
                Ver toda
                <ArrowRight className="h-3 w-3" />
              </Button>
            </div>
          </CardHeader>
          <CardContent className="flex flex-col items-center p-3 pt-0">
            <ResponsiveContainer width="100%" height={160}>
              <PieChart>
                <Pie
                  data={byPriority}
                  cx="50%"
                  cy="50%"
                  innerRadius={45}
                  outerRadius={70}
                  dataKey="value"
                  paddingAngle={3}
                >
                  {byPriority.map((entry, index) => (
                    <Cell key={index} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip
                  contentStyle={{
                    fontSize: 12,
                    borderRadius: 8,
                    border: '1px solid hsl(var(--border))',
                    background: 'hsl(var(--background))',
                    color: 'hsl(var(--foreground))',
                  }}
                />
              </PieChart>
            </ResponsiveContainer>
            <div className="grid grid-cols-2 gap-x-4 gap-y-1">
              {byPriority.map((p) => (
                <div key={p.name} className="flex items-center gap-1.5">
                  <div className="h-2.5 w-2.5 rounded-sm" style={{ background: p.color }} />
                  <span className="text-xs">
                    {p.name} ({p.value})
                  </span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Bar chart by type */}
      <Card>
        <CardHeader className="px-3 pb-2 pt-3">
          <div className="flex items-center justify-between">
            <div>
              <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                Tickets por tipo de servicio
              </CardTitle>
              <CardDescription>Histograma de categorías más frecuentes</CardDescription>
            </div>
            <Button
              variant="ghost"
              size="sm"
              className="h-7 gap-1 text-xs text-muted-foreground hover:text-foreground"
            >
              Ver todo
              <ArrowRight className="h-3 w-3" />
            </Button>
          </div>
        </CardHeader>
        <CardContent className="p-3 pt-0">
          <ResponsiveContainer width="100%" height={180}>
            <BarChart data={byType} layout="vertical" margin={{ left: 80, right: 16 }}>
              <CartesianGrid strokeDasharray="3 3" horizontal={false} className="stroke-border" />
              <XAxis type="number" tick={{ fontSize: 11 }} tickLine={false} axisLine={false} />
              <YAxis
                dataKey="tipo"
                type="category"
                tick={{ fontSize: 11 }}
                tickLine={false}
                axisLine={false}
                width={80}
              />
              <Tooltip
                contentStyle={{
                  fontSize: 12,
                  borderRadius: 8,
                  border: '1px solid hsl(var(--border))',
                  background: 'hsl(var(--background))',
                  color: 'hsl(var(--foreground))',
                }}
              />
              <Bar
                dataKey="cantidad"
                name="Cantidad"
                fill="hsl(var(--primary))"
                radius={[0, 4, 4, 0]}
              />
            </BarChart>
          </ResponsiveContainer>
        </CardContent>
      </Card>

      {/* Segunda fila de gráficos */}
      <div className="grid gap-4 lg:grid-cols-2">
        {/* Tendencia semanal */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Tendencia semanal
                </CardTitle>
                <CardDescription>Tickets registrados por semana</CardDescription>
              </div>
              <Button
                variant="ghost"
                size="sm"
                className="h-7 gap-1 text-xs text-muted-foreground hover:text-foreground"
              >
                Ver todo
                <ArrowRight className="h-3 w-3" />
              </Button>
            </div>
          </CardHeader>
          <CardContent className="p-3 pt-0">
            <ResponsiveContainer width="100%" height={180}>
              <AreaChart data={byWeek} margin={{ left: -20, right: 8 }}>
                <defs>
                  <linearGradient id="colorTickets" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="hsl(var(--primary))" stopOpacity={0.25} />
                    <stop offset="95%" stopColor="hsl(var(--primary))" stopOpacity={0} />
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" className="stroke-border" />
                <XAxis dataKey="semana" tick={{ fontSize: 11 }} tickLine={false} axisLine={false} />
                <YAxis tick={{ fontSize: 11 }} tickLine={false} axisLine={false} />
                <Tooltip
                  contentStyle={{
                    fontSize: 12,
                    borderRadius: 8,
                    border: '1px solid hsl(var(--border))',
                    background: 'hsl(var(--background))',
                    color: 'hsl(var(--foreground))',
                  }}
                />
                <Area
                  type="monotone"
                  dataKey="tickets"
                  name="Tickets"
                  stroke="hsl(var(--primary))"
                  strokeWidth={2}
                  fill="url(#colorTickets)"
                  dot={{ r: 3, fill: 'hsl(var(--primary))' }}
                />
              </AreaChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>

        {/* Por estado */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Por estado
                </CardTitle>
                <CardDescription>Distribución según estado actual</CardDescription>
              </div>
              <Button
                variant="ghost"
                size="sm"
                className="h-7 gap-1 text-xs text-muted-foreground hover:text-foreground"
              >
                Ver toda
                <ArrowRight className="h-3 w-3" />
              </Button>
            </div>
          </CardHeader>
          <CardContent className="flex flex-col items-center p-3 pt-0">
            <ResponsiveContainer width="100%" height={160}>
              <PieChart>
                <Pie
                  data={byStatus}
                  cx="50%"
                  cy="50%"
                  innerRadius={45}
                  outerRadius={70}
                  dataKey="value"
                  paddingAngle={3}
                >
                  {byStatus.map((entry, index) => (
                    <Cell key={index} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip
                  contentStyle={{
                    fontSize: 12,
                    borderRadius: 8,
                    border: '1px solid hsl(var(--border))',
                    background: 'hsl(var(--background))',
                    color: 'hsl(var(--foreground))',
                  }}
                />
              </PieChart>
            </ResponsiveContainer>
            <div className="grid w-full grid-cols-2 gap-x-4 gap-y-1">
              {byStatus.map((s) => (
                <div key={s.name} className="flex items-center gap-1.5">
                  <div
                    className="h-2.5 w-2.5 shrink-0 rounded-sm"
                    style={{ background: s.color }}
                  />
                  <span className="truncate text-xs">
                    {s.name} ({s.value})
                  </span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Available reports */}
      <div>
        <p className="mb-3 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
          Reportes disponibles
        </p>
        <div className="grid gap-3 sm:grid-cols-2">
          {AVAILABLE_REPORTS.map((report) => (
            <Card
              key={report.title}
              className="cursor-pointer transition-all hover:border-primary/30 hover:shadow-md"
            >
              <CardContent className="flex items-start gap-3 p-3">
                <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-primary/10">
                  <report.icon className="h-4 w-4 text-primary" />
                </div>
                <div className="min-w-0 flex-1">
                  <div className="flex items-center gap-2">
                    <p className="text-xs font-medium">{report.title}</p>
                    <Badge variant="secondary" className="text-[10px]">
                      {report.badge}
                    </Badge>
                  </div>
                  <p className="mt-0.5 text-xs text-muted-foreground">{report.description}</p>
                </div>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 shrink-0"
                  disabled={downloading === report.title}
                  onClick={() => handleDownload(report.title)}
                  aria-label={`Descargar ${report.title}`}
                >
                  <FileBarChart className="h-3.5 w-3.5" aria-hidden="true" />
                </Button>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    </div>
  )
}
