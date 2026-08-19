import { apiClient, type PagedBackendResponse } from '@services/apiClient'
import type { TipoServicioDto, CategoriaDto } from '@features/tickets/services/catalogoService'

export type { TipoServicioDto, CategoriaDto }

export const tipoServicioAdminService = {
  listar: (empresaId?: string) =>
    apiClient.get<PagedBackendResponse<TipoServicioDto>>('/tipos-servicio', {
      tamanoPagina: 100,
      ...(empresaId ? { empresaId } : {}),
    }),

  crear: (data: { empresaId: string; nombre: string; orden: number; descripcion?: string }) =>
    apiClient.post<string>('/tipos-servicio', data),

  actualizar: (id: string, data: { nombre: string; orden: number; descripcion?: string }) =>
    apiClient.put<string>(`/tipos-servicio/${id}`, data),

  activar: (id: string) => apiClient.patch<string>(`/tipos-servicio/${id}/activar`),

  desactivar: (id: string) => apiClient.patch<string>(`/tipos-servicio/${id}/desactivar`),
}

/** Catálogo de especialidades de técnicos. Las globales (empresaId null) se ven desde todas las empresas. */
export interface EspecialidadDto {
  id: string
  empresaId: string | null
  nombre: string
  descripcion: string | null
  activo: boolean
  esGlobal: boolean
}

export const especialidadAdminService = {
  listar: (empresaId?: string) =>
    apiClient.get<PagedBackendResponse<EspecialidadDto>>('/especialidades', {
      tamanoPagina: 100,
      ...(empresaId ? { empresaId } : {}),
    }),

  crear: (data: { nombre: string; descripcion?: string; empresaId?: string }) =>
    apiClient.post<string>('/especialidades', data),

  actualizar: (id: string, data: { nombre: string; descripcion?: string }) =>
    apiClient.put<string>(`/especialidades/${id}`, data),

  activar: (id: string) => apiClient.patch<string>(`/especialidades/${id}/activar`),

  desactivar: (id: string) => apiClient.patch<string>(`/especialidades/${id}/desactivar`),
}

export const categoriaAdminService = {
  listar: (empresaId?: string) =>
    apiClient.get<PagedBackendResponse<CategoriaDto>>('/categorias', {
      tamanoPagina: 100,
      ...(empresaId ? { empresaId } : {}),
    }),

  crear: (data: { nombre: string; descripcion?: string; empresaId?: string }) =>
    apiClient.post<string>('/categorias', data),

  actualizar: (id: string, data: { nombre: string; descripcion?: string }) =>
    apiClient.put<string>(`/categorias/${id}`, data),

  activar: (id: string) => apiClient.patch<string>(`/categorias/${id}/activar`),

  desactivar: (id: string) => apiClient.patch<string>(`/categorias/${id}/desactivar`),
}
