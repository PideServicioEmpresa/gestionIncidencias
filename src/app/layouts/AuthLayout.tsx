import { Outlet } from 'react-router-dom'
import { CheckCircle2 } from 'lucide-react'

export function AuthLayout() {
  return (
    <div className="flex min-h-screen flex-col bg-gradient-to-br from-background via-background to-muted/30 lg:flex-row">
      {/* Brand panel — desktop only */}
      <div className="relative hidden flex-1 flex-col items-center justify-center overflow-hidden bg-primary p-12 text-primary-foreground lg:flex">
        {/* Animated background circles — brand panel */}
        <div
          className="ps-circle-bg ps-animate-float-a h-96 w-96 bg-white"
          style={{ top: '-80px', left: '-60px' }}
        />
        <div
          className="ps-circle-bg ps-animate-float-b h-80 w-80 bg-white"
          style={{ bottom: '-60px', right: '-40px' }}
        />
        <div
          className="ps-circle-bg ps-animate-float-c h-64 w-64 bg-white"
          style={{ top: '40%', left: '30%' }}
        />

        <div className="relative z-10 max-w-md space-y-6">
          <div className="flex items-center gap-4">
            <img
              src="/logo.jpeg"
              alt="Pidde Servicio"
              className="h-14 w-14 rounded-2xl object-contain"
            />
            <div>
              <h1 className="text-xl font-bold tracking-tight">Pidde Servicio</h1>
              <p className="text-xs text-primary-foreground/70">
                Sistema de gestión de incidencias
              </p>
            </div>
          </div>
          <p className="text-xs leading-relaxed text-primary-foreground/90">
            Registra, asigna y resuelve solicitudes de servicio en un solo lugar. Trazabilidad
            completa, en tiempo real.
          </p>
          <ul className="space-y-3 text-xs text-primary-foreground/80">
            {[
              'Tickets con prioridad y seguimiento en tiempo real',
              'Asignación automática por área y sucursal',
              'Historial completo de auditoría',
              'Dashboard con métricas y reportes',
            ].map((item) => (
              <li key={item} className="flex items-start gap-2">
                <CheckCircle2 className="mt-0.5 h-3.5 w-3.5 shrink-0 text-white/80" />
                {item}
              </li>
            ))}
          </ul>
        </div>
      </div>

      {/* Form panel */}
      <div className="relative flex flex-1 flex-col items-center justify-center overflow-hidden p-5 lg:p-10">
        {/* Animated background circles — form panel */}
        <div
          className="ps-circle-bg ps-animate-float-b h-96 w-96 bg-primary"
          style={{ top: '-100px', right: '-80px' }}
        />
        <div
          className="ps-circle-bg ps-animate-float-a h-80 w-80 bg-primary"
          style={{ bottom: '-80px', left: '-60px' }}
        />
        <div
          className="ps-circle-bg ps-animate-float-c h-64 w-64 bg-primary"
          style={{ top: '50%', left: '50%', transform: 'translate(-50%, -50%)' }}
        />

        {/* Mobile logo */}
        <div className="relative z-10 mb-8 flex flex-col items-center gap-3 lg:hidden">
          <img
            src="/logo.jpeg"
            alt="Pidde Servicio"
            className="h-12 w-12 rounded-xl object-contain"
          />
          <h1 className="text-base font-bold tracking-tight">Pidde Servicio</h1>
        </div>

        <div className="relative z-10 w-full max-w-sm lg:max-w-md">
          <Outlet />
        </div>
      </div>
    </div>
  )
}
