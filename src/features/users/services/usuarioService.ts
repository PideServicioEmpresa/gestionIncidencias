import { apiClient } from '@services/apiClient'
import type { PagedBackendResponse } from '@services/apiClient'
import { supabase } from '@services/supabase'

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
  empresaId: string
  sucursalId: string
  areaId: string | null
  nombre: string
  apellido: string
  nombreCompleto: string
  correo: string
  nombreUsuario: string
  telefono: string | null
  rol: string
  estadoLaboral: string
  activo: boolean
  fotoUrl: string | null
  ultimoAcceso: string | null
  createdAt: string
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

  actualizarPerfil: (
    id: string,
    body: { nombre: string; apellido: string; telefono?: string; areaId?: string },
  ) =>
    apiClient.put<UsuarioDetalleDto>(`/usuarios/${id}/perfil`, {
      nombre: body.nombre,
      apellido: body.apellido,
      telefono: body.telefono ?? null,
      areaId: body.areaId ?? null,
      fotoUrl: null,
      actualizarFoto: false,
    }),

  activar: (id: string) => apiClient.patch(`/usuarios/${id}/activar`),

  desactivar: (id: string) => apiClient.patch(`/usuarios/${id}/desactivar`),

  eliminar: (id: string) => apiClient.delete(`/usuarios/${id}`),

  restablecerContrasena: async (correo: string): Promise<void> => {
    const appUrl = import.meta.env.VITE_APP_URL ?? window.location.origin
    const { error } = await supabase.auth.resetPasswordForEmail(correo, {
      redirectTo: `${appUrl}/reset-password`,
    })
    if (error) throw new Error(error.message)
  },
}
