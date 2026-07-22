import { useState } from 'react'
import { X, Star } from 'lucide-react'
import { cn } from '@lib/utils'
import { Badge } from '@shared/ui/badge'
import { Button } from '@shared/ui/button'
import { SearchableSelect } from '@shared/components/SearchableSelect'
import type { SearchableSelectOption } from '@shared/components/SearchableSelect'

export interface SucursalItem {
  sucursalId: string
  sucursalNombre: string
  esPrincipal: boolean
}

interface Props {
  value: SucursalItem[]
  onChange: (value: SucursalItem[]) => void
  opciones: SearchableSelectOption[]
  loadingOpciones?: boolean
  empresaId?: string
  error?: string
  disabled?: boolean
}

export function SucursalMultiSelector({
  value,
  onChange,
  opciones,
  loadingOpciones = false,
  error,
  disabled = false,
}: Props) {
  const [pendiente, setPendiente] = useState<string>('')

  const asignadasIds = new Set(value.map((s) => s.sucursalId))
  const disponibles = opciones.filter((o) => !asignadasIds.has(o.value))

  function handleAgregar(sucursalId: string) {
    if (!sucursalId || asignadasIds.has(sucursalId)) return
    const opcion = opciones.find((o) => o.value === sucursalId)
    if (!opcion) return

    const esPrimera = value.length === 0
    onChange([...value, { sucursalId, sucursalNombre: opcion.label, esPrincipal: esPrimera }])
    setPendiente('')
  }

  function handleEliminar(sucursalId: string) {
    const filtradas = value.filter((s) => s.sucursalId !== sucursalId)
    // Si se elimina la principal y quedan más, marcar la primera como principal
    const huboPrincipal = value.find((s) => s.sucursalId === sucursalId)?.esPrincipal
    if (huboPrincipal && filtradas.length > 0) {
      filtradas[0] = { ...filtradas[0], esPrincipal: true }
    }
    onChange(filtradas)
  }

  function handleMarcarPrincipal(sucursalId: string) {
    onChange(value.map((s) => ({ ...s, esPrincipal: s.sucursalId === sucursalId })))
  }

  const sinPrincipal = value.length > 0 && !value.some((s) => s.esPrincipal)
  const hayError = !!error || sinPrincipal

  return (
    <div className="space-y-2">
      {/* Selector para agregar sucursales */}
      <div className="flex gap-2">
        <div className="flex-1">
          <SearchableSelect
            options={disponibles}
            value={pendiente}
            onChange={(v) => {
              setPendiente(v)
              handleAgregar(v)
            }}
            placeholder={
              loadingOpciones
                ? 'Cargando...'
                : disponibles.length === 0 && value.length > 0
                  ? 'Todas las sucursales asignadas'
                  : 'Agregar sucursal...'
            }
            searchPlaceholder="Buscar sucursal..."
            emptyMessage="Sin sucursales disponibles."
            disabled={disabled || loadingOpciones || disponibles.length === 0}
          />
        </div>
      </div>

      {/* Chips de sucursales asignadas */}
      {value.length > 0 && (
        <div className="space-y-1.5">
          {value.map((s) => (
            <div
              key={s.sucursalId}
              className={cn(
                'flex items-center justify-between rounded-lg border px-3 py-2',
                s.esPrincipal ? 'border-primary/30 bg-primary/5' : 'border-border bg-muted/20',
              )}
            >
              <div className="flex min-w-0 items-center gap-2">
                <button
                  type="button"
                  onClick={() => handleMarcarPrincipal(s.sucursalId)}
                  disabled={disabled || s.esPrincipal}
                  title={s.esPrincipal ? 'Sucursal principal' : 'Marcar como principal'}
                  className={cn(
                    'shrink-0 transition-colors',
                    s.esPrincipal ? 'text-primary' : 'text-muted-foreground/40 hover:text-primary',
                    disabled && 'pointer-events-none',
                  )}
                >
                  <Star className="h-3.5 w-3.5" fill={s.esPrincipal ? 'currentColor' : 'none'} />
                </button>
                <span className="truncate text-xs font-medium">{s.sucursalNombre}</span>
                {s.esPrincipal && (
                  <Badge className="ml-1 shrink-0 border-transparent bg-primary/15 px-1.5 py-0 text-[10px] text-primary">
                    Principal
                  </Badge>
                )}
              </div>

              {!disabled && (
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="h-6 w-6 shrink-0 text-muted-foreground hover:text-destructive"
                  onClick={() => handleEliminar(s.sucursalId)}
                  title="Quitar sucursal"
                >
                  <X className="h-3.5 w-3.5" />
                </Button>
              )}
            </div>
          ))}
        </div>
      )}

      {/* Mensajes de error */}
      {hayError && (
        <p className="text-[11px] text-destructive">
          {error ?? 'Debe seleccionar una sucursal como principal.'}
        </p>
      )}

      {/* Hint */}
      {!hayError && value.length > 0 && (
        <p className="text-[11px] text-muted-foreground">
          Toca la estrella ★ para cambiar la sucursal principal. La principal define el acceso al
          iniciar sesión.
        </p>
      )}
    </div>
  )
}
