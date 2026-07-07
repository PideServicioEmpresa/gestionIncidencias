import { createBrowserRouter, Navigate, Outlet, RouterProvider } from 'react-router-dom'
import { AuthLayout } from '@app/layouts/AuthLayout'
import { AppLayout } from '@app/layouts/AppLayout'
import { useAuthStore } from '@store/auth.store'
import { ROUTES } from '@constants/index'

// Lazy page imports
import { LoginPage } from '@features/auth/pages/LoginPage'
import { DashboardPage } from '@features/dashboard/pages/DashboardPage'
import { MyTicketsPage } from '@features/tickets/pages/MyTicketsPage'
import { CreateTicketPage } from '@features/tickets/pages/CreateTicketPage'
import { TicketDetailPage } from '@features/tickets/pages/TicketDetailPage'
import { UsersPage } from '@features/users/pages/UsersPage'
import { UserNewPage } from '@features/users/pages/UserNewPage'
import { UserDetailPage } from '@features/users/pages/UserDetailPage'
import { UserEditPage } from '@features/users/pages/UserEditPage'
import { NotificationsPage } from '@features/notifications/pages/NotificationsPage'
import { ProfilePage } from '@features/profile/pages/ProfilePage'
import { DesignSystemPage } from '@features/design-system/pages/DesignSystemPage'
import { ReportsPage } from '@features/reports/pages/ReportsPage'
import { AuditPage } from '@features/audit/pages/AuditPage'
import { SettingsPage } from '@features/settings/pages/SettingsPage'

// ── Guards ────────────────────────────────────────────────────────────────────

function RequireAuth() {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated)
  if (!isAuthenticated) {
    return <Navigate to={ROUTES.LOGIN} replace />
  }
  return <Outlet />
}

function RequireGuest() {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated)
  if (isAuthenticated) {
    return <Navigate to={ROUTES.DASHBOARD} replace />
  }
  return <Outlet />
}

// ── Router ────────────────────────────────────────────────────────────────────

const router = createBrowserRouter([
  // Redirect home to dashboard
  {
    path: ROUTES.HOME,
    element: <Navigate to={ROUTES.DASHBOARD} replace />,
  },

  // Auth routes (unauthenticated only)
  {
    element: <RequireGuest />,
    children: [
      {
        element: <AuthLayout />,
        children: [{ path: ROUTES.LOGIN, element: <LoginPage /> }],
      },
    ],
  },

  // App routes (authenticated)
  {
    element: <RequireAuth />,
    children: [
      {
        element: <AppLayout />,
        children: [
          { path: ROUTES.DASHBOARD, element: <DashboardPage /> },
          { path: ROUTES.TICKETS, element: <MyTicketsPage /> },
          { path: ROUTES.TICKETS_NEW, element: <CreateTicketPage /> },
          { path: '/tickets/:id', element: <TicketDetailPage /> },
          { path: ROUTES.USERS, element: <UsersPage /> },
          { path: ROUTES.USERS_NEW, element: <UserNewPage /> },
          { path: ROUTES.USERS_DETAIL, element: <UserDetailPage /> },
          { path: ROUTES.USERS_EDIT, element: <UserEditPage /> },
          { path: ROUTES.NOTIFICATIONS, element: <NotificationsPage /> },
          { path: ROUTES.PROFILE, element: <ProfilePage /> },
          { path: ROUTES.REPORTS, element: <ReportsPage /> },
          { path: ROUTES.AUDIT, element: <AuditPage /> },
          { path: ROUTES.SETTINGS, element: <SettingsPage /> },
          // Design system showcase
          { path: ROUTES.DESIGN_SYSTEM, element: <DesignSystemPage /> },
        ],
      },
    ],
  },

  // Catch-all
  {
    path: '*',
    element: <Navigate to={ROUTES.DASHBOARD} replace />,
  },
])

export function AppRouter() {
  return <RouterProvider router={router} />
}
