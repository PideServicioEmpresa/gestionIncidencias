import { useEffect, useState } from 'react'
import { QueryClient, QueryClientProvider, QueryCache } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { toast } from 'sonner'
import { Toaster } from 'sonner'
import { ThemeProvider } from '@shared/components/ThemeProvider'
import { ErrorBoundary } from '@shared/components/ErrorBoundary'
import { authService } from '@features/auth/services/authService'
import { ApiClientError } from '@services/apiClient'

// Los errores 401 y 403 se manejan en apiClient (redirect + clearAuth).
// Aquí solo mostramos toast para errores de red u otros errores de queries.
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
      if (
        error instanceof ApiClientError &&
        !['NO_AUTENTICADO', 'SIN_PERMISOS'].includes(error.codigo)
      ) {
        toast.error(error.message)
      } else if (!(error instanceof ApiClientError)) {
        toast.error('Ocurrió un error inesperado. Intenta de nuevo.')
      }
    },
  }),
})

interface ProvidersProps {
  children: React.ReactNode
}

/**
 * Restaura la sesión de Supabase al cargar la app.
 * Muestra un indicador de carga mientras verifica la sesión.
 */
function SessionRestorer({ children }: { children: React.ReactNode }) {
  const [ready, setReady] = useState(false)

  useEffect(() => {
    authService
      .restaurarSesion()
      .catch(() => {
        /* sesión inválida — clearAuth ya fue llamado */
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

  return <>{children}</>
}

export function Providers({ children }: ProvidersProps) {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider defaultTheme="dark" storageKey="pide-servicio-theme">
        <ErrorBoundary>
          <SessionRestorer>{children}</SessionRestorer>
        </ErrorBoundary>
        <Toaster richColors position="top-right" duration={4000} closeButton expand={false} />
      </ThemeProvider>
      {import.meta.env.DEV && <ReactQueryDevtools initialIsOpen={false} />}
    </QueryClientProvider>
  )
}
