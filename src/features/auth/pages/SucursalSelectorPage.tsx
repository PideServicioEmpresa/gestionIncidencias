import { useState, useEffect, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { Building2, Search, Check } from 'lucide-react'
import { useAuthStore } from '@store/auth.store'
import type { SucursalPerfil } from '@types-app/index'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Badge } from '@shared/ui/badge'
import { ScrollArea } from '@shared/ui/scroll-area'
import { cn } from '@lib/utils'
import { ROUTES } from '@constants/index'

const ROLES_MULTI_SUCURSAL = ['tecnico', 'trabajador', 'usuario']

// ── Ítem de sucursal ──────────────────────────────────────────────────────────
// El slot `notificacionesPendientes` queda reservado para la Fase 5.
// Cuando el backend provea el conteo por sucursal, solo hay que pasar el número.
// TODO (Fase 5): recibir conteo de notificaciones pendientes por sucursal desde el backend

interface SucursalItemProps {
  sucursal: SucursalPerfil
  seleccionada: boolean
  onSeleccionar: (id: string) => void
  notificacionesPendientes?: number // reservado — no implementado hasta Fase 5
}

function SucursalItem({
  sucursal,
  seleccionada,
  onSeleccionar,
  notificacionesPendientes,
}: SucursalItemProps) {
  return (
    <button
      type="button"
      onClick={() => onSeleccionar(sucursal.sucursalId)}
      className={cn(
        'flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-left text-sm transition-all',
        seleccionada ? 'bg-primary/10 ring-1 ring-primary/30' : 'hover:bg-accent',
      )}
    >
      {/* Icono + nombre */}
      <Building2
        className={cn('h-4 w-4 shrink-0', seleccionada ? 'text-primary' : 'text-muted-foreground')}
      />

      <div className="min-w-0 flex-1">
        <span
          className={cn(
            'truncate text-sm font-medium',
            seleccionada ? 'text-primary' : 'text-foreground',
          )}
        >
          {sucursal.sucursalNombre}
        </span>
        {sucursal.esPrincipal && (
          <Badge variant="secondary" className="ml-2 px-1.5 py-0 text-[10px]">
            Principal
          </Badge>
        )}
      </div>

      {/* Slot de notificaciones pendientes — Fase 5 */}
      {notificacionesPendientes !== undefined && notificacionesPendientes > 0 && (
        <Badge className="ml-auto shrink-0 bg-destructive text-destructive-foreground">
          {notificacionesPendientes > 99 ? '99+' : notificacionesPendientes}
        </Badge>
      )}

      {/* Checkmark de selección */}
      <Check
        className={cn('h-4 w-4 shrink-0', seleccionada ? 'text-primary opacity-100' : 'opacity-0')}
      />
    </button>
  )
}

// ── Pantalla principal ────────────────────────────────────────────────────────

export function SucursalSelectorPage() {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const sucursalesDisponibles = useAuthStore((s) => s.sucursalesDisponibles)
  const sucursalActiva = useAuthStore((s) => s.sucursalActiva)
  const cambiarSucursalActiva = useAuthStore((s) => s.cambiarSucursalActiva)

  const esMultiSucursal = user?.rol ? ROLES_MULTI_SUCURSAL.includes(user.rol) : false
  const sucursalesActivas = useMemo(
    () => sucursalesDisponibles.filter((s) => s.activo),
    [sucursalesDisponibles],
  )

  const [busqueda, setBusqueda] = useState('')
  const [seleccionada, setSeleccionada] = useState<string>(
    () =>
      sucursalActiva ??
      sucursalesActivas.find((s) => s.esPrincipal)?.sucursalId ??
      sucursalesActivas[0]?.sucursalId ??
      '',
  )

  useEffect(() => {
    if (!esMultiSucursal) {
      navigate(ROUTES.DASHBOARD, { replace: true })
      return
    }
    if (sucursalesActivas.length === 1) {
      cambiarSucursalActiva(sucursalesActivas[0].sucursalId)
      navigate(ROUTES.DASHBOARD, { replace: true })
    }
  }, [esMultiSucursal, sucursalesActivas.length, cambiarSucursalActiva, navigate]) // eslint-disable-line react-hooks/exhaustive-deps

  const sucursalesFiltradas = useMemo(() => {
    const q = busqueda.trim().toLowerCase()
    if (!q) return sucursalesActivas
    return sucursalesActivas.filter((s) => s.sucursalNombre.toLowerCase().includes(q))
  }, [sucursalesActivas, busqueda])

  const handleContinuar = () => {
    if (!seleccionada) return
    cambiarSucursalActiva(seleccionada)
    navigate(ROUTES.DASHBOARD, { replace: true })
  }

  return (
    <div className="flex min-h-dvh flex-col items-center justify-center bg-background p-4">
      <div className="w-full max-w-sm space-y-5">
        {/* Encabezado */}
        <div className="flex flex-col items-center gap-3 text-center">
          <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary/10">
            <Building2 className="h-6 w-6 text-primary" />
          </div>
          <div>
            <h1 className="text-xl font-semibold">Selecciona tu sucursal</h1>
            <p className="mt-1 text-sm text-muted-foreground">
              Elige la sucursal en la que vas a trabajar en esta sesión.
            </p>
          </div>
        </div>

        {/* Búsqueda */}
        <div className="relative">
          <Search className="absolute left-2.5 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Buscar sucursal..."
            value={busqueda}
            onChange={(e) => setBusqueda(e.target.value)}
            className="pl-8 text-sm"
            autoFocus
          />
        </div>

        {/* Lista de sucursales */}
        <ScrollArea className="max-h-64 rounded-lg border">
          <div className="space-y-0.5 p-1.5">
            {sucursalesFiltradas.length === 0 ? (
              <p className="px-3 py-4 text-center text-sm text-muted-foreground">
                No se encontró ninguna sucursal.
              </p>
            ) : (
              sucursalesFiltradas.map((s) => (
                <SucursalItem
                  key={s.sucursalId}
                  sucursal={s}
                  seleccionada={seleccionada === s.sucursalId}
                  onSeleccionar={setSeleccionada}
                  // notificacionesPendientes — Fase 5: pasar conteo desde el backend
                />
              ))
            )}
          </div>
        </ScrollArea>

        {/* Acción */}
        <Button className="w-full" disabled={!seleccionada} onClick={handleContinuar}>
          Continuar
        </Button>
      </div>
    </div>
  )
}
