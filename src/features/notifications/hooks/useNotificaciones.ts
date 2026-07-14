import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { notificacionService } from '../services/notificacionService'
import type { NotificacionListParams } from '../services/notificacionService'

const NOTIF_KEYS = {
  all: ['notificaciones'] as const,
  list: (params?: NotificacionListParams) => ['notificaciones', 'list', params] as const,
}

export function useNotificaciones(params?: NotificacionListParams) {
  return useQuery({
    queryKey: NOTIF_KEYS.list(params),
    queryFn: () => notificacionService.listar(params),
    staleTime: 1000 * 30,
  })
}

export function useMarcarLeida() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => notificacionService.marcarLeida(id),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: NOTIF_KEYS.all })
    },
  })
}

export function useMarcarTodasLeidas() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: () => notificacionService.marcarTodasLeidas(),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: NOTIF_KEYS.all })
    },
  })
}

export function useConteoNotificaciones() {
  return useQuery({
    queryKey: ['notificaciones', 'conteo'],
    queryFn: async () => {
      const resp = await notificacionService.listar({ soloNoLeidas: true, tamanoPagina: 1 })
      return { sinLeer: resp.totalRegistros ?? 0 }
    },
    staleTime: 1000 * 30,
    refetchInterval: 1000 * 60,
  })
}
