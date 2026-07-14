import { useQuery } from '@tanstack/react-query'
import { catalogoService } from '../services/catalogoService'

export const CATALOGO_KEYS = {
  tiposServicio: ['catalogos', 'tipos-servicio'] as const,
  categorias: ['catalogos', 'categorias'] as const,
  sucursales: (empresaId?: string) => ['catalogos', 'sucursales', empresaId] as const,
  areas: (empresaId?: string) => ['catalogos', 'areas', empresaId] as const,
  tecnicos: (empresaId?: string) => ['catalogos', 'tecnicos', empresaId] as const,
  motivosRechazo: ['catalogos', 'motivos-rechazo'] as const,
  motivosCancelacion: ['catalogos', 'motivos-cancelacion'] as const,
}

export function useTiposServicio(empresaId?: string) {
  return useQuery({
    queryKey: [...CATALOGO_KEYS.tiposServicio, empresaId] as const,
    queryFn: async () => {
      const resp = await catalogoService.listarTiposServicio(empresaId)
      return resp.items ?? []
    },
    staleTime: 1000 * 60 * 10,
    enabled: !!empresaId,
  })
}

export function useCategorias(empresaId?: string) {
  return useQuery({
    queryKey: [...CATALOGO_KEYS.categorias, empresaId] as const,
    queryFn: async () => {
      const resp = await catalogoService.listarCategorias(empresaId)
      return resp.items ?? []
    },
    staleTime: 1000 * 60 * 10,
  })
}

export function useSucursales(empresaId?: string) {
  return useQuery({
    queryKey: CATALOGO_KEYS.sucursales(empresaId),
    queryFn: async () => {
      const resp = await catalogoService.listarSucursales(empresaId)
      return resp.items ?? []
    },
    staleTime: 1000 * 60 * 10,
  })
}

export function useAreas(empresaId?: string) {
  return useQuery({
    queryKey: CATALOGO_KEYS.areas(empresaId),
    queryFn: async () => {
      const resp = await catalogoService.listarAreas(empresaId)
      return resp.items ?? []
    },
    staleTime: 1000 * 60 * 10,
  })
}

export function useTecnicos(empresaId?: string) {
  return useQuery({
    queryKey: CATALOGO_KEYS.tecnicos(empresaId),
    queryFn: async () => {
      const resp = await catalogoService.listarTecnicos(empresaId)
      return resp.items ?? []
    },
    staleTime: 1000 * 60 * 5,
  })
}

export function useMotivosRechazo() {
  return useQuery({
    queryKey: CATALOGO_KEYS.motivosRechazo,
    queryFn: async () => {
      const resp = await catalogoService.listarMotivosRechazo()
      return resp.items ?? []
    },
    staleTime: 1000 * 60 * 10,
  })
}

export function useMotivosCancelacion() {
  return useQuery({
    queryKey: CATALOGO_KEYS.motivosCancelacion,
    queryFn: async () => {
      const resp = await catalogoService.listarMotivosCancelacion()
      return resp.items ?? []
    },
    staleTime: 1000 * 60 * 10,
  })
}
