import { useQuery } from '@tanstack/react-query'
import { apiClient } from '@services/apiClient'

export interface ResumenEstadoDto {
  estado: string
  total: number
}

export interface ResumenPrioridadDto {
  prioridad: string
  total: number
}

export interface ResumenSucursalDto {
  sucursalId: string
  sucursalNombre: string
  total: number
}

export interface ResumenAreaDto {
  areaId: string
  areaNombre: string
  sucursalId: string
  total: number
}

export interface ResumenTecnicoDto {
  tecnicoId: string
  tecnicoNombre: string
  total: number
}

export interface ResumenTipoServicioDto {
  tipoServicioId: string
  tipoServicioNombre: string
  total: number
}

export interface TendenciaDiariaDto {
  fecha: string
  total: number
}

export interface TendenciaSemanalDto {
  semana: string
  total: number
}

export interface DashboardResumenDto {
  totalAbiertos: number
  criticos: number
  cerradosHoy: number
  tasaResolucionPct: number
  porEstado: ResumenEstadoDto[]
  porPrioridad: ResumenPrioridadDto[]
  porSucursal: ResumenSucursalDto[]
  porArea: ResumenAreaDto[]
  porTecnico: ResumenTecnicoDto[]
  porTipoServicio: ResumenTipoServicioDto[]
  tendencia16Dias: TendenciaDiariaDto[]
  tendenciaSemanal: TendenciaSemanalDto[]
}

export function useDashboardResumen(params?: { sucursalId?: string }) {
  return useQuery({
    queryKey: ['dashboard', 'resumen', params],
    queryFn: () =>
      apiClient.get<DashboardResumenDto>(
        '/dashboard/resumen',
        params as Record<string, string | undefined>,
      ),
    staleTime: 1000 * 30,
    retry: 1,
  })
}
