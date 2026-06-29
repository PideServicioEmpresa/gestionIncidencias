import { Outlet } from 'react-router-dom'
import { Ticket } from 'lucide-react'

export function AuthLayout() {
  return (
    <div className="flex min-h-screen flex-col bg-muted/30 lg:flex-row">
      {/* Brand panel — desktop only */}
      <div className="hidden flex-1 flex-col items-center justify-center bg-primary p-12 text-primary-foreground lg:flex">
        <div className="max-w-md space-y-6">
          <div className="flex items-center gap-4">
            <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-white/20 backdrop-blur-sm">
              <Ticket className="h-7 w-7" />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight">Pide Servicio</h1>
              <p className="text-sm text-primary-foreground/70">
                Sistema de gestión de incidencias
              </p>
            </div>
          </div>
          <p className="text-lg font-light leading-relaxed text-primary-foreground/90">
            Registra, asigna y resuelve solicitudes de servicio en un solo lugar. Trazabilidad
            completa, en tiempo real.
          </p>
          <ul className="space-y-3 text-sm text-primary-foreground/80">
            {[
              'Tickets con prioridad y seguimiento en tiempo real',
              'Asignación automática por área y sucursal',
              'Historial completo de auditoría',
              'Dashboard con métricas y reportes',
            ].map((item) => (
              <li key={item} className="flex items-start gap-2">
                <span className="mt-0.5 text-white">✓</span>
                {item}
              </li>
            ))}
          </ul>
        </div>
      </div>

      {/* Form panel */}
      <div className="flex flex-1 flex-col items-center justify-center p-6 lg:max-w-md lg:p-12">
        {/* Mobile logo */}
        <div className="mb-8 flex flex-col items-center gap-3 lg:hidden">
          <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary">
            <Ticket className="h-6 w-6 text-primary-foreground" />
          </div>
          <h1 className="text-xl font-bold tracking-tight">Pide Servicio</h1>
        </div>

        <div className="w-full max-w-sm">
          <Outlet />
        </div>
      </div>
    </div>
  )
}
