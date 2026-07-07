import { useState } from 'react'
import {
  AlertCircle,
  AlertTriangle,
  BarChart3,
  Bell,
  BookOpen,
  Building2,
  CheckCircle2,
  ChevronDown,
  Clock,
  Download,
  Eye,
  FileText,
  Filter,
  Flame,
  Home,
  Info,
  LayoutDashboard,
  LogOut,
  Menu,
  Moon,
  MoreHorizontal,
  Plus,
  RefreshCw,
  Search,
  Settings,
  Shield,
  Star,
  Sun,
  TicketCheck,
  Trash2,
  Upload,
  User,
  Users,
  X,
  Zap,
} from 'lucide-react'
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from '@shared/ui/accordion'
import { Alert, AlertDescription, AlertTitle } from '@shared/ui/alert'
import { AlertDialog, AlertDialogTrigger } from '@shared/ui/alert-dialog'
import { Avatar, AvatarFallback, AvatarImage } from '@shared/ui/avatar'
import { Badge } from '@shared/ui/badge'
import { Button } from '@shared/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@shared/ui/card'
import { Checkbox } from '@shared/ui/checkbox'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@shared/ui/dialog'
import { Input } from '@shared/ui/input'
import { Label } from '@shared/ui/label'
import { Progress } from '@shared/ui/progress'
import { RadioGroup, RadioGroupItem } from '@shared/ui/radio-group'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { Separator } from '@shared/ui/separator'
import { Skeleton } from '@shared/ui/skeleton'
import { Switch } from '@shared/ui/switch'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@shared/ui/tabs'
import { Textarea } from '@shared/ui/textarea'
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@shared/ui/tooltip'
import { ConfirmDialog } from '@shared/components/ConfirmDialog'
import { EmptyState } from '@shared/components/EmptyState'
import { PriorityBadge } from '@shared/components/PriorityBadge'
import { Spinner, SpinnerOverlay } from '@shared/components/Spinner'
import { StatusBadge } from '@shared/components/StatusBadge'
import { useTheme } from '@hooks/use-theme'
import type { TicketPriority, TicketStatus } from '@types-app/index'

// ── Constantes ──────────────────────────────────────────────────────────────

const NAV_ITEMS = [
  { href: '#colores', label: 'Colores' },
  { href: '#tipografia', label: 'Tipografía' },
  { href: '#botones', label: 'Botones' },
  { href: '#controles', label: 'Controles' },
  { href: '#tarjetas', label: 'Tarjetas' },
  { href: '#badges', label: 'Badges' },
  { href: '#alertas', label: 'Alertas' },
  { href: '#dialogos', label: 'Diálogos' },
  { href: '#loading', label: 'Loading' },
  { href: '#iconos', label: 'Íconos' },
]

const TICKET_STATUSES: TicketStatus[] = [
  'sin_asignar',
  'asignado',
  'en_proceso',
  'pendiente_validacion',
  'cerrado',
  'reabierto',
]

const TICKET_PRIORITIES: TicketPriority[] = ['baja', 'media', 'alta', 'critica']

const LUCIDE_ICONS = [
  { icon: TicketCheck, name: 'TicketCheck' },
  { icon: FileText, name: 'FileText' },
  { icon: Users, name: 'Users' },
  { icon: User, name: 'User' },
  { icon: Settings, name: 'Settings' },
  { icon: Bell, name: 'Bell' },
  { icon: BarChart3, name: 'BarChart3' },
  { icon: AlertTriangle, name: 'AlertTriangle' },
  { icon: AlertCircle, name: 'AlertCircle' },
  { icon: CheckCircle2, name: 'CheckCircle2' },
  { icon: Clock, name: 'Clock' },
  { icon: Building2, name: 'Building2' },
  { icon: Search, name: 'Search' },
  { icon: Filter, name: 'Filter' },
  { icon: Plus, name: 'Plus' },
  { icon: X, name: 'X' },
  { icon: ChevronDown, name: 'ChevronDown' },
  { icon: MoreHorizontal, name: 'MoreHorizontal' },
  { icon: Trash2, name: 'Trash2' },
  { icon: Download, name: 'Download' },
  { icon: Upload, name: 'Upload' },
  { icon: Eye, name: 'Eye' },
  { icon: RefreshCw, name: 'RefreshCw' },
  { icon: LogOut, name: 'LogOut' },
  { icon: Home, name: 'Home' },
  { icon: LayoutDashboard, name: 'LayoutDashboard' },
  { icon: Shield, name: 'Shield' },
  { icon: Star, name: 'Star' },
  { icon: BookOpen, name: 'BookOpen' },
  { icon: Zap, name: 'Zap' },
  { icon: Flame, name: 'Flame' },
  { icon: Info, name: 'Info' },
  { icon: Menu, name: 'Menu' },
]

// ── Sub-componentes de la página ─────────────────────────────────────────────

