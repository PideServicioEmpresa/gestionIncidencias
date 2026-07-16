import { apiClient } from '@services/apiClient'
import type { PagedBackendResponse } from '@services/apiClient'

// ── DTOs ──────────────────────────────────────────────────────────────────────

export interface TicketListItemDto {
  id: string
  codigo: string
  titulo: string
  estado: string
  prioridadEfectiva: string
  sucursalId: string
  sucursalNombre: string
  areaId: string
  areaNombre: string
  tipo: string
  tipoServicioId: string
  tipoServicioNombre: string | null
  categoriaId: string | null
  tecnicoId: string | null
  asignadoANombre: string | null
  solicitanteId: string
  solicitanteNombre: string
  fechaCreacion: string
  fechaLimite: string | null
}

export interface TicketDetalleDto extends TicketListItemDto {
  descripcion: string
  ubicacion: string | null
  tecnicoNombre: string | null
  empresaId: string
}

export interface ComentarioDto {
  id: string
  autorId: string
  autorNombre: string | null
  autorRol: string | null
  cuerpo: string
  esInterno: boolean
  createdAt: string
}

export interface EvidenciaDto {
  id: string
  tipo: 'INICIAL' | 'FINAL'
  tipoMime: string
  urlAlmacenamiento: string
  nombreOriginal: string
  tamanoBytes: number
}

export interface TicketHistorialDto {
  id: string
  tipoEvento: string
  estadoAnterior: string | null
  estadoNuevo: string | null
  actorId: string | null
  actorNombre: string | null
  comentarioTexto: string | null
  createdAt: string
}

export interface TicketListParams {
  pagina?: number
  tamanoPagina?: number
  busqueda?: string
  estado?: string
  prioridad?: string
  fechaDesde?: string
  fechaHasta?: string
  solicitanteId?: string
  tecnicoId?: string
  sucursalId?: string
}

// ── Service ───────────────────────────────────────────────────────────────────

export const ticketService = {
  listar: (params?: TicketListParams) =>
    apiClient.get<PagedBackendResponse<TicketListItemDto>>(
      '/tickets',
      params as Record<string, string | number | boolean | null | undefined>,
    ),

  obtener: (id: string) => apiClient.get<TicketDetalleDto>(`/tickets/${id}`),

  crear: (body: {
    titulo: string
    descripcion: string
    sucursalId: string
    areaNombre: string
    tipoServicioId: string
    categoriaId?: string
    prioridad: string
    ubicacion?: string
  }) => apiClient.post<string>('/tickets', body),

  asignar: (ticketId: string, tecnicoId: string) =>
    apiClient.post(`/tickets/${ticketId}/asignar`, { tecnicoId }),

  reasignar: (ticketId: string, nuevoTecnicoId: string, motivo?: string) =>
    apiClient.post(`/tickets/${ticketId}/reasignar`, { nuevoTecnicoId, motivo }),

  actualizar: (ticketId: string, nuevoTitulo?: string, nuevoTipoServicioId?: string) =>
    apiClient.patch(`/tickets/${ticketId}/actualizar`, { nuevoTitulo, nuevoTipoServicioId }),

  cambiarPrioridad: (ticketId: string, nuevaPrioridad: string) =>
    apiClient.patch(`/tickets/${ticketId}/prioridad`, { nuevaPrioridad }),

  cambiarArea: (ticketId: string, nuevaAreaId: string) =>
    apiClient.patch(`/tickets/${ticketId}/area`, { nuevaAreaId }),

  iniciarProceso: (ticketId: string) => apiClient.patch(`/tickets/${ticketId}/iniciar`),

  pausar: (ticketId: string) => apiClient.patch(`/tickets/${ticketId}/pausar`),

  reanudar: (ticketId: string) => apiClient.patch(`/tickets/${ticketId}/reanudar`),

  submitValidacion: (ticketId: string) => apiClient.patch(`/tickets/${ticketId}/submit-validacion`),

  cerrar: (ticketId: string) => apiClient.patch(`/tickets/${ticketId}/cerrar`),

  reabrir: (ticketId: string, motivoRechazoId: string, comentarioRechazo?: string) =>
    apiClient.patch(`/tickets/${ticketId}/reabrir`, { motivoRechazoId, comentarioRechazo }),

  cancelar: (ticketId: string, motivoCancelacionId: string, comentario?: string) =>
    apiClient.patch(`/tickets/${ticketId}/cancelar`, {
      motivoCancelacionId,
      ...(comentario ? { comentario } : {}),
    }),

  listarComentarios: (ticketId: string) =>
    apiClient.get<PagedBackendResponse<ComentarioDto>>(`/tickets/${ticketId}/comentarios`, {
      tamanoPagina: 200,
    }),

  crearComentario: (ticketId: string, body: { cuerpo: string; esInterno: boolean }) =>
    apiClient.post<ComentarioDto>(`/tickets/${ticketId}/comentarios`, body),

  listarEvidencias: (ticketId: string) =>
    apiClient.get<PagedBackendResponse<EvidenciaDto>>(`/tickets/${ticketId}/evidencias`, {
      tamanoPagina: 50,
    }),

  subirEvidencia: (ticketId: string, archivo: File, tipo: 'INICIAL' | 'FINAL') => {
    const form = new FormData()
    form.append('archivo', archivo)
    form.append('tipo', tipo)
    return apiClient.upload<EvidenciaDto>(`/tickets/${ticketId}/evidencias`, form)
  },

  listarHistorial: (ticketId: string) =>
    apiClient.get<PagedBackendResponse<TicketHistorialDto>>(`/tickets/${ticketId}/historial`, {
      tamanoPagina: 200,
    }),
}
