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

export interface SucursalAsignacionItem {
  sucursalId: string
  sucursalNombre: string
  esPrincipal: boolean
  activo: boolean
}

export interface EspecialidadAsignadaItem {
  especialidadId: string
  especialidadNombre: string
  activo: boolean
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
  sucursales: SucursalAsignacionItem[]
  especialidades: EspecialidadAsignadaItem[]
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
    // Opcional: los roles Admin y SuperAdmin no se asignan a una sucursal y omiten el campo
    sucursalId?: string
    empresaId?: string
    nombre: string
    apellido: string
    correo: string
    nombreUsuario: string
    contrasena: string
    telefono?: string
    rol: string
    sucursales?: { sucursalId: string; esPrincipal: boolean }[]
  }) => apiClient.post<UsuarioDetalleDto>('/usuarios', body),

  actualizarSucursales: (id: string, sucursales: { sucursalId: string; esPrincipal: boolean }[]) =>
    apiClient.put(`/usuarios/${id}/sucursales`, { sucursales }),

  /** Reemplaza la lista completa de especialidades del usuario. Puede ir vacía. */
  actualizarEspecialidades: (id: string, especialidades: string[]) =>
    apiClient.put(`/usuarios/${id}/especialidades`, { especialidades }),

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

  cambiarRol: (id: string, nuevoRol: string) => apiClient.put(`/usuarios/${id}/rol`, { nuevoRol }),

  listarRoles: () =>
    apiClient.get<{ items: { codigo: string; nombre: string; activo: boolean }[] }>('/roles'),

  restablecerContrasena: async (correo: string): Promise<void> => {
    const appUrl = import.meta.env.VITE_APP_URL ?? window.location.origin
    const { error } = await supabase.auth.resetPasswordForEmail(correo, {
      redirectTo: `${appUrl}/reset-password`,
    })
    if (error) throw new Error(error.message)
  },
}
