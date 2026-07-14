import { apiClient } from '@services/apiClient'

export interface ParametroDto {
  id: string
  empresaId: string | null
  clave: string
  valor: string
  tipoDato: string
  descripcion: string | null
  updatedAt: string
}

export const configuracionService = {
  listar: (empresaId?: string) =>
    apiClient.get<ParametroDto[]>('/configuracion', empresaId ? { empresaId } : undefined),

  obtenerPorClave: (clave: string) => apiClient.get<ParametroDto>(`/configuracion/${clave}`),

  actualizar: (clave: string, nuevoValor: string, empresaId?: string) =>
    apiClient.put(`/configuracion/${clave}`, { nuevoValor, empresaId }),
}
