import { apiClient } from '@services/apiClient'
import type { PagedBackendResponse } from '@services/apiClient'

export interface NotificacionDto {
  id: string
  titulo: string
  cuerpo: string
  tipo: string
  ticketId: string | null
  esLeida: boolean
  createdAt: string
}

export interface NotificacionListParams {
  pagina?: number
  tamanoPagina?: number
  soloNoLeidas?: boolean
}

export const notificacionService = {
  listar: (params?: NotificacionListParams) =>
    apiClient.get<PagedBackendResponse<NotificacionDto>>(
      '/notificaciones',
      params as Record<string, string | number | boolean | null | undefined>,
    ),

  marcarLeida: (id: string) => apiClient.patch(`/notificaciones/${id}/leer`),

  marcarTodasLeidas: () => apiClient.patch('/notificaciones/leer-todas'),
}
