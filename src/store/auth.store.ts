import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { AppUser, SucursalPerfil } from '@types-app/index'

interface AuthState {
  user: AppUser | null
  accessToken: string | null
  isAuthenticated: boolean
  sucursalActiva: string | null
  sucursalesDisponibles: SucursalPerfil[]
  // true solo después de que el usuario confirme en la pantalla de selección.
  // false → RequireSucursalActiva redirige al selector.
  // No se modifica en restaurarSesion (solo en login y logout).
  sucursalConfirmada: boolean

  setUser: (user: AppUser | null) => void
  setToken: (token: string | null) => void
  setAuth: (user: AppUser, token: string) => void
  clearAuth: () => void
  setSucursales: (sucursales: SucursalPerfil[], principal: string | null) => void
  cambiarSucursalActiva: (sucursalId: string) => void
  confirmarSucursal: () => void
  resetConfirmacionSucursal: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      accessToken: null,
      isAuthenticated: false,
      sucursalActiva: null,
      sucursalesDisponibles: [],
      sucursalConfirmada: false,

      setUser: (user) => set({ user, isAuthenticated: user !== null }),
      setToken: (token) => set({ accessToken: token }),
      setAuth: (user, token) => set({ user, accessToken: token, isAuthenticated: true }),
      clearAuth: () =>
        set({
          user: null,
          accessToken: null,
          isAuthenticated: false,
          sucursalActiva: null,
          sucursalesDisponibles: [],
          sucursalConfirmada: false,
        }),
      setSucursales: (sucursales, principal) =>
        set({ sucursalesDisponibles: sucursales, sucursalActiva: principal }),
      cambiarSucursalActiva: (sucursalId) => set({ sucursalActiva: sucursalId }),
      confirmarSucursal: () => set({ sucursalConfirmada: true }),
      resetConfirmacionSucursal: () => set({ sucursalConfirmada: false }),
    }),
    {
      name: 'pide-servicio-auth',
      partialize: (state) => ({
        user: state.user,
        accessToken: state.accessToken,
        isAuthenticated: state.isAuthenticated,
        sucursalActiva: state.sucursalActiva,
        sucursalesDisponibles: state.sucursalesDisponibles,
        sucursalConfirmada: state.sucursalConfirmada,
      }),
    },
  ),
)
