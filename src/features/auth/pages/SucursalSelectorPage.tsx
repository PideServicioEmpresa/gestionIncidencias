import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { Building2 } from 'lucide-react'
import { useAuthStore } from '@store/auth.store'
import { SearchableSelect } from '@shared/components/SearchableSelect'
import { Button } from '@shared/ui/button'
import { ROUTES } from '@constants/index'

const ROLES_MULTI_SUCURSAL = ['tecnico', 'trabajador', 'usuario']

export function SucursalSelectorPage() {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const sucursalesDisponibles = useAuthStore((s) => s.sucursalesDisponibles)
  const sucursalActiva = useAuthStore((s) => s.sucursalActiva)
  const cambiarSucursalActiva = useAuthStore((s) => s.cambiarSucursalActiva)

  const esMultiSucursal = user?.rol ? ROLES_MULTI_SUCURSAL.includes(user.rol) : false
  const sucursalesActivas = sucursalesDisponibles.filter((s) => s.activo)

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

  const opciones = sucursalesActivas.map((s) => ({
    value: s.sucursalId,
    label: s.sucursalNombre,
  }))

  const handleContinuar = () => {
    if (!seleccionada) return
    cambiarSucursalActiva(seleccionada)
    navigate(ROUTES.DASHBOARD, { replace: true })
  }

  return (
    <div className="flex min-h-dvh flex-col items-center justify-center bg-background p-4">
      <div className="w-full max-w-sm space-y-6">
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

        <div className="space-y-3">
          <SearchableSelect
            options={opciones}
            value={seleccionada}
            onChange={setSeleccionada}
            placeholder="Seleccionar sucursal..."
            searchPlaceholder="Buscar sucursal..."
            emptyMessage="No se encontró la sucursal."
          />
          <Button className="w-full" disabled={!seleccionada} onClick={handleContinuar}>
            Continuar
          </Button>
        </div>
      </div>
    </div>
  )
}
