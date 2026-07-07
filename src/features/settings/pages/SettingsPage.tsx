import {
  Bell,
  Shield,
  Clock,
  Ticket,
  Building2,
  ChevronRight,
  HelpCircle,
  AlertTriangle,
} from 'lucide-react'
import { useState } from 'react'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Switch } from '@shared/ui/switch'
import { Button } from '@shared/ui/button'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { Input } from '@shared/ui/input'
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@shared/ui/tooltip'
import { ConfirmDialog } from '@shared/components/ConfirmDialog'
import { FormField } from '@shared/components/FormField'
import { toast } from 'sonner'

// ─── Helpers ───────────────────────────────────────────────────────────────

function FieldTooltip({ text }: { text: string }) {
  return (
    <TooltipProvider>
      <Tooltip>
        <TooltipTrigger asChild>
          <HelpCircle className="ml-1 inline-block h-3 w-3 cursor-help text-muted-foreground" />
        </TooltipTrigger>
        <TooltipContent>
          <p className="max-w-xs text-xs">{text}</p>
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>
  )
}

interface ToggleRowProps {
  label: React.ReactNode
  description: string
  checked: boolean
  onCheckedChange: (v: boolean) => void
}

function ToggleRow({ label, description, checked, onCheckedChange }: ToggleRowProps) {
  return (
    <div className="flex items-center justify-between gap-4 rounded-md border border-border bg-muted/30 px-4 py-3">
      <div className="min-w-0 flex-1">
        <p className="text-sm font-medium text-foreground">{label}</p>
        <p className="mt-0.5 text-xs text-muted-foreground">{description}</p>
      </div>
      <Switch checked={checked} onCheckedChange={onCheckedChange} />
    </div>
  )
}

function DangerRow({
  label,
  description,
  onClick,
}: {
  label: string
  description: string
  onClick: () => void
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="flex w-full items-center justify-between gap-4 rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-left transition-colors hover:bg-destructive/10 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-destructive"
    >
      <div className="min-w-0 flex-1">
        <p className="text-sm font-medium text-red-700">{label}</p>
        <p className="mt-0.5 text-xs text-red-500/80">{description}</p>
      </div>
      <ChevronRight className="h-4 w-4 shrink-0 text-red-400" />
    </button>
  )
}

// ─── Main page ──────────────────────────────────────────────────────────────

