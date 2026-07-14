import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { usuarioService } from '../services/usuarioService'
import type { UsuarioListParams } from '../services/usuarioService'
import { toast } from 'sonner'

const USER_KEYS = {
  all: ['usuarios'] as const,
  list: (params?: UsuarioListParams) => ['usuarios', 'list', params] as const,
  detail: (id: string) => ['usuarios', 'detail', id] as const,
}

export function useUsuarios(params?: UsuarioListParams) {
  return useQuery({
    queryKey: USER_KEYS.list(params),
    queryFn: () => usuarioService.listar(params),
    staleTime: 1000 * 60 * 2,
  })
}

export function useUsuario(id: string) {
  return useQuery({
    queryKey: USER_KEYS.detail(id),
    queryFn: () => usuarioService.obtener(id),
    enabled: !!id,
    staleTime: 1000 * 60 * 2,
  })
}

export function useCrearUsuario() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: Parameters<typeof usuarioService.crear>[0]) => usuarioService.crear(body),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: USER_KEYS.all })
    },
  })
}

export function useActualizarPerfil() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({
      id,
      data,
    }: {
      id: string
      data: { nombre: string; apellido: string; telefono?: string; areaId?: string }
    }) => usuarioService.actualizarPerfil(id, data),
    onSuccess: (_data, { id }) => {
      void qc.invalidateQueries({ queryKey: USER_KEYS.detail(id) })
      void qc.invalidateQueries({ queryKey: USER_KEYS.all })
    },
  })
}

export function useToggleEstadoUsuario() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, activar }: { id: string; activar: boolean }) =>
      activar ? usuarioService.activar(id) : usuarioService.desactivar(id),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: USER_KEYS.all })
    },
    onError: () => {
      toast.error('No se pudo cambiar el estado del usuario.')
    },
  })
}

export function useEliminarUsuario() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => usuarioService.eliminar(id),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: USER_KEYS.all })
    },
    onError: () => {
      toast.error('No se pudo eliminar el usuario.')
    },
  })
}

export function useRestablecerContrasena() {
  return useMutation({
    mutationFn: (correo: string) => usuarioService.restablecerContrasena(correo),
    onSuccess: () => {
      toast.success('Se envió el enlace de restablecimiento al correo indicado.')
    },
    onError: () => {
      toast.error('No se pudo enviar el enlace. Verifica el correo e intenta de nuevo.')
    },
  })
}
