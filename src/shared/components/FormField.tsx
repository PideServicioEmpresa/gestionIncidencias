import { AlertCircle } from 'lucide-react'
import * as React from 'react'

import { cn } from '@lib/utils'
import { Label } from '@shared/ui/label'

interface FormFieldProps {
  label: React.ReactNode
  required?: boolean
  optional?: boolean
  error?: string
  hint?: string
  children: React.ReactNode
  className?: string
  /**
   * ID explícito del campo asociado al Label (htmlFor).
   * Cuando se omite, se usa un ID auto-generado con useId().
   * El primer hijo directo recibe ese ID si aún no tiene uno propio.
   */
  htmlFor?: string
}

function FormField({
  label,
  required,
  optional,
  error,
  hint,
  children,
  className,
  htmlFor,
}: FormFieldProps) {
  // ID estable: caller puede pasar uno explícito o se genera automáticamente
  const autoId = React.useId()
  const fieldId = htmlFor ?? autoId
  const errorId = error ? `${fieldId}-error` : undefined

  /**
   * Intenta inyectar `id` y `aria-describedby`/`aria-invalid` en el primer
   * hijo de React válido, solo si ese hijo no tiene ya un `id` propio y
   * si es un único hijo directo (evita errores con múltiples children).
   */
  const processedChildren = React.useMemo(() => {
    const childArray = React.Children.toArray(children)

    // Solo procesar cuando hay exactamente un hijo para evitar ambigüedades
    if (childArray.length !== 1) return children

    const first = childArray[0]
    if (!React.isValidElement(first)) return children

    const existingId = (first.props as Record<string, unknown>).id as string | undefined
    const targetId = existingId ?? fieldId

    const extraProps: Record<string, unknown> = {}

    // Inyectar id solo si el hijo no lo tenía
    if (!existingId) {
      extraProps.id = targetId
    }

    // Inyectar aria-describedby e aria-invalid cuando hay error
    if (errorId) {
      const existingDescribedBy = (first.props as Record<string, unknown>)['aria-describedby'] as
        string | undefined
      extraProps['aria-describedby'] = existingDescribedBy
        ? `${existingDescribedBy} ${errorId}`
        : errorId
      extraProps['aria-invalid'] = true
    }

    return React.cloneElement(first as React.ReactElement<Record<string, unknown>>, extraProps)
  }, [children, fieldId, errorId])

  return (
    <div className={cn('space-y-1.5', className)}>
      <Label htmlFor={fieldId} className="text-xs font-medium">
        {label}
        {required && <span className="ml-0.5 text-xs text-destructive">*</span>}
        {optional && !required && (
          <span className="ml-1 text-[10px] font-normal text-muted-foreground">(Opcional)</span>
        )}
      </Label>

      <div className="relative">{processedChildren}</div>

      {error && (
        <p
          id={errorId}
          role="alert"
          className="flex items-center gap-1 text-[11px] text-destructive"
        >
          <AlertCircle className="h-3 w-3 shrink-0" aria-hidden="true" />
          {error}
        </p>
      )}

      {hint && !error && <p className="text-[11px] text-muted-foreground">{hint}</p>}
    </div>
  )
}

export { FormField }
export type { FormFieldProps }
