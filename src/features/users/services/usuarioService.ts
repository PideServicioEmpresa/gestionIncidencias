import { apiClient } from '@services/apiClient'
import type { PagedBackendResponse } from '@services/apiClient'

export interface UsuarioResumenDto {
  id: string
  authId: string
  correo: string
  nombreCompleto: string
  rol: string
  estadoLaboral: string
  activo: boolean
  sucursalId: string
  empresaId: string
}

export interface UsuarioDetalleDto {
  id: string
  authId: string
  correo: string
  nombre: string
  apellido: string
  rol: string
  estadoLaboral: string
  activo: boolean
  telefono: string | null
  sucursalId: string
  empresaId: string
}

export interface UsuarioListParams {
  pagina?: number
  tamanoPagina?: number
  sucursalId?: string
  rol?: string
  soloActivos?: boolean
}

export const usuarioService = {
  listar: (params?: UsuarioListParams) =>
    apiClient.get<PagedBackendResponse<UsuarioResumenDto>>(
      '/usuarios',
      params as Record<string, string | number | boolean | null | undefined>,
    ),

  obtener: (id: string) => apiClient.get<UsuarioDetalleDto>(`/usuarios/${id}`),

  crear: (body: {
    sucursalId: string
    nombre: string
    apellido: string
    correo: string
    nombreUsuario: string
    contrasena: string
    telefono?: string
    rol: string
  }) => apiClient.post<UsuarioDetalleDto>('/usuarios', body),

  actualizarPerfil: (id: string, body: { nombre: string; apellido: string; telefono?: string }) =>
    apiClient.put<UsuarioDetalleDto>(`/usuarios/${id}`, body),

  toggleEstado: (id: string) => apiClient.patch(`/usuarios/${id}/toggle-estado`),
}