function Section({
  id,
  title,
  children,
}: {
  id: string
  title: string
  children: React.ReactNode
}) {
  return (
    <section id={id} className="scroll-mt-28">
      <div className="mb-6">
        <h2 className="text-2xl font-semibold tracking-tight text-foreground">{title}</h2>
        <Separator className="mt-3" />
      </div>
      {children}
    </section>
  )
}

function SubSection({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="space-y-4">
      <h3 className="text-sm font-medium uppercase tracking-widest text-muted-foreground">
        {title}
      </h3>
      {children}
    </div>
  )
}

function DemoBox({ className, children }: { className?: string; children: React.ReactNode }) {
  return <div className={`rounded-xl border bg-card p-6 ${className ?? ''}`}>{children}</div>
}

interface ColorSwatchProps {
  cssVar: string
  foregroundVar?: string
  label: string
  sublabel?: string
}

function ColorSwatch({ cssVar, foregroundVar, label, sublabel }: ColorSwatchProps) {
  return (
    <div className="space-y-1.5">
      <div
        className="flex h-16 flex-col justify-end rounded-lg p-2 shadow-sm ring-1 ring-inset ring-black/5"
        style={{
          background: `hsl(var(${cssVar}))`,
          color: foregroundVar ? `hsl(var(${foregroundVar}))` : undefined,
        }}
      >
        {foregroundVar && <span className="text-xs font-medium">Aa</span>}
      </div>
      <p className="text-xs font-medium text-foreground">{label}</p>
      {sublabel && <p className="font-mono text-xs text-muted-foreground">{sublabel}</p>}
    </div>
  )
}

// ── Página principal ─────────────────────────────────────────────────────────

