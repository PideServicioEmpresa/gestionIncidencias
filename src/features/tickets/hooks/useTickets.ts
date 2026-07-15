import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ticketService } from '../services/ticketService'
import type { TicketListParams } from '../services/ticketService'

const TICKET_KEYS = {
  all: ['tickets'] as const,
  list: (params?: TicketListParams) => ['tickets', 'list', params] as const,
  detail: (id: string) => ['tickets', 'detail', id] as const,
  comentarios: (id: string) => ['tickets', id, 'comentarios'] as const,
  evidencias: (id: string) => ['tickets', id, 'evidencias'] as const,
  historial: (id: string) => ['tickets', id, 'historial'] as const,
}

export function useTickets(params?: TicketListParams) {
  return useQuery({
    queryKey: TICKET_KEYS.list(params),
    queryFn: () => ticketService.listar(params),
    staleTime: 1000 * 30,
  })
}

export function useTicket(id: string) {
  return useQuery({
    queryKey: TICKET_KEYS.detail(id),
    queryFn: () => ticketService.obtener(id),
    enabled: !!id,
    staleTime: 1000 * 30,
  })
}

export function useComentarios(ticketId: string) {
  return useQuery({
    queryKey: TICKET_KEYS.comentarios(ticketId),
    queryFn: async () => {
      const resp = await ticketService.listarComentarios(ticketId)
      return resp.items ?? []
    },
    enabled: !!ticketId,
    staleTime: 1000 * 15,
  })
}

export function useEvidencias(ticketId: string) {
  return useQuery({
    queryKey: TICKET_KEYS.evidencias(ticketId),
    queryFn: async () => {
      const resp = await ticketService.listarEvidencias(ticketId)
      return resp.items ?? []
    },
    enabled: !!ticketId,
    staleTime: 1000 * 15,
  })
}

export function useTicketHistorial(ticketId: string) {
  return useQuery({
    queryKey: TICKET_KEYS.historial(ticketId),
    queryFn: async () => {
      const resp = await ticketService.listarHistorial(ticketId)
      return resp.items ?? []
    },
    enabled: !!ticketId,
    staleTime: 1000 * 15,
  })
}

export function useCrearTicket() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: Parameters<typeof ticketService.crear>[0]) => ticketService.crear(body),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useAsignarTicket() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ ticketId, tecnicoId }: { ticketId: string; tecnicoId: string }) =>
      ticketService.asignar(ticketId, tecnicoId),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useCambiarPrioridad() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ ticketId, nuevaPrioridad }: { ticketId: string; nuevaPrioridad: string }) =>
      ticketService.cambiarPrioridad(ticketId, nuevaPrioridad),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useCambiarArea() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ ticketId, nuevaAreaId }: { ticketId: string; nuevaAreaId: string }) =>
      ticketService.cambiarArea(ticketId, nuevaAreaId),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useIniciarProceso() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (ticketId: string) => ticketService.iniciarProceso(ticketId),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function usePausar() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (ticketId: string) => ticketService.pausar(ticketId),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useReanudar() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (ticketId: string) => ticketService.reanudar(ticketId),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useSubmitValidacion() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (ticketId: string) => ticketService.submitValidacion(ticketId),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useCerrarTicket() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ ticketId }: { ticketId: string }) => ticketService.cerrar(ticketId),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useReabrirTicket() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({
      ticketId,
      motivoRechazoId,
      comentarioRechazo,
    }: {
      ticketId: string
      motivoRechazoId: string
      comentarioRechazo?: string
    }) => ticketService.reabrir(ticketId, motivoRechazoId, comentarioRechazo),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useCancelarTicket() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({
      ticketId,
      motivoCancelacionId,
      comentario,
    }: {
      ticketId: string
      motivoCancelacionId: string
      comentario?: string
    }) => ticketService.cancelar(ticketId, motivoCancelacionId, comentario),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.all })
    },
  })
}

export function useCrearComentario() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({
      ticketId,
      cuerpo,
      esInterno,
    }: {
      ticketId: string
      cuerpo: string
      esInterno: boolean
    }) => ticketService.crearComentario(ticketId, { cuerpo, esInterno }),
    onSuccess: (_data, { ticketId }) => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.comentarios(ticketId) })
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.historial(ticketId) })
    },
  })
}

export function useSubirEvidencia() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({
      ticketId,
      archivo,
      tipo,
    }: {
      ticketId: string
      archivo: File
      tipo: 'INICIAL' | 'FINAL'
    }) => ticketService.subirEvidencia(ticketId, archivo, tipo),
    onSuccess: (_data, { ticketId }) => {
      void qc.invalidateQueries({ queryKey: TICKET_KEYS.evidencias(ticketId) })
    },
  })
}
