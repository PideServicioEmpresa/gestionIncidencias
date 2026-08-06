import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { correosGuardadosService } from '../services/correosGuardadosService'

const QUERY_KEY = ['correos-guardados'] as const

export function useCorreosGuardados() {
  return useQuery({
    queryKey: QUERY_KEY,
    queryFn: () => correosGuardadosService.listar(),
    staleTime: 1000 * 60 * 5,
  })
}

export function useAgregarCorreoGuardado() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (correo: string) => correosGuardadosService.agregar(correo),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: QUERY_KEY })
    },
  })
}

export function useEliminarCorreoGuardado() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => correosGuardadosService.eliminar(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: QUERY_KEY })
    },
  })
}
