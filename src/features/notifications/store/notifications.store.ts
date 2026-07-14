import { create } from 'zustand'

interface StoreNotification {
  id: string
  read: boolean
}

interface NotificationsState {
  notifications: StoreNotification[]
  setNotifications: (items: StoreNotification[]) => void
  markRead: (id: string) => void
  markAllRead: () => void
}

export const useNotificationsStore = create<NotificationsState>((set) => ({
  notifications: [],
  setNotifications: (items) => set({ notifications: items }),
  markRead: (id) =>
    set((state) => ({
      notifications: state.notifications.map((n) => (n.id === id ? { ...n, read: true } : n)),
    })),
  markAllRead: () =>
    set((state) => ({
      notifications: state.notifications.map((n) => ({ ...n, read: true })),
    })),
}))
