/**
 * Cliente HTTP para la API .NET de PideServicio.
 * Todos los requests incluyen el JWT de Supabase Auth automáticamente.
 * Base URL: VITE_API_URL + /api/v1
 */
import { useAuthStore } from '@store/auth.store'

const API_BASE = `${import.meta.env.VITE_API_URL ?? 'http://localhost:5000'}/api/v1`

// ── Tipos del contrato backend ────────────────────────────────────────────────

export interface BackendResponse<T = unknown> {
  exitoso: boolean
  datos?: T
  error?: BackendError
  traceId?: string
}

export interface BackendError {
  codigo: string
  mensaje: string
  erroresValidacion?: Record<string, string[]>
}

export interface PagedBackendResponse<T> {
  items: T[]
  pagina: number
  tamanoPagina: number
  totalRegistros: number
  totalPaginas: number
  tienePaginaAnterior: boolean
  tienePaginaSiguiente: boolean
  traceId?: string
}

// ── Error tipado ──────────────────────────────────────────────────────────────

export class ApiClientError extends Error {
  constructor(
    public readonly codigo: string,
    message: string,
    public readonly erroresValidacion?: Record<string, string[]>,
    public readonly status?: number,
  ) {
    super(message)
    this.name = 'ApiClientError'
  }
}

const HTTP_ERROR_MESSAGES: Record<number, string> = {
  400: 'Los datos enviados no son válidos.',
  403: 'No tienes permiso para realizar esta acción.',
  404: 'El recurso solicitado no existe.',
  408: 'La solicitud tardó demasiado. Intenta de nuevo.',
  409: 'Conflicto con el estado actual del recurso.',
  422: 'Los datos enviados contienen errores de validación.',
  429: 'Demasiadas solicitudes. Espera un momento e intenta de nuevo.',
  500: 'Error interno del servidor. Intenta más tarde.',
  502: 'El servidor no está disponible temporalmente.',
  503: 'Servicio no disponible. Intenta más tarde.',
  504: 'El servidor tardó demasiado en responder.',
}

function httpErrorMessage(status: number): string {
  return HTTP_ERROR_MESSAGES[status] ?? `Error de comunicación con el servidor (HTTP ${status}).`
}

// ── Función base de request ───────────────────────────────────────────────────

async function request<T>(endpoint: string, init: RequestInit = {}): Promise<T> {
  const token = useAuthStore.getState().accessToken

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...((init.headers as Record<string, string>) ?? {}),
  }

  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  let response: Response
  try {
    response = await fetch(`${API_BASE}${endpoint}`, { ...init, headers })
  } catch {
    throw new ApiClientError(
      'RED_ERROR',
      'No se pudo conectar con el servidor. Verifica tu conexión.',
    )
  }

  // Respuestas vacías (204 No Content)
  if (response.status === 204) {
    return undefined as unknown as T
  }

  // Sesión expirada: limpiar store y redirigir al login
  if (response.status === 401) {
    useAuthStore.getState().clearAuth()
    window.location.href = '/login'
    throw new ApiClientError(
      'NO_AUTENTICADO',
      'Sesión expirada. Inicia sesión de nuevo.',
      undefined,
      401,
    )
  }

  let json: BackendResponse<T>
  try {
    json = await response.json()
  } catch {
    throw new ApiClientError(
      `HTTP_${response.status}`,
      httpErrorMessage(response.status),
      undefined,
      response.status,
    )
  }

  if (!json.exitoso || !response.ok) {
    throw new ApiClientError(
      json.error?.codigo ?? `HTTP_${response.status}`,
      json.error?.mensaje ?? httpErrorMessage(response.status),
      json.error?.erroresValidacion,
      response.status,
    )
  }

  return json.datos as T
}

// ── Helpers de query string ───────────────────────────────────────────────────

type QueryParams = Record<string, string | number | boolean | null | undefined>

function buildUrl(endpoint: string, params?: QueryParams): string {
  if (!params) return endpoint
  const entries = Object.entries(params).filter(([, v]) => v !== null && v !== undefined)
  if (entries.length === 0) return endpoint
  const qs = new URLSearchParams(entries.map(([k, v]) => [k, String(v)])).toString()
  return `${endpoint}?${qs}`
}

// ── API client público ────────────────────────────────────────────────────────

export const apiClient = {
  /** GET con query params opcionales */
  get<T>(endpoint: string, params?: QueryParams): Promise<T> {
    return request<T>(buildUrl(endpoint, params))
  },

  /** POST con body JSON */
  post<T>(endpoint: string, body?: unknown): Promise<T> {
    return request<T>(endpoint, {
      method: 'POST',
      body: body !== undefined ? JSON.stringify(body) : undefined,
    })
  },

  /** PUT con body JSON */
  put<T>(endpoint: string, body?: unknown): Promise<T> {
    return request<T>(endpoint, {
      method: 'PUT',
      body: body !== undefined ? JSON.stringify(body) : undefined,
    })
  },

  /** PATCH con body JSON opcional */
  patch<T>(endpoint: string, body?: unknown): Promise<T> {
    return request<T>(endpoint, {
      method: 'PATCH',
      body: body !== undefined ? JSON.stringify(body) : undefined,
    })
  },

  /** DELETE */
  delete<T>(endpoint: string): Promise<T> {
    return request<T>(endpoint, { method: 'DELETE' })
  },

  /** POST multipart/form-data (upload de archivos) */
  async upload<T>(endpoint: string, formData: FormData): Promise<T> {
    const token = useAuthStore.getState().accessToken
    const headers: Record<string, string> = {}
    if (token) headers['Authorization'] = `Bearer ${token}`

    let response: Response
    try {
      response = await fetch(`${API_BASE}${endpoint}`, {
        method: 'POST',
        headers,
        body: formData,
      })
    } catch {
      throw new ApiClientError('RED_ERROR', 'No se pudo conectar con el servidor.')
    }

    if (response.status === 401) {
      useAuthStore.getState().clearAuth()
      window.location.href = '/login'
      throw new ApiClientError(
        'NO_AUTENTICADO',
        'Sesión expirada. Inicia sesión de nuevo.',
        undefined,
        401,
      )
    }

    let json: BackendResponse<T>
    try {
      json = await response.json()
    } catch {
      throw new ApiClientError(
        `HTTP_${response.status}`,
        httpErrorMessage(response.status),
        undefined,
        response.status,
      )
    }

    if (!json.exitoso || !response.ok) {
      throw new ApiClientError(
        json.error?.codigo ?? `HTTP_${response.status}`,
        json.error?.mensaje ?? httpErrorMessage(response.status),
        json.error?.erroresValidacion,
        response.status,
      )
    }

    return json.datos as T
  },
}
