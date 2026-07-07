import { useState } from 'react'
import {
  FileBarChart,
  Download,
  Calendar,
  TrendingUp,
  AlertTriangle,
  CheckCircle2,
} from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@shared/ui/button'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { Badge } from '@shared/ui/badge'
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
} from 'recharts'

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

  const handleDownload = (reportTitle: string) => {
    if (downloading) return
    setDownloading(reportTitle)
    const promise = new Promise<void>((resolve) => {
      setTimeout(() => {
        resolve()
      }, 1500)
    }).finally(() => {
      setDownloading(null)
    })
    toast.promise(promise, {
      loading: 'Generando reporte...',
      success: 'Reporte descargado',
      error: 'Error al generar el reporte',
    })
  }

  return (
    <div className="space-y-4 p-3 lg:p-5">
      {/* Header */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-base font-semibold tracking-tight">Reportes</h1>
          <p className="text-xs text-muted-foreground">
            Análisis y métricas del sistema de tickets.
          </p>
        </div>
        <Button
          variant="outline"
          disabled={downloading !== null}
          onClick={() => handleDownload('exportar-datos')}
        >
          <Download className="mr-2 h-4 w-4" />
          Exportar datos
        </Button>
      </div>

      {/* Charts row */}
      <div className="grid gap-4 lg:grid-cols-3">
        {/* Bar chart by sucursal */}
        <Card className="lg:col-span-2">
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Tickets por empresa
            </CardTitle>
            <CardDescription>Comparativo resueltos vs. pendientes</CardDescription>
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
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Por prioridad
            </CardTitle>
            <CardDescription>Distribución total</CardDescription>
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
          <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            Tickets por tipo de servicio
          </CardTitle>
          <CardDescription>Histograma de categorías más frecuentes</CardDescription>
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
                >
                  <FileBarChart className="h-3.5 w-3.5" />
                </Button>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    </div>
  )
}