export function DesignSystemPage() {
  const { theme, setTheme } = useTheme()
  const [dialogOpen, setDialogOpen] = useState(false)
  const [confirmOpen, setConfirmOpen] = useState(false)
  const [confirmDestructiveOpen, setConfirmDestructiveOpen] = useState(false)
  const [progress, setProgress] = useState(65)
  const [switchOn, setSwitchOn] = useState(true)
  const [checkboxChecked, setCheckboxChecked] = useState<boolean | 'indeterminate'>(true)

  return (
    <TooltipProvider>
      <div className="bg-background">
        {/* ── Header ─────────────────────────────────────────────────────── */}
        <header className="sticky top-0 z-50 border-b bg-background/90 backdrop-blur-sm">
          <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
            <div className="flex items-center gap-2 text-sm">
              <div className="flex h-6 w-6 items-center justify-center rounded-md bg-primary">
                <TicketCheck className="h-3.5 w-3.5 text-primary-foreground" />
              </div>
              <span className="font-semibold text-foreground">Pide Servicio</span>
              <span className="text-muted-foreground">/</span>
              <span className="text-muted-foreground">Design System</span>
            </div>
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}
              aria-label="Cambiar tema"
            >
              {theme === 'dark' ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
            </Button>
          </div>
        </header>

        {/* ── Sub-navegación ──────────────────────────────────────────────── */}
        <nav className="sticky top-[57px] z-40 border-b bg-background/90 backdrop-blur-sm">
          <div className="scrollbar-hide mx-auto max-w-6xl overflow-x-auto">
            <div className="flex items-center gap-0.5 px-4 py-2">
              {NAV_ITEMS.map((item) => (
                <a
                  key={item.href}
                  href={item.href}
                  className="whitespace-nowrap rounded-md px-3 py-1.5 text-sm text-muted-foreground transition-colors hover:bg-accent hover:text-accent-foreground"
                >
                  {item.label}
                </a>
              ))}
            </div>
          </div>
        </nav>

        {/* ── Contenido principal ─────────────────────────────────────────── */}
        <main className="mx-auto max-w-6xl space-y-20 px-4 py-12">
          {/* ── 1. Colores ─────────────────────────────────────────────────── */}
          <Section id="colores" title="Colores">
            <div className="space-y-10">
              <SubSection title="Colores de marca">
                <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-6">
                  <ColorSwatch
                    cssVar="--primary"
                    foregroundVar="--primary-foreground"
                    label="Primary"
                    sublabel="Azul"
                  />
                  <ColorSwatch
                    cssVar="--secondary"
                    foregroundVar="--secondary-foreground"
                    label="Secondary"
                    sublabel="Gris claro"
                  />
                  <ColorSwatch
                    cssVar="--accent"
                    foregroundVar="--accent-foreground"
                    label="Accent"
                    sublabel="Énfasis"
                  />
                  <ColorSwatch
                    cssVar="--muted"
                    foregroundVar="--muted-foreground"
                    label="Muted"
                    sublabel="Neutro"
                  />
                  <ColorSwatch
                    cssVar="--background"
                    foregroundVar="--foreground"
                    label="Background"
                    sublabel="Fondo"
                  />
                  <ColorSwatch cssVar="--foreground" label="Foreground" sublabel="Texto" />
                </div>
              </SubSection>

              <SubSection title="Colores semánticos">
                <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
                  <ColorSwatch
                    cssVar="--success"
                    foregroundVar="--success-foreground"
                    label="Success"
                    sublabel="Verde"
                  />
                  <ColorSwatch
                    cssVar="--warning"
                    foregroundVar="--warning-foreground"
                    label="Warning"
                    sublabel="Ámbar"
                  />
                  <ColorSwatch
                    cssVar="--info"
                    foregroundVar="--info-foreground"
                    label="Info"
                    sublabel="Cian"
                  />
                  <ColorSwatch
                    cssVar="--destructive"
                    foregroundVar="--destructive-foreground"
                    label="Destructive"
                    sublabel="Rojo"
                  />
                </div>
              </SubSection>

              <SubSection title="Estados de tickets">
                <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-6">
                  <ColorSwatch
                    cssVar="--ticket-sin-asignar"
                    foregroundVar="--ticket-sin-asignar-foreground"
                    label="Sin Asignar"
                    sublabel="Gris"
                  />
                  <ColorSwatch
                    cssVar="--ticket-asignado"
                    foregroundVar="--ticket-asignado-foreground"
                    label="Asignado"
                    sublabel="Azul"
                  />
                  <ColorSwatch
                    cssVar="--ticket-en-proceso"
                    foregroundVar="--ticket-en-proceso-foreground"
                    label="En Proceso"
                    sublabel="Naranja"
                  />
                  <ColorSwatch
                    cssVar="--ticket-pendiente"
                    foregroundVar="--ticket-pendiente-foreground"
                    label="Pend. Valid."
                    sublabel="Ámbar"
                  />
                  <ColorSwatch
                    cssVar="--ticket-cerrado"
                    foregroundVar="--ticket-cerrado-foreground"
                    label="Cerrado"
                    sublabel="Verde"
                  />
                  <ColorSwatch
                    cssVar="--ticket-reabierto"
                    foregroundVar="--ticket-reabierto-foreground"
                    label="Reabierto"
                    sublabel="Rojo"
                  />
                </div>
              </SubSection>

              <SubSection title="Prioridades">
                <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
                  <ColorSwatch
                    cssVar="--priority-baja"
                    foregroundVar="--priority-baja-foreground"
                    label="Baja"
                    sublabel="Verde"
                  />
                  <ColorSwatch
                    cssVar="--priority-media"
                    foregroundVar="--priority-media-foreground"
                    label="Media"
                    sublabel="Ámbar"
                  />
                  <ColorSwatch
                    cssVar="--priority-alta"
                    foregroundVar="--priority-alta-foreground"
                    label="Alta"
                    sublabel="Naranja"
                  />
                  <ColorSwatch
                    cssVar="--priority-critica"
                    foregroundVar="--priority-critica-foreground"
                    label="Crítica"
                    sublabel="Rojo"
                  />
                </div>
              </SubSection>
            </div>
          </Section>

          {/* ── 2. Tipografía ───────────────────────────────────────────────── */}
          <Section id="tipografia" title="Tipografía">
            <DemoBox className="space-y-6">
              <div className="space-y-4">
                <div className="space-y-1">
                  <p className="font-mono text-xs text-muted-foreground">
                    display / 3.75rem / −2% tracking
                  </p>
                  <p className="text-display font-bold leading-none tracking-tight text-foreground">
                    Display
                  </p>
                </div>
                <div className="space-y-1">
                  <p className="font-mono text-xs text-muted-foreground">5xl / 3rem</p>
                  <h1 className="text-5xl font-bold text-foreground">Heading 1</h1>
                </div>
                <div className="space-y-1">
                  <p className="font-mono text-xs text-muted-foreground">4xl / 2.25rem</p>
                  <h1>Heading 2 — 4xl</h1>
                </div>
                <div className="space-y-1">
                  <p className="font-mono text-xs text-muted-foreground">3xl / 1.875rem</p>
                  <h2>Heading 3 — 3xl</h2>
                </div>
                <div className="space-y-1">
                  <p className="font-mono text-xs text-muted-foreground">2xl / 1.5rem</p>
                  <h2 className="text-2xl">Heading 4 — 2xl</h2>
                </div>
                <div className="space-y-1">
                  <p className="font-mono text-xs text-muted-foreground">xl / 1.25rem</p>
                  <h3 className="text-xl">Heading 5 — xl</h3>
                </div>
                <div className="space-y-1">
                  <p className="font-mono text-xs text-muted-foreground">lg / 1.125rem</p>
                  <h4>Heading 6 — lg</h4>
                </div>
                <Separator />
                <div className="space-y-3">
                  <div className="space-y-1">
                    <p className="font-mono text-xs text-muted-foreground">base / 1rem · Regular</p>
                    <p className="text-base">
                      Pide Servicio permite gestionar incidencias, solicitudes y riesgos de forma
                      centralizada y trazable.
                    </p>
                  </div>
                  <div className="space-y-1">
                    <p className="font-mono text-xs text-muted-foreground">
                      sm / 0.875rem · Regular
                    </p>
                    <p className="text-sm text-muted-foreground">
                      El sistema reemplaza el flujo de trabajo basado en correo electrónico con un
                      flujo digital, auditable y con control de estado.
                    </p>
                  </div>
                  <div className="space-y-1">
                    <p className="font-mono text-xs text-muted-foreground">xs / 0.75rem · Muted</p>
                    <p className="text-xs text-muted-foreground">
                      Creado el 29 de junio de 2026 · Ticket #PS-0042
                    </p>
                  </div>
                </div>
                <Separator />
                <div className="flex flex-wrap gap-6">
                  {(
                    [
                      'thin',
                      'light',
                      'normal',
                      'medium',
                      'semibold',
                      'bold',
                      'extrabold',
                      'black',
                    ] as const
                  ).map((w) => (
                    <div key={w} className="text-center">
                      <p className={`text-lg font-${w} text-foreground`}>Ag</p>
                      <p className="font-mono text-xs text-muted-foreground">{w}</p>
                    </div>
                  ))}
                </div>
              </div>
            </DemoBox>
          </Section>

          {/* ── 3. Botones ─────────────────────────────────────────────────── */}
          <Section id="botones" title="Botones">
            <div className="space-y-8">
              <SubSection title="Variantes">
                <DemoBox className="flex flex-wrap gap-3">
                  <Button variant="default">Default</Button>
                  <Button variant="secondary">Secondary</Button>
                  <Button variant="outline">Outline</Button>
                  <Button variant="ghost">Ghost</Button>
                  <Button variant="link">Link</Button>
                  <Button variant="destructive">Destructive</Button>
                </DemoBox>
              </SubSection>

              <SubSection title="Tamaños">
                <DemoBox className="flex flex-wrap items-center gap-3">
                  <Button size="sm">Small</Button>
                  <Button size="default">Default</Button>
                  <Button size="lg">Large</Button>
                  <Button size="icon" aria-label="Ajustes">
                    <Settings className="h-4 w-4" />
                  </Button>
                </DemoBox>
              </SubSection>

              <SubSection title="Con íconos">
                <DemoBox className="flex flex-wrap gap-3">
                  <Button>
                    <Plus className="mr-2 h-4 w-4" />
                    Nuevo Ticket
                  </Button>
                  <Button variant="outline">
                    <Download className="mr-2 h-4 w-4" />
                    Exportar
                  </Button>
                  <Button variant="secondary">
                    <Filter className="mr-2 h-4 w-4" />
                    Filtros
                  </Button>
                  <Button variant="ghost">
                    <RefreshCw className="mr-2 h-4 w-4" />
                    Actualizar
                  </Button>
                  <Button variant="destructive">
                    <Trash2 className="mr-2 h-4 w-4" />
                    Eliminar
                  </Button>
                </DemoBox>
              </SubSection>

              <SubSection title="Estados">
                <DemoBox className="flex flex-wrap gap-3">
                  <Button disabled>Deshabilitado</Button>
                  <Button variant="outline" disabled>
                    Outline disabled
                  </Button>
                  <Button>
                    <Spinner size="xs" className="mr-2" />
                    Guardando...
                  </Button>
                  <Button variant="outline">
                    <Spinner size="xs" className="mr-2 text-primary" />
                    Cargando...
                  </Button>
                </DemoBox>
              </SubSection>
            </div>
          </Section>

          {/* ── 4. Controles ───────────────────────────────────────────────── */}
          <Section id="controles" title="Controles de Formulario">
            <div className="grid gap-8 md:grid-cols-2">
              <DemoBox className="space-y-5">
                <SubSection title="Input">
                  <div className="space-y-3">
                    <div className="space-y-2">
                      <Label htmlFor="demo-input">Correo electrónico</Label>
                      <Input id="demo-input" type="email" placeholder="usuario@empresa.com" />
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="demo-search">Búsqueda</Label>
                      <div className="relative">
                        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                        <Input id="demo-search" className="pl-9" placeholder="Buscar tickets..." />
                      </div>
                    </div>
                    <div className="space-y-2">
                      <Label>Deshabilitado</Label>
                      <Input disabled placeholder="No editable" />
                    </div>
                  </div>
                </SubSection>
              </DemoBox>

              <DemoBox className="space-y-5">
                <SubSection title="Select">
                  <div className="space-y-3">
                    <div className="space-y-2">
                      <Label>Estado del ticket</Label>
                      <Select>
                        <SelectTrigger>
                          <SelectValue placeholder="Seleccionar estado..." />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="sin_asignar">Sin Asignar</SelectItem>
                          <SelectItem value="asignado">Asignado</SelectItem>
                          <SelectItem value="en_proceso">En Proceso</SelectItem>
                          <SelectItem value="pendiente_validacion">Pendiente Validación</SelectItem>
                          <SelectItem value="cerrado">Cerrado</SelectItem>
                          <SelectItem value="reabierto">Reabierto</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    <div className="space-y-2">
                      <Label>Sucursal</Label>
                      <Select>
                        <SelectTrigger>
                          <SelectValue placeholder="Seleccionar sucursal..." />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="central">Sede Central</SelectItem>
                          <SelectItem value="norte">Sucursal Norte</SelectItem>
                          <SelectItem value="sur">Sucursal Sur</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                  </div>
                </SubSection>
              </DemoBox>

              <DemoBox className="space-y-5">
                <SubSection title="Textarea">
                  <div className="space-y-2">
                    <Label htmlFor="demo-textarea">Descripción del incidente</Label>
                    <Textarea
                      id="demo-textarea"
                      placeholder="Describe el problema con el mayor detalle posible..."
                      rows={4}
                    />
                  </div>
                </SubSection>
              </DemoBox>

              <DemoBox className="space-y-5">
                <SubSection title="Checkbox, Radio & Switch">
                  <div className="space-y-4">
                    <div className="space-y-2">
                      <div className="flex items-center gap-2">
                        <Checkbox
                          id="chk1"
                          checked={checkboxChecked === true}
                          onCheckedChange={setCheckboxChecked}
                        />
                        <Label htmlFor="chk1">Notificar al responsable</Label>
                      </div>
                      <div className="flex items-center gap-2">
                        <Checkbox id="chk2" defaultChecked />
                        <Label htmlFor="chk2">Adjuntar evidencia inicial</Label>
                      </div>
                      <div className="flex items-center gap-2">
                        <Checkbox id="chk3" disabled />
                        <Label htmlFor="chk3" className="text-muted-foreground">
                          Opción bloqueada
                        </Label>
                      </div>
                    </div>
                    <Separator />
                    <RadioGroup defaultValue="alta">
                      {TICKET_PRIORITIES.map((p) => (
                        <div key={p} className="flex items-center gap-2">
                          <RadioGroupItem value={p} id={`radio-${p}`} />
                          <Label htmlFor={`radio-${p}`} className="capitalize">
                            {p}
                          </Label>
                        </div>
                      ))}
                    </RadioGroup>
                    <Separator />
                    <div className="flex items-center gap-3">
                      <Switch checked={switchOn} onCheckedChange={setSwitchOn} id="sw1" />
                      <Label htmlFor="sw1">
                        {switchOn ? 'Notificaciones activas' : 'Notificaciones inactivas'}
                      </Label>
                    </div>
                  </div>
                </SubSection>
              </DemoBox>
            </div>
          </Section>

          {/* ── 5. Tarjetas ─────────────────────────────────────────────────── */}
          <Section id="tarjetas" title="Tarjetas">
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {/* Tarjeta básica */}
              <Card>
                <CardHeader>
                  <CardTitle className="text-base">Ticket #PS-0042</CardTitle>
                  <CardDescription>Fallo en impresora · Sede Central</CardDescription>
                </CardHeader>
                <CardContent>
                  <p className="text-sm text-muted-foreground">
                    La impresora del área de RRHH no imprime. Se reinició dos veces sin éxito.
                  </p>
                </CardContent>
                <CardFooter className="flex justify-between">
                  <StatusBadge status="en_proceso" />
                  <PriorityBadge priority="alta" />
                </CardFooter>
              </Card>

              {/* Tarjeta con métricas */}
              <Card>
                <CardHeader className="pb-2">
                  <CardDescription>Tickets abiertos hoy</CardDescription>
                  <CardTitle className="text-4xl font-bold">24</CardTitle>
                </CardHeader>
                <CardContent>
                  <Progress value={65} className="h-2" />
                </CardContent>
                <CardFooter>
                  <p className="text-xs text-muted-foreground">
                    <span className="font-medium text-success">↑ 12%</span> respecto a ayer
                  </p>
                </CardFooter>
              </Card>

              {/* Tarjeta de usuario */}
              <Card>
                <CardContent className="pt-6">
                  <div className="flex flex-col items-center gap-3 text-center">
                    <Avatar className="h-16 w-16">
                      <AvatarImage src="/placeholder-avatar.jpg" />
                      <AvatarFallback className="bg-primary text-lg text-primary-foreground">
                        GC
                      </AvatarFallback>
                    </Avatar>
                    <div>
                      <p className="font-semibold">Gian Carlo</p>
                      <p className="text-sm text-muted-foreground">Administrador · Sede Central</p>
                    </div>
                    <Badge variant="secondary">Activo</Badge>
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Tabs dentro de tarjeta */}
            <Card className="mt-6">
              <CardHeader>
                <CardTitle className="text-base">Detalle del ticket</CardTitle>
              </CardHeader>
              <CardContent>
                <Tabs defaultValue="descripcion">
                  <TabsList className="mb-4">
                    <TabsTrigger value="descripcion">Descripción</TabsTrigger>
                    <TabsTrigger value="historial">Historial</TabsTrigger>
                    <TabsTrigger value="evidencias">Evidencias</TabsTrigger>
                  </TabsList>
                  <TabsContent value="descripcion" className="text-sm text-muted-foreground">
                    La impresora Ricoh MP 2014 del piso 3 no responde. Se verificó la conexión USB y
                    de red. Último intento de impresión fue el martes 28/06.
                  </TabsContent>
                  <TabsContent value="historial" className="space-y-3">
                    {[
                      {
                        time: 'Hoy 09:15',
                        text: 'Ticket asignado a Juan Pérez',
                        badge: 'asignado' as TicketStatus,
                      },
                      {
                        time: 'Hoy 08:30',
                        text: 'Ticket creado por María López',
                        badge: 'sin_asignar' as TicketStatus,
                      },
                    ].map((entry) => (
                      <div key={entry.time} className="flex items-start gap-3 text-sm">
                        <span className="shrink-0 text-xs text-muted-foreground">{entry.time}</span>
                        <span>{entry.text}</span>
                        <StatusBadge status={entry.badge} />
                      </div>
                    ))}
                  </TabsContent>
                  <TabsContent value="evidencias" className="text-sm text-muted-foreground">
                    No hay evidencias adjuntas.
                  </TabsContent>
                </Tabs>
              </CardContent>
            </Card>
          </Section>

          {/* ── 6. Badges & Estados ─────────────────────────────────────────── */}
          <Section id="badges" title="Badges y Estados">
            <div className="space-y-8">
              <SubSection title="Badge shadcn (variantes estándar)">
                <DemoBox className="flex flex-wrap gap-3">
                  <Badge>Default</Badge>
                  <Badge variant="secondary">Secondary</Badge>
                  <Badge variant="outline">Outline</Badge>
                  <Badge variant="destructive">Destructive</Badge>
                </DemoBox>
              </SubSection>

              <SubSection title="StatusBadge — estados de ticket">
                <DemoBox className="flex flex-wrap gap-3">
                  {TICKET_STATUSES.map((s) => (
                    <StatusBadge key={s} status={s} />
                  ))}
                </DemoBox>
              </SubSection>

              <SubSection title="PriorityBadge — prioridades">
                <DemoBox className="flex flex-wrap gap-4">
                  {TICKET_PRIORITIES.map((p) => (
                    <div key={p} className="flex flex-col items-start gap-2">
                      <PriorityBadge priority={p} showIcon />
                      <PriorityBadge priority={p} showIcon={false} />
                    </div>
                  ))}
                </DemoBox>
              </SubSection>

              <SubSection title="Avatar">
                <DemoBox className="flex flex-wrap items-center gap-4">
                  <Avatar className="h-8 w-8">
                    <AvatarFallback className="bg-primary text-xs text-primary-foreground">
                      GC
                    </AvatarFallback>
                  </Avatar>
                  <Avatar>
                    <AvatarFallback className="bg-success text-success-foreground">
                      JP
                    </AvatarFallback>
                  </Avatar>
                  <Avatar className="h-12 w-12">
                    <AvatarFallback className="bg-warning text-warning-foreground">
                      ML
                    </AvatarFallback>
                  </Avatar>
                  <Avatar className="h-16 w-16">
                    <AvatarFallback className="bg-destructive text-lg text-destructive-foreground">
                      AB
                    </AvatarFallback>
                  </Avatar>
                  <Avatar>
                    <AvatarImage src="/placeholder-avatar.jpg" alt="Usuario" />
                    <AvatarFallback>US</AvatarFallback>
                  </Avatar>
                </DemoBox>
              </SubSection>

              <SubSection title="Tooltip">
                <DemoBox className="flex flex-wrap gap-4">
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <Button variant="outline" size="icon">
                        <Bell className="h-4 w-4" />
                      </Button>
                    </TooltipTrigger>
                    <TooltipContent>Notificaciones (3 nuevas)</TooltipContent>
                  </Tooltip>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <Button variant="outline" size="icon">
                        <Settings className="h-4 w-4" />
                      </Button>
                    </TooltipTrigger>
                    <TooltipContent side="right">Configuración</TooltipContent>
                  </Tooltip>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <Badge variant="outline" className="cursor-help">
                        PS-0042
                      </Badge>
                    </TooltipTrigger>
                    <TooltipContent>
                      <p className="text-xs">Fallo en impresora · Sede Central</p>
                    </TooltipContent>
                  </Tooltip>
                </DemoBox>
              </SubSection>
            </div>
          </Section>

          {/* ── 7. Alertas ─────────────────────────────────────────────────── */}
          <Section id="alertas" title="Alertas">
            <div className="space-y-4">
              <Alert>
                <Info className="h-4 w-4" />
                <AlertTitle>Información</AlertTitle>
                <AlertDescription>
                  El ticket será asignado automáticamente al trabajador disponible con menor carga.
                </AlertDescription>
              </Alert>
              <Alert className="border-success/50 bg-success/10 text-foreground [&>svg]:text-success">
                <CheckCircle2 className="h-4 w-4" />
                <AlertTitle>Ticket cerrado correctamente</AlertTitle>
                <AlertDescription>
                  El incidente #PS-0042 fue resuelto y cerrado. Se generó el informe de cierre.
                </AlertDescription>
              </Alert>
              <Alert className="border-warning/50 bg-warning/10 text-foreground [&>svg]:text-warning">
                <AlertTriangle className="h-4 w-4" />
                <AlertTitle>Ticket pendiente de validación</AlertTitle>
                <AlertDescription>
                  El ticket #PS-0038 lleva más de 24 horas esperando validación del solicitante.
                </AlertDescription>
              </Alert>
              <Alert variant="destructive">
                <AlertCircle className="h-4 w-4" />
                <AlertTitle>Error al procesar la solicitud</AlertTitle>
                <AlertDescription>
                  No se pudo guardar el ticket. Verifica tu conexión e inténtalo de nuevo.
                </AlertDescription>
              </Alert>
            </div>

            {/* Accordion */}
            <div className="mt-8">
              <SubSection title="Accordion">
                <DemoBox>
                  <Accordion type="single" collapsible className="w-full">
                    <AccordionItem value="item-1">
                      <AccordionTrigger>¿Cómo asigno un ticket a un trabajador?</AccordionTrigger>
                      <AccordionContent>
                        Abre el ticket, haz clic en el campo "Asignado a" y selecciona al trabajador
                        disponible. El sistema notificará automáticamente.
                      </AccordionContent>
                    </AccordionItem>
                    <AccordionItem value="item-2">
                      <AccordionTrigger>¿Cuándo puedo reabrir un ticket cerrado?</AccordionTrigger>
                      <AccordionContent>
                        Solo el solicitante original puede reabrir un ticket cerrado, dentro de los
                        7 días posteriores al cierre, si el problema no fue resuelto.
                      </AccordionContent>
                    </AccordionItem>
                    <AccordionItem value="item-3">
                      <AccordionTrigger>¿Qué evidencias debo adjuntar?</AccordionTrigger>
                      <AccordionContent>
                        Adjunta fotos, capturas de pantalla o documentos que describan el problema
                        (evidencia inicial) y la solución aplicada (evidencia final).
                      </AccordionContent>
                    </AccordionItem>
                  </Accordion>
                </DemoBox>
              </SubSection>
            </div>
          </Section>

          {/* ── 8. Diálogos ─────────────────────────────────────────────────── */}
          <Section id="dialogos" title="Diálogos">
            <DemoBox className="flex flex-wrap gap-4">
              {/* Dialog informativo */}
              <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
                <DialogTrigger asChild>
                  <Button variant="outline">
                    <FileText className="mr-2 h-4 w-4" />
                    Ver Detalles
                  </Button>
                </DialogTrigger>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Ticket #PS-0042</DialogTitle>
                    <DialogDescription>
                      Fallo en impresora · Sede Central · Prioridad Alta
                    </DialogDescription>
                  </DialogHeader>
                  <div className="space-y-4 py-2">
                    <div className="flex items-center justify-between">
                      <span className="text-sm text-muted-foreground">Estado</span>
                      <StatusBadge status="en_proceso" />
                    </div>
                    <div className="flex items-center justify-between">
                      <span className="text-sm text-muted-foreground">Prioridad</span>
                      <PriorityBadge priority="alta" />
                    </div>
                    <div className="flex items-center justify-between">
                      <span className="text-sm text-muted-foreground">Asignado a</span>
                      <span className="text-sm font-medium">Juan Pérez</span>
                    </div>
                    <Separator />
                    <p className="text-sm text-muted-foreground">
                      La impresora Ricoh MP 2014 del piso 3 no responde. Se verificó la conexión USB
                      y de red.
                    </p>
                  </div>
                </DialogContent>
              </Dialog>

              {/* ConfirmDialog normal */}
              <Button onClick={() => setConfirmOpen(true)}>
                <CheckCircle2 className="mr-2 h-4 w-4" />
                Confirmar Acción
              </Button>
              <ConfirmDialog
                open={confirmOpen}
                onOpenChange={setConfirmOpen}
                title="¿Confirmar cierre del ticket?"
                description="Esta acción marcará el ticket #PS-0042 como cerrado. El solicitante podrá reabrirlo dentro de 7 días si el problema no fue resuelto."
                confirmLabel="Cerrar ticket"
                onConfirm={() => setConfirmOpen(false)}
              />

              {/* ConfirmDialog destructivo */}
              <Button variant="destructive" onClick={() => setConfirmDestructiveOpen(true)}>
                <Trash2 className="mr-2 h-4 w-4" />
                Eliminar
              </Button>
              <ConfirmDialog
                open={confirmDestructiveOpen}
                onOpenChange={setConfirmDestructiveOpen}
                title="¿Eliminar este ticket?"
                description="Esta acción es irreversible. El ticket y todas sus evidencias serán eliminados permanentemente del sistema."
                confirmLabel="Sí, eliminar"
                variant="destructive"
                onConfirm={() => setConfirmDestructiveOpen(false)}
              />

              {/* AlertDialog nativo */}
              <AlertDialog>
                <AlertDialogTrigger asChild>
                  <Button variant="outline">
                    <Shield className="mr-2 h-4 w-4" />
                    AlertDialog nativo
                  </Button>
                </AlertDialogTrigger>
              </AlertDialog>
            </DemoBox>
          </Section>

          {/* ── 9. Loading & Empty States ───────────────────────────────────── */}
          <Section id="loading" title="Loading y Vacíos">
            <div className="space-y-8">
              <SubSection title="Spinner">
                <DemoBox className="flex flex-wrap items-center gap-6">
                  {(['xs', 'sm', 'md', 'lg', 'xl'] as const).map((size) => (
                    <div key={size} className="flex flex-col items-center gap-2">
                      <Spinner size={size} className="text-primary" />
                      <span className="text-xs text-muted-foreground">{size}</span>
                    </div>
                  ))}
                  <div className="flex flex-col items-center gap-2">
                    <Spinner className="text-success" />
                    <span className="text-xs text-muted-foreground">success</span>
                  </div>
                  <div className="flex flex-col items-center gap-2">
                    <Spinner className="text-destructive" />
                    <span className="text-xs text-muted-foreground">destructive</span>
                  </div>
                </DemoBox>
              </SubSection>

              <SubSection title="SpinnerOverlay">
                <DemoBox className="relative h-36 overflow-hidden">
                  <div className="h-full rounded-lg bg-muted" />
                  <SpinnerOverlay label="Cargando tickets..." />
                </DemoBox>
              </SubSection>

              <SubSection title="Skeleton">
                <DemoBox className="space-y-4">
                  <div className="flex items-center gap-3">
                    <Skeleton className="h-10 w-10 rounded-full" />
                    <div className="flex-1 space-y-2">
                      <Skeleton className="h-4 w-1/3" />
                      <Skeleton className="h-3 w-1/2" />
                    </div>
                  </div>
                  <Skeleton className="h-24 w-full rounded-lg" />
                  <div className="grid grid-cols-3 gap-3">
                    <Skeleton className="h-20 rounded-lg" />
                    <Skeleton className="h-20 rounded-lg" />
                    <Skeleton className="h-20 rounded-lg" />
                  </div>
                </DemoBox>
              </SubSection>

              <SubSection title="Progress">
                <DemoBox className="space-y-4">
                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span>Tickets cerrados esta semana</span>
                      <span className="font-medium">{progress}%</span>
                    </div>
                    <Progress value={progress} className="h-2" />
                  </div>
                  <div className="flex gap-3">
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => setProgress(Math.max(0, progress - 10))}
                    >
                      −10
                    </Button>
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => setProgress(Math.min(100, progress + 10))}
                    >
                      +10
                    </Button>
                  </div>
                  <div className="space-y-2">
                    <Progress value={100} className="h-1.5 [&>div]:bg-success" />
                    <Progress value={65} className="h-1.5 [&>div]:bg-warning" />
                    <Progress value={30} className="h-1.5 [&>div]:bg-destructive" />
                  </div>
                </DemoBox>
              </SubSection>

              <SubSection title="Empty States">
                <div className="grid gap-4 sm:grid-cols-3">
                  <DemoBox>
                    <EmptyState
                      icon={TicketCheck}
                      title="Sin tickets"
                      description="No hay tickets abiertos en este momento."
                      size="sm"
                    />
                  </DemoBox>
                  <DemoBox>
                    <EmptyState
                      icon={Search}
                      title="Sin resultados"
                      description="Ningún ticket coincide con los filtros aplicados."
                      action={
                        <Button variant="outline" size="sm">
                          Limpiar filtros
                        </Button>
                      }
                      size="sm"
                    />
                  </DemoBox>
                  <DemoBox>
                    <EmptyState
                      icon={Bell}
                      title="Sin notificaciones"
                      description="Estás al día con todas las novedades."
                      size="sm"
                    />
                  </DemoBox>
                </div>
              </SubSection>
            </div>
          </Section>

          {/* ── 10. Íconos ──────────────────────────────────────────────────── */}
          <Section id="iconos" title="Íconos (Lucide React)">
            <DemoBox>
              <div className="grid grid-cols-4 gap-4 sm:grid-cols-6 md:grid-cols-8 lg:grid-cols-10">
                {LUCIDE_ICONS.map(({ icon: Icon, name }) => (
                  <Tooltip key={name}>
                    <TooltipTrigger asChild>
                      <div className="flex cursor-default flex-col items-center gap-2 rounded-lg p-3 transition-colors hover:bg-accent">
                        <Icon className="h-5 w-5 text-foreground" />
                        <span className="hidden w-full truncate text-center text-xs text-muted-foreground sm:block">
                          {name}
                        </span>
                      </div>
                    </TooltipTrigger>
                    <TooltipContent>{name}</TooltipContent>
                  </Tooltip>
                ))}
              </div>
            </DemoBox>
          </Section>
        </main>

        {/* ── Footer ─────────────────────────────────────────────────────── */}
        <footer className="mt-16 border-t py-6">
          <div className="mx-auto max-w-6xl px-4 text-center">
            <p className="text-xs text-muted-foreground">
              Pide Servicio — Design System · Fase 2 · {new Date().getFullYear()}
            </p>
          </div>
        </footer>
      </div>
    </TooltipProvider>
  )
}
