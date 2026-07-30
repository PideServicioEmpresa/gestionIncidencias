import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { AppUser, SucursalPerfil } from '@types-app/index'

interface AuthState {
  user: AppUser | null
  accessToken: string | null
  isAuthenticated: boolean
  sucursalActiva: string | null
  sucursalesDisponibles: SucursalPerfil[]

  setUser: (user: AppUser | null) => void
  setToken: (token: string | null) => void
  setAuth: (user: AppUser, token: string) => void
  clearAuth: () => void
  setSucursales: (sucursales: SucursalPerfil[], principal: string | null) => void
  cambiarSucursalActiva: (sucursalId: string) => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      accessToken: null,
      isAuthenticated: false,
      sucursalActiva: null,
      sucursalesDisponibles: [],

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
        }),
      setSucursales: (sucursales, principal) =>
        set({ sucursalesDisponibles: sucursales, sucursalActiva: principal }),
      cambiarSucursalActiva: (sucursalId) => set({ sucursalActiva: sucursalId }),
    }),
    {
      name: 'pide-servicio-auth',
      partialize: (state) => ({
        user: state.user,
        accessToken: state.accessToken,
        isAuthenticated: state.isAuthenticated,
        sucursalActiva: state.sucursalActiva,
        sucursalesDisponibles: state.sucursalesDisponibles,
      }),
    },
  ),
)
