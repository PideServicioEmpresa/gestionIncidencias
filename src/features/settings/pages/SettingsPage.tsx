import { Ticket, Building2, ChevronRight, HelpCircle, MapPin } from 'lucide-react'
import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import { useAuthStore } from '@store/auth.store'
import { ROUTES } from '@constants/index'
import { configuracionService } from '../services/configuracionService'
import { SeccionTiposServicio } from '../components/SeccionTiposServicio'
import { SeccionCategorias } from '../components/SeccionCategorias'
import { SeccionRoles } from '../components/SeccionRoles'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Switch } from '@shared/ui/switch'
import { Button } from '@shared/ui/button'
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@shared/ui/tooltip'
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

// ─── Main page ──────────────────────────────────────────────────────────────

export function SettingsPage() {
  const navigate = useNavigate()
  const qc = useQueryClient()
  const user = useAuthStore((s) => s.user)

  const { data: parametros, error: parametrosError } = useQuery({
    queryKey: ['configuracion', user?.empresaId],
    queryFn: () => configuracionService.listar(user?.empresaId),
    retry: false,
  })

  useEffect(() => {
    if (parametrosError) toast.error(parametrosError.message)
  }, [parametrosError])

  // SuperAdmin guarda en ámbito global (empresa_id null); Admin en el de su empresa
  const empresaIdParaGuardar = user?.rol === 'superadmin' ? undefined : user?.empresaId

  const guardar = async (pares: Record<string, string>, mensajeExito: string) => {
    try {
      await Promise.all(
        Object.entries(pares).map(([clave, valor]) =>
          configuracionService.actualizar(clave, valor, empresaIdParaGuardar),
        ),
      )
      void qc.invalidateQueries({ queryKey: ['configuracion'] })
      toast.success(mensajeExito)
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Error al guardar la configuración')
    }
  }

  // Parámetros conectados de verdad
  const [modoMantenimiento, setModoMantenimiento] = useState(false)
  const [permitirReapertura, setPermitirReapertura] = useState(true)

  // TODO: conectar NOTIF_EMAIL y ALERTAS_CRITICOS cuando el envío de correo esté configurado

  useEffect(() => {
    if (!parametros) return
    const get = (clave: string) => parametros.find((p) => p.clave === clave)?.valor
    const mm = get('MODO_MANTENIMIENTO')
    if (mm !== undefined) setModoMantenimiento(mm === 'true')
    const pr = get('PERMITIR_REAPERTURA')
    if (pr !== undefined) setPermitirReapertura(pr === 'true')
  }, [parametros])

  return (
    <div className="px-3 py-3 lg:px-5">
      <div className="mb-4">
        <h2 className="text-base font-semibold tracking-tight">Configuración</h2>
        <p className="text-xs text-muted-foreground">
          Parámetros del sistema, catálogos, empresas y sucursales.
        </p>
      </div>

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
            <ToggleRow
              label={
                <>
                  Modo de mantenimiento
                  <FieldTooltip text="Mientras esté activo, solo el SuperAdministrador puede ingresar al sistema. Los demás usuarios recibirán un error 503." />
                </>
              }
              description="Bloquea el acceso a todos los usuarios excepto superadmin"
              checked={modoMantenimiento}
              onCheckedChange={setModoMantenimiento}
            />
            <div className="flex justify-end pt-1">
              <Button
                size="sm"
                onClick={() =>
                  void guardar(
                    { MODO_MANTENIMIENTO: String(modoMantenimiento) },
                    'Configuración general guardada',
                  )
                }
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
              Configuración de tickets
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4 p-3 pt-0">
            <ToggleRow
              label={
                <>
                  Permitir reapertura de tickets
                  <FieldTooltip text="Permite a los usuarios volver a abrir un ticket cerrado si el problema no quedó resuelto. El superadmin siempre puede reabrir." />
                </>
              }
              description="Los usuarios pueden reabrir tickets cerrados"
              checked={permitirReapertura}
              onCheckedChange={setPermitirReapertura}
            />
            <div className="flex justify-end pt-1">
              <Button
                size="sm"
                onClick={() =>
                  void guardar(
                    { PERMITIR_REAPERTURA: String(permitirReapertura) },
                    'Configuración de tickets guardada',
                  )
                }
              >
                Guardar cambios
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* ── ROLES ───────────────────────────────────────────────────────── */}
        <SeccionRoles />

        {/* ── TIPOS DE SERVICIO ──────────────────────────────────────────── */}
        <SeccionTiposServicio />

        {/* ── CATEGORÍAS ────────────────────────────────────────────────── */}
        <SeccionCategorias />

        {/* ── ORGANIZACIÓN ────────────────────────────────────────────────── */}
        <Card className="lg:col-span-2">
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              <Building2 className="h-3.5 w-3.5 text-blue-500" />
              Organización
            </CardTitle>
          </CardHeader>
          <CardContent className="grid gap-3 p-3 pt-0 sm:grid-cols-2">
            {user?.rol === 'superadmin' && (
              <button
                type="button"
                onClick={() => navigate(ROUTES.EMPRESAS)}
                className="flex items-center justify-between gap-4 rounded-md border border-border bg-muted/30 px-4 py-3 text-left transition-colors hover:bg-accent focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              >
                <div className="flex items-center gap-3">
                  <Building2 className="h-5 w-5 shrink-0 text-muted-foreground" />
                  <div>
                    <p className="text-sm font-medium text-foreground">Gestionar empresas</p>
                    <p className="mt-0.5 text-xs text-muted-foreground">
                      Crear, editar y activar empresas del sistema
                    </p>
                  </div>
                </div>
                <ChevronRight className="h-4 w-4 shrink-0 text-muted-foreground" />
              </button>
            )}
            <button
              type="button"
              onClick={() => navigate(ROUTES.SUCURSALES)}
              className="flex items-center justify-between gap-4 rounded-md border border-border bg-muted/30 px-4 py-3 text-left transition-colors hover:bg-accent focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <div className="flex items-center gap-3">
                <MapPin className="h-5 w-5 shrink-0 text-muted-foreground" />
                <div>
                  <p className="text-sm font-medium text-foreground">Gestionar sucursales</p>
                  <p className="mt-0.5 text-xs text-muted-foreground">
                    Crear, editar y administrar sucursales
                  </p>
                </div>
              </div>
              <ChevronRight className="h-4 w-4 shrink-0 text-muted-foreground" />
            </button>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
