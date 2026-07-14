// ── Roles del sistema ─────────────────────────────────────────────────────────
// Valores en minúsculas (mapeados desde los enums uppercase del backend).
// Los roles del backend SUPERVISOR, TECNICO, TRABAJADOR se mapean a equivalentes
// más amplios para mantener compatibilidad con el frontend existente.
export type UserRole = 'superadmin' | 'admin' | 'supervisor' | 'tecnico' | 'trabajador' | 'usuario'

// Estado laboral de trabajadores y administradores
export type LaborStatus = 'activo' | 'vacaciones' | 'licencia' | 'suspendido' | 'retirado'

// Estados del workflow de tickets (alineados con el backend)
export type TicketStatus =
  | 'sin_asignar'
  | 'asignado'
  | 'en_proceso'
  | 'en_espera'
  | 'pendiente_validacion'
  | 'cerrado'
  | 'reabierto'
  | 'cancelado'

export type TicketPriority = 'baja' | 'media' | 'alta' | 'critica'

export type EvidenceType = 'inicial' | 'final'

// ── Perfil de usuario autenticado ─────────────────────────────────────────────
// Campos adicionales (empresaId, sucursalIds, permisos) incorporados al integrar
// con el endpoint GET /api/v1/auth/me.
export interface AppUser {
  id: string
  authUserId: string
  nombre: string
  apellido: string
  correo: string
  usuario: string
  telefono?: string
  rolId: string
  rol: UserRole
  empresaId?: string
  sucursalId: string
  sucursalIds?: string[]
  areaId?: string
  estadoLaboral: LaborStatus
  activo: boolean
  ultimoAcceso?: string
  foto?: string
  permisos?: {
    esSuperAdmin: boolean
    esAdmin: boolean
    esSupervisor: boolean
    esTecnico: boolean
    esTrabajador: boolean
    esUsuario: boolean
    tieneAccesoAdministrativo: boolean
    puedeManejarTickets: boolean
  }
}

// ── Respuestas del backend .NET ───────────────────────────────────────────────
// Contrato ApiResponse<T> de PideServicio.Contracts
export interface ApiSuccess<T = unknown> {
  success: true
  message: string
  data: T
}

export interface ApiError {
  success: false
  code: 'VALIDATION_ERROR' | 'FORBIDDEN' | 'INTERNAL_ERROR' | 'NOT_FOUND'
  message: string
  errors?: string[]
}

export type ApiResult<T = unknown> = ApiSuccess<T> | ApiError

// ── Paginación ────────────────────────────────────────────────────────────────
export interface PaginatedResult<T> {
  data: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

export interface PaginationParams {
  page?: number
  pageSize?: number
  search?: string
  orderBy?: string
  orderDir?: 'asc' | 'desc'
}

// ── DTOs del backend (alineados con PideServicio.Application DTOs) ─────────────

export interface PerfilBackend {
  id: string
  correo: string
  nombreCompleto: string
  rol: string
  empresaId: string
  sucursalId: string
  activo: boolean
  permisos: {
    esSuperAdmin: boolean
    esAdmin: boolean
    esSupervisor: boolean
    esTecnico: boolean
    esTrabajador: boolean
    esUsuario: boolean
    tieneAccesoAdministrativo: boolean
    puedeManejarTickets: boolean
  }
}

// Mapa de roles backend → frontend
export const ROL_MAP: Record<string, UserRole> = {
  SUPERADMIN: 'superadmin',
  ADMIN: 'admin',
  SUPERVISOR: 'supervisor',
  TECNICO: 'tecnico',
  TRABAJADOR: 'trabajador',
  USUARIO: 'usuario',
}
