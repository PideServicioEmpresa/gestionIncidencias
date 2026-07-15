// Rutas de la aplicacion
export const ROUTES = {
  HOME: '/',
  LOGIN: '/login',
  FORGOT_PASSWORD: '/recuperar-contrasena',
  RESET_PASSWORD: '/reset-password',
  DASHBOARD: '/dashboard',
  PROFILE: '/perfil',
  TICKETS: '/tickets',
  TICKETS_NEW: '/tickets/nuevo',
  TICKETS_DETAIL: '/tickets/:id',
  USERS: '/usuarios',
  USERS_NEW: '/usuarios/nuevo',
  USERS_DETAIL: '/usuarios/:id',
  USERS_EDIT: '/usuarios/:id/editar',
  EMPRESAS: '/empresas',
  EMPRESAS_NEW: '/empresas/nueva',
  EMPRESAS_DETAIL: '/empresas/:id',
  EMPRESAS_EDIT: '/empresas/:id/editar',
  SUCURSALES: '/sucursales',
  SUCURSALES_NEW: '/sucursales/nueva',
  SUCURSALES_DETAIL: '/sucursales/:id',
  SUCURSALES_EDIT: '/sucursales/:id/editar',
  WORKERS: '/trabajadores',
  SETTINGS: '/configuracion',
  NOTIFICATIONS: '/notificaciones',
  REPORTS: '/reportes',
  AUDIT: '/auditoria',
  DESIGN_SYSTEM: '/ui',
} as const

export function ticketDetailPath(id: string) {
  return `/tickets/${id}`
}

export function userDetailPath(id: string) {
  return '/usuarios/' + id
}

export function userEditPath(id: string) {
  return '/usuarios/' + id + '/editar'
}

export function empresaDetailPath(id: string) {
  return '/empresas/' + id
}

export function empresaEditPath(id: string) {
  return '/empresas/' + id + '/editar'
}

export function sucursalDetailPath(id: string) {
  return '/sucursales/' + id
}

export function sucursalEditPath(id: string) {
  return '/sucursales/' + id + '/editar'
}

// Paginacion por defecto
export const DEFAULT_PAGE_SIZE = 20
export const MAX_PAGE_SIZE = 100

// Limites de archivos (SDD BE-006)
export const MAX_FILE_SIZE_MB = 10
export const MAX_FILE_SIZE_BYTES = MAX_FILE_SIZE_MB * 1024 * 1024
export const ALLOWED_MIME_TYPES = [
  'image/jpeg',
  'image/png',
  'image/webp',
  'image/gif',
  'application/pdf',
  'video/mp4',
  'video/quicktime',
] as const

// Formato del codigo de ticket (SDD TKT-001)
export const TICKET_CODE_PREFIX = 'PS'

// Tiempo de sesion en segundos (configurable desde BD)
export const SESSION_TIMEOUT_SECONDS = 3600

// Limites de la UI
export const MAX_COMMENT_LENGTH = 2000
export const MAX_TITLE_LENGTH = 150
export const MAX_DESCRIPTION_LENGTH = 5000
