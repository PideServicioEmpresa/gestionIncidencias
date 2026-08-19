/**
 * Regresión: el Dashboard debe renderizar sin errores con la base sin tickets.
 * Recharts debe renderizar de verdad para cubrir el caso, de ahí el polyfill de
 * ResizeObserver y las dimensiones simuladas.
 */
import { describe, it, vi, beforeEach, expect } from 'vitest'
import { render } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useAuthStore } from '@store/auth.store'

class ResizeObserverPolyfill {
  callback: ResizeObserverCallback
  constructor(cb: ResizeObserverCallback) {
    this.callback = cb
  }
  observe() {
    // Reporta un tamaño real para que ResponsiveContainer renderice sus hijos
    this.callback(
      [{ contentRect: { width: 800, height: 400 } } as unknown as ResizeObserverEntry],
      this as unknown as ResizeObserver,
    )
  }
  unobserve() {}
  disconnect() {}
}
globalThis.ResizeObserver = ResizeObserverPolyfill as unknown as typeof ResizeObserver

// Dimensiones no nulas para todo el layout
Object.defineProperty(HTMLElement.prototype, 'offsetWidth', { configurable: true, value: 800 })
Object.defineProperty(HTMLElement.prototype, 'offsetHeight', { configurable: true, value: 400 })
Object.defineProperty(HTMLElement.prototype, 'clientWidth', { configurable: true, value: 800 })
Object.defineProperty(HTMLElement.prototype, 'clientHeight', { configurable: true, value: 400 })

const RESUMEN_VACIO = {
  totalAbiertos: 0,
  totalCerrados: 0,
  total: 0,
  criticos: 0,
  cerradosHoy: 0,
  tasaResolucionPct: 0,
  porEstado: [],
  porPrioridad: [],
  porSucursal: [],
  porArea: [],
  porTecnico: [],
  porTipoServicio: [],
  tendencia16Dias: [],
  tendenciaSemanal: [],
  sparkAbiertos: [],
  sparkCriticos: [],
  sparkCerrados: [],
}

vi.mock('@services/apiClient', () => ({
  apiClient: {
    get: vi.fn(async (endpoint: string) => {
      if (endpoint.includes('/dashboard/resumen')) return RESUMEN_VACIO
      return { items: [], pagina: 1, tamanoPagina: 10, totalRegistros: 0, totalPaginas: 0 }
    }),
    post: vi.fn(),
    patch: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
  ApiClientError: class ApiClientError extends Error {},
}))

import { DashboardPage } from './DashboardPage'

describe('Dashboard vacío con Recharts renderizando de verdad', () => {
  beforeEach(() => {
    useAuthStore.setState({
      user: {
        id: '11111111-1111-1111-1111-111111111111',
        authUserId: '22222222-2222-2222-2222-222222222222',
        nombre: 'Milagros',
        apellido: 'Maco',
        correo: 'milagros@inmoveg.pe',
        usuario: 'milagros',
        rolId: 'SUPERADMIN',
        rol: 'superadmin',
        empresaId: '33333333-3333-3333-3333-333333333333',
        sucursalId: '00000000-0000-0000-0000-000000000000',
        sucursalIds: [],
        estadoLaboral: 'activo',
        activo: true,
        permisos: [],
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
      } as any,
      accessToken: 'token-de-prueba',
      isAuthenticated: true,
      sucursalActiva: null,
      sucursalesDisponibles: [],
      sucursalConfirmada: true,
    })
  })

  it('no debe lanzar TypeError al renderizar los gráficos', async () => {
    const qc = new QueryClient({ defaultOptions: { queries: { retry: false, gcTime: 0 } } })
    const errores: unknown[] = []

    // Captura los errores que React relanza durante el render
    const onError = (e: ErrorEvent) => errores.push(e.error)
    window.addEventListener('error', onError)

    try {
      render(
        <QueryClientProvider client={qc}>
          <MemoryRouter initialEntries={['/dashboard']}>
            <DashboardPage />
          </MemoryRouter>
        </QueryClientProvider>,
      )
      await new Promise((r) => setTimeout(r, 2000))
    } catch (e) {
      errores.push(e)
    }
    window.removeEventListener('error', onError)

    if (errores.length > 0) {
      // eslint-disable-next-line no-console
      console.log('=== ERRORES CAPTURADOS ===')
      for (const e of errores) {
        // eslint-disable-next-line no-console
        console.log(e instanceof Error ? `${e.message}\n${e.stack}` : String(e))
      }
    }
    expect(errores).toHaveLength(0)
  })
})
