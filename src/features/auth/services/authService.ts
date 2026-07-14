import { supabase } from '@services/supabase'
import { apiClient } from '@services/apiClient'
import { useAuthStore } from '@store/auth.store'
import type { AppUser, PerfilBackend, UserRole, LaborStatus } from '@types-app/index'
import { ROL_MAP } from '@types-app/index'

// ── Helpers ───────────────────────────────────────────────────────────────────

function mapPerfilToAppUser(perfil: PerfilBackend, authUserId: string): AppUser {
  const partes = perfil.nombreCompleto.trim().split(' ')
  const nombre = partes[0] ?? ''
  const apellido = partes.slice(1).join(' ')
  const rol: UserRole = (ROL_MAP[perfil.rol.toUpperCase()] ?? 'usuario') as UserRole

  return {
    id: perfil.id,
    authUserId,
    nombre,
    apellido,
    correo: perfil.correo,
    usuario: perfil.correo.split('@')[0] ?? perfil.correo,
    rolId: perfil.rol,
    rol,
    empresaId: perfil.empresaId,
    sucursalId: perfil.sucursalId,
    sucursalIds: [perfil.sucursalId],
    estadoLaboral: 'activo' as LaborStatus,
    activo: perfil.activo,
    permisos: perfil.permisos,
  }
}

// ── Auth service ──────────────────────────────────────────────────────────────

export const authService = {
  /**
   * Autentica con Supabase Auth y carga el perfil del usuario desde el backend.
   * Guarda el token y el usuario en el store de Zustand.
   */
  async login(correo: string, contrasena: string): Promise<AppUser> {
    const { data, error } = await supabase.auth.signInWithPassword({
      email: correo,
      password: contrasena,
    })

    if (error) {
      const msg =
        error.message === 'Invalid login credentials'
          ? 'Correo o contraseña incorrectos.'
          : error.message === 'Email not confirmed'
            ? 'Debes confirmar tu correo antes de iniciar sesión.'
            : error.message
      throw new Error(msg)
    }

    if (!data.session) throw new Error('No se pudo iniciar sesión. Inténtalo de nuevo.')

    // Guardar token antes de llamar al backend (apiClient lo necesita)
    useAuthStore.getState().setToken(data.session.access_token)

    // Cargar perfil del usuario desde el backend .NET
    const perfil = await apiClient.get<PerfilBackend>('/auth/me')

    const user = mapPerfilToAppUser(perfil, data.user.id)
    useAuthStore.getState().setAuth(user, data.session.access_token)

    return user
  },

  /**
   * Cierra la sesión tanto en Supabase como en el store local.
   */
  async logout(): Promise<void> {
    await supabase.auth.signOut()
    useAuthStore.getState().clearAuth()
  },

  /**
   * Restaura la sesión activa desde Supabase (al recargar la app).
   * Devuelve el usuario si hay sesión válida, null si no.
   */
  async restaurarSesion(): Promise<AppUser | null> {
    const { data } = await supabase.auth.getSession()
    if (!data.session) {
      useAuthStore.getState().clearAuth()
      return null
    }

    useAuthStore.getState().setToken(data.session.access_token)

    try {
      const perfil = await apiClient.get<PerfilBackend>('/auth/me')
      const user = mapPerfilToAppUser(perfil, data.session.user.id)
      useAuthStore.getState().setAuth(user, data.session.access_token)
      return user
    } catch {
      // Si el backend no responde o el usuario no existe, limpiar sesión
      useAuthStore.getState().clearAuth()
      return null
    }
  },

  /**
   * Suscribirse a cambios de sesión (auto-refresh de tokens).
   * Retorna la función de unsubscribe.
   */
  onAuthStateChange(callback: (user: AppUser | null) => void): () => void {
    const {
      data: { subscription },
    } = supabase.auth.onAuthStateChange(async (event, session) => {
      if (event === 'SIGNED_OUT' || !session) {
        useAuthStore.getState().clearAuth()
        callback(null)
        return
      }

      if (event === 'TOKEN_REFRESHED' && session.access_token) {
        useAuthStore.getState().setToken(session.access_token)
      }

      if (event === 'SIGNED_IN' || event === 'TOKEN_REFRESHED') {
        const currentUser = useAuthStore.getState().user
        if (currentUser) {
          callback(currentUser)
        }
      }
    })

    return () => subscription.unsubscribe()
  },
}