export function SettingsPage() {
  // General
  const [zona, setZona] = useState('America/Lima')
  const [modoMantenimiento, setModoMantenimiento] = useState(false)

  // Seguridad
  const [sessionTimeout, setSessionTimeout] = useState('60')
  const [dosFactor, setDosFactor] = useState(false)
  const [bloqueoPorIntentos, setBloqueoPorIntentos] = useState(true)

  // Tickets
  const [maxFileSize, setMaxFileSize] = useState('10')
  const [asignacionAutomatica, setAsignacionAutomatica] = useState(true)
  const [permitirReapertura, setPermitirReapertura] = useState(true)

  // Notificaciones
  const [notifEmail, setNotifEmail] = useState(true)
  const [resumenDiario, setResumenDiario] = useState(false)
  const [alertasCriticos, setAlertasCriticos] = useState(true)
  const [recordatoriosSla, setRecordatoriosSla] = useState(true)

  // Danger dialogs
  const [confirmLimpiarLogs, setConfirmLimpiarLogs] = useState(false)
  const [confirmRestablecer, setConfirmRestablecer] = useState(false)

  return (
    <div className="px-3 py-3 lg:px-5">
      {/* Header de pagina */}
      <div className="mb-4">
        <h2 className="text-base font-semibold tracking-tight">Configuracion</h2>
        <p className="text-xs text-muted-foreground">
          Ajusta los parametros del sistema, seguridad, tickets y notificaciones.
        </p>
      </div>

      {/* Grid de secciones: 1 columna en mobile, 2 en desktop */}
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        {/* ── GENERAL ─────────────────────────────────────────────────────── */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              <Building2 className="h-3.5 w-3.5 text-blue-500" />
              General
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4 p-3 pt-0">
            <div className="grid gap-3 sm:grid-cols-2">
              <FormField
                label={
                  <>
                    Nombre de la empresa
                    <FieldTooltip text="Nombre legal o comercial de la organizacion. Aparece en reportes, correos y documentos generados por el sistema." />
                  </>
                }
                required
              >
                <Input defaultValue="Empresa Demo S.A.C." className="h-9 text-sm" />
              </FormField>
              <FormField
                label={
                  <>
                    Zona horaria
                    <FieldTooltip text="Define la zona horaria base para registrar fechas de tickets, auditorias y notificaciones. Todos los timestamps del sistema se almacenan en UTC y se muestran en esta zona." />
                  </>
                }
                required
              >
                <Select
                  value={zona}
                  onValueChange={(v) => {
                    setZona(v)
                    toast.success('Zona horaria actualizada')
                  }}
                >
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="America/Lima">America/Lima (UTC-5)</SelectItem>
                    <SelectItem value="America/Bogota">America/Bogota (UTC-5)</SelectItem>
                    <SelectItem value="America/Santiago">America/Santiago (UTC-4)</SelectItem>
                  </SelectContent>
                </Select>
              </FormField>
            </div>

            <ToggleRow
              label={
                <>
                  Modo de mantenimiento
                  <FieldTooltip text="Mientras este activo, solo el SuperAdministrador puede ingresar al sistema. Los demas usuarios veran una pantalla de mantenimiento en curso." />
                </>
              }
              description="Bloquea el acceso a todos los usuarios excepto superadmin"
              checked={modoMantenimiento}
              onCheckedChange={(v) => {
                setModoMantenimiento(v)
                toast.success(v ? 'Modo mantenimiento activado' : 'Modo mantenimiento desactivado')
              }}
            />

            <div className="flex justify-end pt-1">
              <Button size="sm" onClick={() => toast.success('Configuracion general guardada')}>
                Guardar cambios
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* ── SEGURIDAD ───────────────────────────────────────────────────── */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              <Shield className="h-3.5 w-3.5 text-blue-500" />
              Seguridad y sesiones
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4 p-3 pt-0">
            <FormField
              label={
                <>
                  Tiempo de sesion (minutos)
                  <FieldTooltip text="Tiempo de inactividad permitido antes de cerrar la sesion automaticamente. Un valor menor aumenta la seguridad, pero puede interrumpir el trabajo en curso." />
                </>
              }
              required
            >
              <Select
                value={sessionTimeout}
                onValueChange={(v) => {
                  setSessionTimeout(v)
                  toast.success(`Tiempo de sesion actualizado a ${v} minutos`)
                }}
              >
                <SelectTrigger className="h-9 text-sm">
                  <Clock className="mr-2 h-4 w-4 text-muted-foreground" />
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {['30', '60', '120', '240', '480'].map((v) => (
                    <SelectItem key={v} value={v}>
                      {v} minutos
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FormField>

            <div className="space-y-3">
              <ToggleRow
                label={
                  <>
                    Autenticacion de dos factores
                    <FieldTooltip text="Agrega una capa adicional de seguridad al iniciar sesion. Los administradores deberan verificar su identidad con un codigo de un solo uso (OTP) ademas de su contrasena." />
                  </>
                }
                description="Requerir 2FA para administradores"
                checked={dosFactor}
                onCheckedChange={(v) => {
                  setDosFactor(v)
                  toast.success(
                    v
                      ? 'Autenticacion de dos factores activada'
                      : 'Autenticacion de dos factores desactivada',
                  )
                }}
              />
              <ToggleRow
                label={
                  <>
                    Bloqueo por intentos fallidos
                    <FieldTooltip text="Bloquea temporalmente una cuenta de usuario despues de 5 intentos de inicio de sesion incorrectos consecutivos, previniendo ataques de fuerza bruta." />
                  </>
                }
                description="Bloquear cuenta tras 5 intentos fallidos"
                checked={bloqueoPorIntentos}
                onCheckedChange={(v) => {
                  setBloqueoPorIntentos(v)
                  toast.success(
                    v
                      ? 'Bloqueo por intentos fallidos activado'
                      : 'Bloqueo por intentos fallidos desactivado',
                  )
                }}
              />
            </div>

            <div className="flex justify-end pt-1">
              <Button
                size="sm"
                onClick={() => toast.success('Configuracion de seguridad guardada')}
              >
                Guardar cambios
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* ── TICKETS ─────────────────────────────────────────────────────── */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              <Ticket className="h-3.5 w-3.5 text-blue-500" />
              Configuracion de tickets
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4 p-3 pt-0">
            <FormField
              label={
                <>
                  Tamano maximo de archivo adjunto (MB)
                  <FieldTooltip text="Limite de tamano por archivo adjunto en un ticket. Afecta a todos los archivos que los usuarios puedan subir (imagenes, PDFs, documentos). Valores mayores consumen mas almacenamiento." />
                </>
              }
              required
            >
              <Select
                value={maxFileSize}
                onValueChange={(v) => {
                  setMaxFileSize(v)
                  toast.success(`Tamano maximo de adjunto actualizado a ${v} MB`)
                }}
              >
                <SelectTrigger className="h-9 text-sm">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {['5', '10', '20', '50'].map((v) => (
                    <SelectItem key={v} value={v}>
                      {v} MB
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FormField>

            <div className="space-y-3">
              <ToggleRow
                label={
                  <>
                    Asignacion automatica
                    <FieldTooltip text="El sistema asigna automaticamente los nuevos tickets al trabajador disponible con menor carga de trabajo en la sucursal correspondiente. Si esta desactivado, los tickets quedan sin asignar hasta que un administrador los asigne manualmente." />
                  </>
                }
                description="Asignar al trabajador disponible con menos carga"
                checked={asignacionAutomatica}
                onCheckedChange={(v) => {
                  setAsignacionAutomatica(v)
                  toast.success(
                    v ? 'Asignacion automatica activada' : 'Asignacion automatica desactivada',
                  )
                }}
              />
              <ToggleRow
                label={
                  <>
                    Permitir reapertura de tickets
                    <FieldTooltip text="Permite a los usuarios volver a abrir un ticket cerrado dentro de un plazo de 7 dias desde su cierre, en caso de que el problema no haya quedado resuelto satisfactoriamente." />
                  </>
                }
                description="Los usuarios pueden reabrir tickets cerrados dentro de 7 dias"
                checked={permitirReapertura}
                onCheckedChange={(v) => {
                  setPermitirReapertura(v)
                  toast.success(
                    v ? 'Reapertura de tickets activada' : 'Reapertura de tickets desactivada',
                  )
                }}
              />
            </div>

            {/* SLA row */}
            <div className="flex items-center justify-between gap-4 rounded-md border border-border bg-muted/30 px-4 py-3">
              <div className="min-w-0 flex-1">
                <p className="text-sm font-medium text-foreground">
                  SLA por prioridad
                  <FieldTooltip text="Define tiempos maximos de resolucion segun la prioridad del ticket (Critica, Alta, Media, Baja). El sistema generara alertas cuando un ticket este proximo a superar su tiempo limite." />
                </p>
                <p className="mt-0.5 text-xs text-muted-foreground">
                  Alertas de vencimiento segun prioridad
                </p>
              </div>
              <Button
                variant="outline"
                size="sm"
                className="shrink-0"
                onClick={() => toast.info('Configuracion de SLA disponible proximamente')}
              >
                Configurar
                <ChevronRight className="ml-1 h-3.5 w-3.5" />
              </Button>
            </div>

            <div className="flex justify-end pt-1">
              <Button size="sm" onClick={() => toast.success('Configuracion de tickets guardada')}>
                Guardar cambios
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* ── NOTIFICACIONES ──────────────────────────────────────────────── */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              <Bell className="h-3.5 w-3.5 text-blue-500" />
              Notificaciones del sistema
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 p-3 pt-0">
            <ToggleRow
              label={
                <>
                  Notificaciones por correo
                  <FieldTooltip text="Envia correos electronicos automaticos a los involucrados cuando un ticket cambia de estado (apertura, asignacion, resolucion, cierre). Requiere configuracion de servidor SMTP." />
                </>
              }
              description="Emails automaticos para cambios de estado"
              checked={notifEmail}
              onCheckedChange={(v) => {
                setNotifEmail(v)
                toast.success(
                  v
                    ? 'Notificaciones por correo activadas'
                    : 'Notificaciones por correo desactivadas',
                )
              }}
            />
            <ToggleRow
              label={
                <>
                  Resumen diario a administradores
                  <FieldTooltip text="Envia cada manana a las 8:00 AM un correo con el resumen del dia anterior: tickets abiertos, cerrados, tiempo promedio de resolucion y alertas de SLA vencidas." />
                </>
              }
              description="Email con metricas a las 8:00 AM"
              checked={resumenDiario}
              onCheckedChange={(v) => {
                setResumenDiario(v)
                toast.success(v ? 'Resumen diario activado' : 'Resumen diario desactivado')
              }}
            />
            <ToggleRow
              label={
                <>
                  Alertas de tickets criticos
                  <FieldTooltip text="Notifica de forma inmediata por correo y notificacion push cuando se crea o escala un ticket con prioridad Critica, para garantizar atencion inmediata." />
                </>
              }
              description="Notificar inmediatamente por email y push"
              checked={alertasCriticos}
              onCheckedChange={(v) => {
                setAlertasCriticos(v)
                toast.success(
                  v
                    ? 'Alertas de tickets criticos activadas'
                    : 'Alertas de tickets criticos desactivadas',
                )
              }}
            />
            <ToggleRow
              label={
                <>
                  Recordatorios de SLA
                  <FieldTooltip text="Envia una alerta al trabajador asignado y al administrador cuando un ticket lleva el 80% del tiempo SLA consumido sin resolverse, para evitar incumplimientos." />
                </>
              }
              description="Alertar cuando un ticket esta proximo a vencer"
              checked={recordatoriosSla}
              onCheckedChange={(v) => {
                setRecordatoriosSla(v)
                toast.success(
                  v ? 'Recordatorios de SLA activados' : 'Recordatorios de SLA desactivados',
                )
              }}
            />

            <div className="flex justify-end pt-1">
              <Button
                size="sm"
                onClick={() => toast.success('Configuracion de notificaciones guardada')}
              >
                Guardar cambios
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* ── ZONA DE PELIGRO (ocupa las 2 columnas en desktop) ───────────── */}
        <Card className="border-red-200 lg:col-span-2">
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-red-500">
              <AlertTriangle className="h-3.5 w-3.5" />
              Zona de peligro
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 p-3 pt-0">
            <p className="text-xs text-muted-foreground">
              Estas acciones son permanentes e irreversibles. Procede con precaucion.
            </p>
            <div className="grid gap-3 sm:grid-cols-2">
              <DangerRow
                label="Limpiar logs de auditoria"
                description="Elimina registros con mas de 1 ano de antiguedad"
                onClick={() => setConfirmLimpiarLogs(true)}
              />
              <DangerRow
                label="Restablecer configuracion"
                description="Volver a los valores predeterminados del sistema"
                onClick={() => setConfirmRestablecer(true)}
              />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* ── Dialogs ─────────────────────────────────────────────────────────── */}
      <ConfirmDialog
        open={confirmLimpiarLogs}
        onOpenChange={setConfirmLimpiarLogs}
        title="Limpiar logs de auditoria?"
        description="Se eliminaran todos los registros de auditoria con mas de 1 ano de antiguedad. Esta accion no se puede deshacer."
        confirmLabel="Limpiar logs"
        variant="destructive"
        onConfirm={() => {
          setConfirmLimpiarLogs(false)
          toast.info('Logs de auditoria eliminados')
        }}
      />
      <ConfirmDialog
        open={confirmRestablecer}
        onOpenChange={setConfirmRestablecer}
        title="Restablecer configuracion?"
        description="Se restauraran todos los valores predeterminados del sistema. Esta accion no se puede deshacer."
        confirmLabel="Restablecer"
        variant="destructive"
        onConfirm={() => {
          setConfirmRestablecer(false)
          toast.info('Configuracion restablecida')
        }}
      />
    </div>
  )
}
