import { useState } from 'react'
import { X } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { SearchableSelect } from '@shared/components/SearchableSelect'
import type { SearchableSelectOption } from '@shared/components/SearchableSelect'

export interface EspecialidadItem {
  especialidadId: string
  especialidadNombre: string
}

interface Props {
  value: EspecialidadItem[]
  onChange: (value: EspecialidadItem[]) => void
  opciones: SearchableSelectOption[]
  loadingOpciones?: boolean
  error?: string
  disabled?: boolean
}

/**
 * Selector múltiple de especialidades. A diferencia de SucursalMultiSelector no hay
 * jerarquía: ninguna especialidad es "principal" y la lista puede quedar vacía.
 */
export function EspecialidadMultiSelector({
  value,
  onChange,
  opciones,
  loadingOpciones = false,
  error,
  disabled = false,
}: Props) {
  const [pendiente, setPendiente] = useState<string>('')

  const asignadasIds = new Set(value.map((e) => e.especialidadId))
  const disponibles = opciones.filter((o) => !asignadasIds.has(o.value))

  function handleAgregar(especialidadId: string) {
    if (!especialidadId || asignadasIds.has(especialidadId)) return
    const opcion = opciones.find((o) => o.value === especialidadId)
    if (!opcion) return

    onChange([...value, { especialidadId, especialidadNombre: opcion.label }])
    setPendiente('')
  }

  function handleEliminar(especialidadId: string) {
    onChange(value.filter((e) => e.especialidadId !== especialidadId))
  }

  return (
    <div className="space-y-2">
      {/* Selector para agregar */}
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
              ? 'Todas las especialidades asignadas'
              : 'Agregar especialidad...'
        }
        searchPlaceholder="Buscar especialidad..."
        emptyMessage="Sin especialidades disponibles."
        disabled={disabled || loadingOpciones || disponibles.length === 0}
      />

      {/* Especialidades asignadas */}
      {value.length > 0 && (
        <div className="space-y-1.5">
          {value.map((e) => (
            <div
              key={e.especialidadId}
              className="flex items-center justify-between rounded-lg border border-border bg-muted/20 px-3 py-2"
            >
              <span className="truncate text-xs font-medium">{e.especialidadNombre}</span>
              {!disabled && (
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="h-6 w-6 shrink-0 text-muted-foreground hover:text-destructive"
                  onClick={() => handleEliminar(e.especialidadId)}
                  title="Quitar especialidad"
                >
                  <X className="h-3.5 w-3.5" />
                </Button>
              )}
            </div>
          ))}
        </div>
      )}

      {error && <p className="text-[11px] text-destructive">{error}</p>}

      {!error && (
        <p className="text-[11px] text-muted-foreground">
          Campo opcional. Se muestran como referencia al asignar tickets.
        </p>
      )}
    </div>
  )
}
