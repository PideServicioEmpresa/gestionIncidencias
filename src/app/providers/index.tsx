import { useEffect, useState } from 'react'
import { QueryClient, QueryClientProvider, QueryCache } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { toast } from 'sonner'
import { Toaster } from 'sonner'
import { Wrench } from 'lucide-react'
import { ThemeProvider } from '@shared/components/ThemeProvider'
import { ErrorBoundary } from '@shared/components/ErrorBoundary'
import { authService } from '@features/auth/services/authService'
import { ApiClientError } from '@services/apiClient'
import { usePreferencesStore } from '@store/preferences.store'
import type { AccentColor } from '@store/preferences.store'

// Los errores 401 y 403 se manejan en apiClient (redirect + clearAuth).
// Los errores de mantenimiento se muestran como aviso, no como error rojo.
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5,
      gcTime: 1000 * 60 * 10,
      retry: 1,
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: 0,
    },
  },
  queryCache: new QueryCache({
    onError: (error) => {
      if (error instanceof ApiClientError) {
        if (error.codigo === 'SISTEMA_EN_MANTENIMIENTO') {
          toast.warning(error.message, { duration: 8000 })
        } else if (!['NO_AUTENTICADO', 'SIN_PERMISOS'].includes(error.codigo)) {
          toast.error(error.message)
        }
      } else {
        toast.error('Ocurrió un error inesperado. Intenta de nuevo.')
      }
    },
  }),
})

// Mapa HSL por color de acento — inline style gana sobre cualquier regla CSS
const ACCENT_HSL: Record<AccentColor, string> = {
  blue: '217 91% 60%',
  violet: '263 70% 60%',
  green: '142 71% 45%',
  orange: '25 95% 58%',
  red: '0 84% 60%',
  yellow: '45 93% 52%',
}

function AccentApplier() {
  const accentColor = usePreferencesStore((s) => s.accentColor)

  useEffect(() => {
    const hsl = ACCENT_HSL[accentColor] ?? ACCENT_HSL.blue
    document.documentElement.style.setProperty('--primary', hsl)
    document.documentElement.style.setProperty('--ring', hsl)
    document.documentElement.style.setProperty('--sidebar-primary', hsl)
  }, [accentColor])

  return null
}

interface ProvidersProps {
  children: React.ReactNode
}

function MantenimientoScreen() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-5 bg-background p-8">
      <div className="flex h-16 w-16 items-center justify-center rounded-full bg-amber-100 dark:bg-amber-950/40">
        <Wrench className="h-8 w-8 text-amber-600 dark:text-amber-500" aria-hidden />
      </div>
      <div className="max-w-xs text-center">
        <h1 className="text-lg font-bold text-foreground">Sistema en mantenimiento</h1>
        <p className="mt-2 text-sm leading-relaxed text-muted-foreground">
          El sistema está temporalmente en mantenimiento. Por favor, vuelve a intentarlo en unos
          minutos. Disculpa las molestias.
        </p>
      </div>
      <button
        className="text-sm font-medium text-primary hover:underline focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
        onClick={() => window.location.reload()}
      >
        Reintentar
      </button>
    </div>
  )
}

/**
 * Restaura la sesión de Supabase al cargar la app.
 * Muestra un indicador de carga mientras verifica la sesión.
 * Si el backend está en mantenimiento, muestra la pantalla correspondiente.
 */
function SessionRestorer({ children }: { children: React.ReactNode }) {
  const [ready, setReady] = useState(false)
  const [enMantenimiento, setEnMantenimiento] = useState(false)

  useEffect(() => {
    authService
      .restaurarSesion()
      .catch((err) => {
        if (err instanceof ApiClientError && err.codigo === 'SISTEMA_EN_MANTENIMIENTO') {
          setEnMantenimiento(true)
        }
        // Otros errores: clearAuth ya fue llamado dentro de restaurarSesion
      })
      .finally(() => setReady(true))

    return authService.onAuthStateChange(() => {
      /* store ya actualizado dentro del servicio */
    })
  }, [])

  if (!ready) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background">
        <div
          className="h-8 w-8 animate-spin rounded-full border-2 border-primary border-t-transparent"
          role="status"
          aria-label="Cargando..."
        />
      </div>
    )
  }

  if (enMantenimiento) return <MantenimientoScreen />

  return <>{children}</>
}

export function Providers({ children }: ProvidersProps) {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider defaultTheme="dark" storageKey="pide-servicio-theme">
        <AccentApplier />
        <ErrorBoundary>
          <SessionRestorer>{children}</SessionRestorer>
        </ErrorBoundary>
        <Toaster richColors position="top-right" duration={4000} closeButton expand={false} />
      </ThemeProvider>
      {import.meta.env.DEV && <ReactQueryDevtools initialIsOpen={false} />}
    </QueryClientProvider>
  )
}
