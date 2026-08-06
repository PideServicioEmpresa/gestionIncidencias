import { apiClient } from '@services/apiClient'

export interface CorreoGuardadoDto {
  id: string
  correo: string
}

export const correosGuardadosService = {
  async listar(): Promise<CorreoGuardadoDto[]> {
    return apiClient.get<CorreoGuardadoDto[]>('/correos-guardados')
  },

  async agregar(correo: string): Promise<CorreoGuardadoDto> {
    return apiClient.post<CorreoGuardadoDto>('/correos-guardados', { correo })
  },

  async eliminar(id: string): Promise<void> {
    await apiClient.delete(`/correos-guardados/${id}`)
  },
}
