import { apiClient } from '@services/apiClient'
import type { PagedBackendResponse } from '@services/apiClient'

export interface TipoServicioDto {
  id: string
  empresaId: string
  nombre: string
  descripcion: string | null
  orden: number
  activo: boolean
}

export interface CategoriaDto {
  id: string
  empresaId: string | null
  nombre: string
  descripcion: string | null
  activa: boolean
  esGlobal: boolean
}

export interface SucursalResumenDto {
  id: string
  empresaId: string
  nombre: string
  activa: boolean
}

export interface AreaResumenDto {
  id: string
  empresaId: string
  nombre: string
  activa: boolean
}

export interface TecnicoResumenDto {
  id: string
  nombreCompleto: string
  correo: string
  rol: string
  activo: boolean
}

export interface MotivoRechazoResumenDto {
  id: string
  empresaId: string | null
  codigo: string
  nombre: string
  esOtro: boolean
  orden: number
  activo: boolean
  esGlobal: boolean
}

export interface MotivoCancelacionResumenDto {
  id: string
  empresaId: string | null
  codigo: string
  nombre: string
  esOtro: boolean
  orden: number
  activo: boolean
  esGlobal: boolean
}

export const catalogoService = {
  listarTiposServicio: (empresaId?: string) =>
    apiClient.get<PagedBackendResponse<TipoServicioDto>>('/tipos-servicio', {
      tamanoPagina: 100,
      ...(empresaId ? { empresaId } : {}),
    }),

  listarCategorias: (empresaId?: string) =>
    apiClient.get<PagedBackendResponse<CategoriaDto>>('/categorias', {
      tamanoPagina: 100,
      ...(empresaId ? { empresaId } : {}),
    }),

  listarSucursales: (empresaId?: string) =>
    apiClient.get<PagedBackendResponse<SucursalResumenDto>>('/sucursales', {
      tamanoPagina: 100,
      ...(empresaId ? { empresaId } : {}),
    }),

  listarAreas: (empresaId?: string) =>
    apiClient.get<PagedBackendResponse<AreaResumenDto>>('/areas', {
      tamanoPagina: 100,
      ...(empresaId ? { empresaId } : {}),
    }),

  listarTecnicos: (empresaId?: string) =>
    apiClient.get<PagedBackendResponse<TecnicoResumenDto>>('/usuarios', {
      rol: 'TECNICO',
      soloActivos: true,
      tamanoPagina: 100,
      ...(empresaId ? { empresaId } : {}),
    }),

  listarMotivosRechazo: () =>
    apiClient.get<PagedBackendResponse<MotivoRechazoResumenDto>>('/motivos-rechazo', {
      soloActivos: true,
      tamanoPagina: 100,
    }),

  listarMotivosCancelacion: () =>
    apiClient.get<PagedBackendResponse<MotivoCancelacionResumenDto>>('/motivos-cancelacion', {
      soloActivos: true,
      tamanoPagina: 100,
    }),
}
