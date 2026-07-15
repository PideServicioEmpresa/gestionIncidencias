import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import {
  sucursalService,
  type SucursalListParams,
  type CrearSucursalDto,
  type ActualizarSucursalDto,
} from '../services/sucursalService'

const SUCURSAL_KEYS = {
  all: ['sucursales'] as const,
  list: (params?: SucursalListParams) => ['sucursales', 'list', params] as const,
  detail: (id: string) => ['sucursales', 'detail', id] as const,
}

export function useSucursales(params?: SucursalListParams) {
  return useQuery({
    queryKey: SUCURSAL_KEYS.list(params),
    queryFn: () => sucursalService.listar(params),
    staleTime: 1000 * 60 * 2,
  })
}

export function useSucursal(id: string) {
  return useQuery({
    queryKey: SUCURSAL_KEYS.detail(id),
    queryFn: () => sucursalService.obtener(id),
    enabled: !!id,
    staleTime: 1000 * 60 * 2,
  })
}

export function useCrearSucursal() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (dto: CrearSucursalDto) => sucursalService.crear(dto),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: SUCURSAL_KEYS.all })
      // Invalida también el cache de catálogos usado en CreateTicketPage
      void qc.invalidateQueries({ queryKey: ['catalogos', 'sucursales'] })
      toast.success('Sucursal creada correctamente.')
    },
    onError: (err: Error) => {
      toast.error(err.message)
    },
  })
}

export function useActualizarSucursal(id: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (dto: ActualizarSucursalDto) => sucursalService.actualizar(id, dto),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: SUCURSAL_KEYS.all })
      void qc.invalidateQueries({ queryKey: SUCURSAL_KEYS.detail(id) })
      toast.success('Sucursal actualizada correctamente.')
    },
    onError: (err: Error) => {
      toast.error(err.message)
    },
  })
}

export function useToggleSucursal() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, activa }: { id: string; activa: boolean }) =>
      activa ? sucursalService.desactivar(id) : sucursalService.activar(id),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: SUCURSAL_KEYS.all })
      toast.success('Estado de sucursal actualizado.')
    },
    onError: (err: Error) => {
      toast.error(err.message)
    },
  })
}
