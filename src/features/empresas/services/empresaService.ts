import { apiClient, type PagedBackendResponse } from '@services/apiClient'

export interface EmpresaResumenDto {
  id: string
  nombreComercial: string
  activa: boolean
  createdAt: string
}

export interface EmpresaDetalleDto {
  id: string
  nombreComercial: string
  razonSocial: string
  identificacionFiscal: string
  logoUrl: string | null
  colorPrimario: string | null
  colorSecundario: string | null
  zonaHoraria: string
  activa: boolean
  createdAt: string
}

export interface EmpresaListParams {
  busqueda?: string
  soloActivas?: boolean
  pagina?: number
  tamanoPagina?: number
}

export interface CrearEmpresaDto {
  nombreComercial: string
  razonSocial: string
  identificacionFiscal: string
  zonaHoraria: string
  logoUrl?: string
  colorPrimario?: string
  colorSecundario?: string
}

export interface ActualizarEmpresaDto {
  nombreComercial: string
  razonSocial: string
  zonaHoraria: string
  logoUrl?: string
  colorPrimario?: string
  colorSecundario?: string
}

type Params = Record<string, string | number | boolean | null | undefined>

export const empresaService = {
  listar: (params?: EmpresaListParams) =>
    apiClient.get<PagedBackendResponse<EmpresaResumenDto>>('/empresas', params as Params),

  obtener: (id: string) => apiClient.get<EmpresaDetalleDto>(`/empresas/${id}`),

  crear: (dto: CrearEmpresaDto) => apiClient.post<EmpresaDetalleDto>('/empresas', dto),

  actualizar: (id: string, dto: ActualizarEmpresaDto) =>
    apiClient.put<EmpresaDetalleDto>(`/empresas/${id}`, dto),

  activar: (id: string) => apiClient.patch<void>(`/empresas/${id}/activar`),

  desactivar: (id: string) => apiClient.patch<void>(`/empresas/${id}/desactivar`),
}
