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
}

function FormField({
  label,
  required,
  optional,
  error,
  hint,
  children,
  className,
}: FormFieldProps) {
  return (
    <div className={cn('space-y-1.5', className)}>
      <Label className="text-xs font-medium">
        {label}
        {required && <span className="ml-0.5 text-xs text-destructive">*</span>}
        {optional && !required && (
          <span className="ml-1 text-[10px] font-normal text-muted-foreground">(Opcional)</span>
        )}
      </Label>

      <div className="relative">{children}</div>

      {error && (
        <p className="flex items-center gap-1 text-[11px] text-destructive">
          <AlertCircle className="h-3 w-3 shrink-0" />
          {error}
        </p>
      )}

      {hint && !error && <p className="text-[11px] text-muted-foreground">{hint}</p>}
    </div>
  )
}

export { FormField }
export type { FormFieldProps }
