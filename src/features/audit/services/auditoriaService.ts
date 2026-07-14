import { apiClient, type PagedBackendResponse } from '@services/apiClient'

export interface AuditLogDto {
  id: string
  empresaId: string | null
  tabla: string
  registroId: string
  accion: string
  usuarioId: string | null
  valoresAnteriores: string | null
  valoresNuevos: string | null
  ipAddress: string | null
  createdAt: string
}

export interface AuditoriaListParams {
  tabla?: string
  registroId?: string
  usuarioId?: string
  desde?: string
  hasta?: string
  pagina?: number
  tamanoPagina?: number
}

export const auditoriaService = {
  listar: (params?: AuditoriaListParams) =>
    apiClient.get<PagedBackendResponse<AuditLogDto>>(
      '/auditoria',
      params as Record<string, string | number | boolean | null | undefined>,
    ),
}
