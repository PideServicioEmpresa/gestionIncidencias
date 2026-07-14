import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { AppUser } from '@types-app/index'

interface AuthState {
  user: AppUser | null
  accessToken: string | null
  isAuthenticated: boolean
  setUser: (user: AppUser | null) => void
  setToken: (token: string | null) => void
  setAuth: (user: AppUser, token: string) => void
  clearAuth: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      accessToken: null,
      isAuthenticated: false,

      setUser: (user) => set({ user, isAuthenticated: user !== null }),
      setToken: (token) => set({ accessToken: token }),
      setAuth: (user, token) => set({ user, accessToken: token, isAuthenticated: true }),
      clearAuth: () => set({ user: null, accessToken: null, isAuthenticated: false }),
    }),
    {
      name: 'pide-servicio-auth',
      partialize: (state) => ({
        user: state.user,
        accessToken: state.accessToken,
        isAuthenticated: state.isAuthenticated,
      }),
    },
  ),
)
