import { useState } from 'react'
import { Settings, Bell, Shield, Clock, Ticket, Building2, ChevronRight } from 'lucide-react'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { Switch } from '@shared/ui/switch'
import { Button } from '@shared/ui/button'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { Label } from '@shared/ui/label'
import { Input } from '@shared/ui/input'
import { ConfirmDialog } from '@shared/components/ConfirmDialog'
import { toast } from 'sonner'

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

  // Notificaciones del sistema
  const [notifEmail, setNotifEmail] = useState(true)
  const [resumenDiario, setResumenDiario] = useState(false)
  const [alertasCriticos, setAlertasCriticos] = useState(true)
  const [recordatoriosSla, setRecordatoriosSla] = useState(true)

  // Dialogs de zona de peligro
  const [confirmLimpiarLogs, setConfirmLimpiarLogs] = useState(false)
  const [confirmRestablecer, setConfirmRestablecer] = useState(false)

  return (
    <div className="mx-auto max-w-2xl space-y-3 p-3 lg:p-4">
      {/* Header */}
      <div>
        <h2 className="flex items-center gap-2 text-base font-semibold tracking-tight">
          <Settings className="h-5 w-5" />
          Configuración
        </h2>
        <p className="text-xs text-muted-foreground">Ajustes globales del sistema Pide Servicio.</p>
      </div>

      {/* General settings */}
      <Card>
        <CardHeader className="px-3 pb-2 pt-3">
          <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            <Building2 className="h-4 w-4" />
            General
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-3 p-3 pt-0">
          <div className="space-y-2">
            <Label htmlFor="empresa" className="text-xs font-medium">
              Nombre de la empresa
            </Label>
            <Input id="empresa" defaultValue="Empresa Demo S.A.C." />
          </div>
          <div className="space-y-2">
            <Label htmlFor="zona" className="text-xs font-medium">
              Zona horaria
            </Label>
            <Select
              value={zona}
              onValueChange={(v) => {
                setZona(v)
                toast.success('Zona horaria actualizada')
              }}
            >
              <SelectTrigger id="zona">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="America/Lima">América/Lima (UTC-5)</SelectItem>
                <SelectItem value="America/Bogota">América/Bogotá (UTC-5)</SelectItem>
                <SelectItem value="America/Santiago">América/Santiago (UTC-4)</SelectItem>
              </SelectContent>
            </Select>
          </div>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium">Modo de mantenimiento</p>
              <p className="text-xs text-muted-foreground">
                Bloquea el acceso a todos los usuarios excepto superadmin
              </p>
            </div>
            <Switch
              checked={modoMantenimiento}
              onCheckedChange={(v) => {
                setModoMantenimiento(v)
                toast.success(v ? 'Modo mantenimiento activado' : 'Modo mantenimiento desactivado')
              }}
            />
          </div>
          <Button
            size="sm"
            className="mt-3 w-full"
            onClick={() => toast.success('Configuración general guardada')}
          >
            Guardar cambios
          </Button>
        </CardContent>
      </Card>

      {/* Security */}
      <Card>
        <CardHeader className="px-3 pb-2 pt-3">
          <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            <Shield className="h-4 w-4" />
            Seguridad y sesiones
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-3 p-3 pt-0">
          <div className="space-y-2">
            <Label htmlFor="session" className="text-xs font-medium">
              Tiempo de sesión (minutos)
            </Label>
            <Select
              value={sessionTimeout}
              onValueChange={(v) => {
                setSessionTimeout(v)
                toast.success(`Tiempo de sesión actualizado a ${v} minutos`)
              }}
            >
              <SelectTrigger id="session">
                <Clock className="mr-2 h-3.5 w-3.5 text-muted-foreground" />
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
          </div>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium">Autenticación de dos factores</p>
              <p className="text-xs text-muted-foreground">Requerir 2FA para administradores</p>
            </div>
            <Switch
              checked={dosFactor}
              onCheckedChange={(v) => {
                setDosFactor(v)
                toast.success(
                  v
                    ? 'Autenticación de dos factores activada'
                    : 'Autenticación de dos factores desactivada',
                )
              }}
            />
          </div>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium">Bloqueo por intentos fallidos</p>
              <p className="text-xs text-muted-foreground">
                Bloquear cuenta tras 5 intentos fallidos
              </p>
            </div>
            <Switch
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
          <Button
            size="sm"
            className="mt-3 w-full"
            onClick={() => toast.success('Configuración de seguridad guardada')}
          >
            Guardar cambios
          </Button>
        </CardContent>
      </Card>

      {/* Tickets */}
      <Card>
        <CardHeader className="px-3 pb-2 pt-3">
          <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            <Ticket className="h-4 w-4" />
            Configuración de tickets
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-3 p-3 pt-0">
          <div className="space-y-2">
            <Label htmlFor="filesize" className="text-xs font-medium">
              Tamaño máximo de archivo adjunto (MB)
            </Label>
            <Select
              value={maxFileSize}
              onValueChange={(v) => {
                setMaxFileSize(v)
                toast.success(`Tamaño máximo de adjunto actualizado a ${v} MB`)
              }}
            >
              <SelectTrigger id="filesize">
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
          </div>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium">Asignación automática</p>
              <p className="text-xs text-muted-foreground">
                Asignar tickets automáticamente al trabajador disponible del área
              </p>
            </div>
            <Switch
              checked={asignacionAutomatica}
              onCheckedChange={(v) => {
                setAsignacionAutomatica(v)
                toast.success(
                  v ? 'Asignación automática activada' : 'Asignación automática desactivada',
                )
              }}
            />
          </div>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium">Permitir reapertura de tickets</p>
              <p className="text-xs text-muted-foreground">
                Los usuarios pueden reabrir tickets cerrados dentro de 7 días
              </p>
            </div>
            <Switch
              checked={permitirReapertura}
              onCheckedChange={(v) => {
                setPermitirReapertura(v)
                toast.success(
                  v ? 'Reapertura de tickets activada' : 'Reapertura de tickets desactivada',
                )
              }}
            />
          </div>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium">SLA por prioridad</p>
              <p className="text-xs text-muted-foreground">
                Alertas de vencimiento según prioridad
              </p>
            </div>
            <Button
              variant="ghost"
              size="sm"
              className="h-7 text-xs"
              onClick={() => toast.info('Configuración de SLA disponible próximamente')}
            >
              Configurar
            </Button>
          </div>
          <Button
            size="sm"
            className="mt-3 w-full"
            onClick={() => toast.success('Configuración de tickets guardada')}
          >
            Guardar cambios
          </Button>
        </CardContent>
      </Card>

      {/* Notifications global */}
      <Card>
        <CardHeader className="px-3 pb-2 pt-3">
          <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            <Bell className="h-4 w-4" />
            Notificaciones del sistema
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-3 p-3 pt-0">
          <div className="flex items-center justify-between gap-4">
            <div className="min-w-0 flex-1">
              <p className="text-xs font-medium">Notificaciones por correo</p>
              <p className="text-xs text-muted-foreground">
                Enviar emails automáticos para cambios de estado
              </p>
            </div>
            <Switch
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
          </div>
          <div className="flex items-center justify-between gap-4">
            <div className="min-w-0 flex-1">
              <p className="text-xs font-medium">Resumen diario a administradores</p>
              <p className="text-xs text-muted-foreground">Email con métricas a las 8:00 AM</p>
            </div>
            <Switch
              checked={resumenDiario}
              onCheckedChange={(v) => {
                setResumenDiario(v)
                toast.success(v ? 'Resumen diario activado' : 'Resumen diario desactivado')
              }}
            />
          </div>
          <div className="flex items-center justify-between gap-4">
            <div className="min-w-0 flex-1">
              <p className="text-xs font-medium">Alertas de tickets críticos</p>
              <p className="text-xs text-muted-foreground">
                Notificar inmediatamente por email y push
              </p>
            </div>
            <Switch
              checked={alertasCriticos}
              onCheckedChange={(v) => {
                setAlertasCriticos(v)
                toast.success(
                  v
                    ? 'Alertas de tickets críticos activadas'
                    : 'Alertas de tickets críticos desactivadas',
                )
              }}
            />
          </div>
          <div className="flex items-center justify-between gap-4">
            <div className="min-w-0 flex-1">
              <p className="text-xs font-medium">Recordatorios de SLA</p>
              <p className="text-xs text-muted-foreground">
                Alertar cuando un ticket está próximo a vencer
              </p>
            </div>
            <Switch
              checked={recordatoriosSla}
              onCheckedChange={(v) => {
                setRecordatoriosSla(v)
                toast.success(
                  v ? 'Recordatorios de SLA activados' : 'Recordatorios de SLA desactivados',
                )
              }}
            />
          </div>
          <Button
            size="sm"
            className="mt-3 w-full"
            onClick={() => toast.success('Configuración de notificaciones guardada')}
          >
            Guardar cambios
          </Button>
        </CardContent>
      </Card>

      {/* Danger zone */}
      <Card className="border-destructive/50">
        <CardHeader className="bg-destructive/5 px-3 pb-2 pt-3">
          <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-destructive">
            Zona de peligro
          </CardTitle>
          <CardDescription>Estas acciones son irreversibles.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-2 p-3 pt-0">
          <button
            type="button"
            onClick={() => setConfirmLimpiarLogs(true)}
            className="flex w-full items-center justify-between rounded-lg border border-destructive/20 px-4 py-3 text-left transition-colors hover:bg-destructive/5"
          >
            <div>
              <p className="text-xs font-medium">Limpiar logs de auditoría</p>
              <p className="text-xs text-muted-foreground">
                Elimina registros con más de 1 año de antigüedad
              </p>
            </div>
            <ChevronRight className="h-4 w-4 text-muted-foreground" />
          </button>
          <button
            type="button"
            onClick={() => setConfirmRestablecer(true)}
            className="flex w-full items-center justify-between rounded-lg border border-destructive/20 px-4 py-3 text-left transition-colors hover:bg-destructive/5"
          >
            <div>
              <p className="text-xs font-medium">Restablecer configuración</p>
              <p className="text-xs text-muted-foreground">
                Volver a los valores predeterminados del sistema
              </p>
            </div>
            <ChevronRight className="h-4 w-4 text-muted-foreground" />
          </button>
        </CardContent>
      </Card>

      <div className="flex justify-end">
        <Button onClick={() => toast.success('Configuración guardada')}>Guardar cambios</Button>
      </div>

      {/* Confirm: Limpiar logs */}
      <ConfirmDialog
        open={confirmLimpiarLogs}
        onOpenChange={setConfirmLimpiarLogs}
        title="¿Limpiar logs de auditoría?"
        description="Se eliminarán todos los registros de auditoría con más de 1 año de antigüedad. Esta acción no se puede deshacer."
        confirmLabel="Limpiar logs"
        variant="destructive"
        onConfirm={() => {
          setConfirmLimpiarLogs(false)
          toast.info('Logs de auditoría eliminados')
        }}
      />

      {/* Confirm: Restablecer configuración */}
      <ConfirmDialog
        open={confirmRestablecer}
        onOpenChange={setConfirmRestablecer}
        title="¿Restablecer configuración?"
        description="Se restaurarán todos los valores predeterminados del sistema. Esta acción no se puede deshacer."
        confirmLabel="Restablecer"
        variant="destructive"
        onConfirm={() => {
          setConfirmRestablecer(false)
          toast.info('Configuración restablecida')
        }}
      />
    </div>
  )
}
