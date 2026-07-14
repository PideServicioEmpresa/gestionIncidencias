import { cn } from '@lib/utils'
import type { TicketStatus } from '@types-app/index'

interface StatusConfig {
  label: string
  className: string
}

const STATUS_CONFIG: Record<TicketStatus, StatusConfig> = {
  sin_asignar: {
    label: 'Sin Asignar',
    className: 'bg-ticket-sin-asignar text-ticket-sin-asignar-foreground border-transparent',
  },
  asignado: {
    label: 'Asignado',
    className: 'bg-ticket-asignado text-ticket-asignado-foreground border-transparent',
  },
  en_proceso: {
    label: 'En Proceso',
    className: 'bg-ticket-en-proceso text-ticket-en-proceso-foreground border-transparent',
  },
  pendiente_validacion: {
    label: 'Pend. Validación',
    className: 'bg-ticket-pendiente text-ticket-pendiente-foreground border-transparent',
  },
  cerrado: {
    label: 'Cerrado',
    className: 'bg-ticket-cerrado text-ticket-cerrado-foreground border-transparent',
  },
  reabierto: {
    label: 'Reabierto',
    className: 'bg-ticket-reabierto text-ticket-reabierto-foreground border-transparent',
  },
  en_espera: {
    label: 'En Espera',
    className:
      'bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400 border-transparent',
  },
  cancelado: {
    label: 'Cancelado',
    className: 'bg-muted text-muted-foreground border-transparent',
  },
}

interface StatusBadgeProps {
  status: TicketStatus
  className?: string
}

export function StatusBadge({ status, className }: StatusBadgeProps) {
  const config = STATUS_CONFIG[status]
  return (
    <span
      className={cn(
        'inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold transition-colors',
        config.className,
        className,
      )}
    >
      {config.label}
    </span>
  )
}
