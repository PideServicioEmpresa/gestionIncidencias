import { useState } from 'react'
import { Plus, X } from 'lucide-react'
import { toast } from 'sonner'
import { EmailChipsInput } from '@shared/components/EmailChipsInput'
import { Badge } from '@shared/ui/badge'
import {
  useCorreosGuardados,
  useAgregarCorreoGuardado,
  useEliminarCorreoGuardado,
} from '@features/correos-guardados/hooks/useCorreosGuardados'

interface Props {
  value: string[]
  onChange: (value: string[]) => void
  maxItems?: number
  error?: string
  disabled?: boolean
  placeholder?: string
}

export function EmailChipsInputConGuardados({
  value,
  onChange,
  maxItems = 5,
  error,
  disabled = false,
  placeholder,
}: Props) {
  const [correosParaGuardar, setCorreosParaGuardar] = useState<string[]>([])

  const { data: guardados = [], isLoading } = useCorreosGuardados()
  const agregar = useAgregarCorreoGuardado()
  const eliminar = useEliminarCorreoGuardado()

  // Mostrar hasta 5 correos guardados
  const sugerenciasFiltradas = guardados.slice(0, 5)
  const tieneGuardados = guardados.length > 0

  function handleAgregarAlCampo(correo: string) {
    if (value.includes(correo)) return
    onChange([...value, correo])
  }

  function handleEliminarGuardado(id: string) {
    eliminar.mutate(id, {
      onError: () => {
        toast.error('No se pudo eliminar el correo guardado.')
      },
    })
  }

  function handleGuardarCorreo(correo: string) {
    agregar.mutate(correo, {
      onSuccess: () => {
        setCorreosParaGuardar((prev) => prev.filter((c) => c !== correo))
      },
      onError: () => {
        toast.error('No se pudo guardar el correo.')
      },
    })
  }

  // Detectar correos nuevos añadidos manualmente que no están en guardados
  function handleChange(nuevosEmails: string[]) {
    onChange(nuevosEmails)

    const correosGuardadosSet = new Set(guardados.map((g) => g.correo))
    const nuevosNoGuardados = nuevosEmails.filter(
      (e) => !correosGuardadosSet.has(e) && !correosParaGuardar.includes(e),
    )

    if (nuevosNoGuardados.length > 0) {
      setCorreosParaGuardar((prev) => {
        const set = new Set(prev)
        nuevosNoGuardados.forEach((e) => set.add(e))
        return Array.from(set)
      })
    }

    // Limpiar sugerencias de guardar para correos que ya no están en value
    setCorreosParaGuardar((prev) => prev.filter((c) => nuevosEmails.includes(c)))
  }

  // Correos que se pueden guardar: están en value, no están en guardados, y no se están guardando ya
  const correosGuardadosSet = new Set(guardados.map((g) => g.correo))
  const correosPendientesDeGuardar = correosParaGuardar.filter(
    (c) => value.includes(c) && !correosGuardadosSet.has(c),
  )

  return (
    <div className="space-y-2">
      <EmailChipsInput
        value={value}
        onChange={handleChange}
        maxItems={maxItems}
        error={error}
        disabled={disabled}
        placeholder={placeholder}
      />

      {/* Opción de guardar correos nuevos */}
      {correosPendientesDeGuardar.map((correo) => (
        <div key={correo} className="flex items-center gap-1">
          {agregar.isPending && agregar.variables === correo ? (
            <span className="text-[11px] text-muted-foreground">Guardando...</span>
          ) : (
            <button
              type="button"
              onClick={() => handleGuardarCorreo(correo)}
              className="text-[11px] text-primary underline transition-colors hover:text-primary/80"
            >
              Guardar &quot;{correo}&quot; para usar en otros tickets
            </button>
          )}
        </div>
      ))}

      {/* Sección de correos guardados */}
      {!isLoading && tieneGuardados && sugerenciasFiltradas.length > 0 && (
        <div className="space-y-1.5">
          <p className="text-[11px] font-medium text-muted-foreground">Correos guardados</p>
          <div className="flex flex-col gap-1">
            {sugerenciasFiltradas.map((guardado) => {
              const yaEnCampo = value.includes(guardado.correo)
              const isEliminando = eliminar.isPending && eliminar.variables === guardado.id

              return (
                <Badge
                  key={guardado.id}
                  variant="secondary"
                  className="flex w-fit items-center gap-1 pr-1 text-[11px] font-normal"
                >
                  <span className="max-w-[200px] truncate">{guardado.correo}</span>

                  {/* Botón + para agregar al campo */}
                  <button
                    type="button"
                    onClick={() => handleAgregarAlCampo(guardado.correo)}
                    disabled={yaEnCampo || disabled || value.length >= maxItems}
                    className="ml-0.5 shrink-0 rounded-full p-0.5 text-muted-foreground transition-colors hover:bg-primary/15 hover:text-primary disabled:cursor-not-allowed disabled:opacity-50"
                    aria-label={`Agregar ${guardado.correo}`}
                  >
                    <Plus className="h-3 w-3" />
                  </button>

                  {/* Botón × para eliminar de guardados */}
                  <button
                    type="button"
                    onClick={() => handleEliminarGuardado(guardado.id)}
                    disabled={isEliminando}
                    className="ml-0.5 shrink-0 rounded-full p-0.5 text-muted-foreground transition-colors hover:bg-destructive/15 hover:text-destructive disabled:cursor-not-allowed disabled:opacity-50"
                    aria-label={`Eliminar ${guardado.correo} de guardados`}
                  >
                    <X className="h-3 w-3" />
                  </button>
                </Badge>
              )
            })}
          </div>
        </div>
      )}
    </div>
  )
}
