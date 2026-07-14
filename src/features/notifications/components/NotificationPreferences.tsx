import { Switch } from '@shared/ui/switch'
import { useState } from 'react'

export function NotificationPreferences() {
  const [emailEnabled, setEmailEnabled] = useState(true)
  const [pushEnabled, setPushEnabled] = useState(false)
  const [ticketUpdates, setTicketUpdates] = useState(true)
  const [slaAlerts, setSlaAlerts] = useState(true)

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between rounded-md border border-border bg-muted/30 px-4 py-3">
        <div>
          <p className="text-sm font-medium">Notificaciones por correo</p>
          <p className="text-xs text-muted-foreground">Recibir alertas en tu correo electrónico</p>
        </div>
        <Switch checked={emailEnabled} onCheckedChange={setEmailEnabled} />
      </div>
      <div className="flex items-center justify-between rounded-md border border-border bg-muted/30 px-4 py-3">
        <div>
          <p className="text-sm font-medium">Notificaciones push</p>
          <p className="text-xs text-muted-foreground">Alertas en tiempo real en el navegador</p>
        </div>
        <Switch checked={pushEnabled} onCheckedChange={setPushEnabled} />
      </div>
      <div className="flex items-center justify-between rounded-md border border-border bg-muted/30 px-4 py-3">
        <div>
          <p className="text-sm font-medium">Actualizaciones de tickets</p>
          <p className="text-xs text-muted-foreground">Notificar cambios de estado y comentarios</p>
        </div>
        <Switch checked={ticketUpdates} onCheckedChange={setTicketUpdates} />
      </div>
      <div className="flex items-center justify-between rounded-md border border-border bg-muted/30 px-4 py-3">
        <div>
          <p className="text-sm font-medium">Alertas de SLA</p>
          <p className="text-xs text-muted-foreground">Avisar cuando un ticket esté por vencer</p>
        </div>
        <Switch checked={slaAlerts} onCheckedChange={setSlaAlerts} />
      </div>
    </div>
  )
}
