import { create } from 'zustand'
import { persist } from 'zustand/middleware'

export type AccentColor = 'blue' | 'violet' | 'green' | 'orange' | 'red' | 'yellow'

export interface NotificationPrefs {
  email: boolean
  push: boolean
  assignments: boolean
  statusChanges: boolean
  comments: boolean
}

interface PreferencesState {
  accentColor: AccentColor
  notifications: NotificationPrefs
  setAccentColor: (color: AccentColor) => void
  setNotificationPref: (key: keyof NotificationPrefs, value: boolean) => void
}

export const usePreferencesStore = create<PreferencesState>()(
  persist(
    (set) => ({
      accentColor: 'blue',
      notifications: {
        email: true,
        push: true,
        assignments: true,
        statusChanges: true,
        comments: false,
      },
      setAccentColor: (accentColor) => set({ accentColor }),
      setNotificationPref: (key, value) =>
        set((s) => ({ notifications: { ...s.notifications, [key]: value } })),
    }),
    { name: 'pide-preferences' },
  ),
)
