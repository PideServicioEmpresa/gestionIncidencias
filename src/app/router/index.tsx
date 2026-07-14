import { lazy, Suspense } from 'react'
import { createBrowserRouter, Navigate, Outlet, RouterProvider } from 'react-router-dom'
import { AuthLayout } from '@app/layouts/AuthLayout'
import { AppLayout } from '@app/layouts/AppLayout'
import { useAuthStore } from '@store/auth.store'
import { ROUTES } from '@constants/index'
import { ErrorBoundary } from '@shared/components/ErrorBoundary'
import { PageSkeleton } from '@shared/components/PageSkeletons'

// LoginPage se carga de manera eager (es la primera pantalla para usuarios no autenticados)
import { LoginPage } from '@features/auth/pages/LoginPage'

// Resto de páginas con lazy loading para reducir el bundle inicial
const DashboardPage = lazy(() =>
  import('@features/dashboard/pages/DashboardPage').then((m) => ({ default: m.DashboardPage })),
)
const MyTicketsPage = lazy(() =>
  import('@features/tickets/pages/MyTicketsPage').then((m) => ({ default: m.MyTicketsPage })),
)
const CreateTicketPage = lazy(() =>
  import('@features/tickets/pages/CreateTicketPage').then((m) => ({ default: m.CreateTicketPage })),
)
const TicketDetailPage = lazy(() =>
  import('@features/tickets/pages/TicketDetailPage').then((m) => ({ default: m.TicketDetailPage })),
)
const UsersPage = lazy(() =>
  import('@features/users/pages/UsersPage').then((m) => ({ default: m.UsersPage })),
)
const UserNewPage = lazy(() =>
  import('@features/users/pages/UserNewPage').then((m) => ({ default: m.UserNewPage })),
)
const UserDetailPage = lazy(() =>
  import('@features/users/pages/UserDetailPage').then((m) => ({ default: m.UserDetailPage })),
)
const UserEditPage = lazy(() =>
  import('@features/users/pages/UserEditPage').then((m) => ({ default: m.UserEditPage })),
)
const NotificationsPage = lazy(() =>
  import('@features/notifications/pages/NotificationsPage').then((m) => ({
    default: m.NotificationsPage,
  })),
)
const ProfilePage = lazy(() =>
  import('@features/profile/pages/ProfilePage').then((m) => ({ default: m.ProfilePage })),
)
const DesignSystemPage = lazy(() =>
  import('@features/design-system/pages/DesignSystemPage').then((m) => ({
    default: m.DesignSystemPage,
  })),
)
const ReportsPage = lazy(() =>
  import('@features/reports/pages/ReportsPage').then((m) => ({ default: m.ReportsPage })),
)
const AuditPage = lazy(() =>
  import('@features/audit/pages/AuditPage').then((m) => ({ default: m.AuditPage })),
)
const SettingsPage = lazy(() =>
  import('@features/settings/pages/SettingsPage').then((m) => ({ default: m.SettingsPage })),
)

// ── Wrapper de suspense para páginas lazy ─────────────────────────────────────

function LazyPage({ children }: { children: React.ReactNode }) {
  return (
    <ErrorBoundary>
      <Suspense fallback={<PageSkeleton />}>{children}</Suspense>
    </ErrorBoundary>
  )
}

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

// Guard de rol: solo admin y superadmin. Redirige al dashboard si el rol no es suficiente.
// Debe usarse anidado dentro de RequireAuth para garantizar que el usuario está autenticado.
function RequireAdminRole() {
  const user = useAuthStore((s) => s.user)
  if (!user || (user.rol !== 'admin' && user.rol !== 'superadmin')) {
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
          {
            path: ROUTES.DASHBOARD,
            element: (
              <LazyPage>
                <DashboardPage />
              </LazyPage>
            ),
          },
          {
            path: ROUTES.TICKETS,
            element: (
              <LazyPage>
                <MyTicketsPage />
              </LazyPage>
            ),
          },
          {
            path: ROUTES.TICKETS_NEW,
            element: (
              <LazyPage>
                <CreateTicketPage />
              </LazyPage>
            ),
          },
          {
            path: '/tickets/:id',
            element: (
              <LazyPage>
                <TicketDetailPage />
              </LazyPage>
            ),
          },
          {
            path: ROUTES.NOTIFICATIONS,
            element: (
              <LazyPage>
                <NotificationsPage />
              </LazyPage>
            ),
          },
          {
            path: ROUTES.PROFILE,
            element: (
              <LazyPage>
                <ProfilePage />
              </LazyPage>
            ),
          },
          {
            path: ROUTES.REPORTS,
            element: (
              <LazyPage>
                <ReportsPage />
              </LazyPage>
            ),
          },
          // Design system showcase
          {
            path: ROUTES.DESIGN_SYSTEM,
            element: (
              <LazyPage>
                <DesignSystemPage />
              </LazyPage>
            ),
          },
          // Rutas exclusivas para admin y superadmin
          {
            element: <RequireAdminRole />,
            children: [
              {
                path: ROUTES.USERS,
                element: (
                  <LazyPage>
                    <UsersPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.USERS_NEW,
                element: (
                  <LazyPage>
                    <UserNewPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.USERS_DETAIL,
                element: (
                  <LazyPage>
                    <UserDetailPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.USERS_EDIT,
                element: (
                  <LazyPage>
                    <UserEditPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.AUDIT,
                element: (
                  <LazyPage>
                    <AuditPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.SETTINGS,
                element: (
                  <LazyPage>
                    <SettingsPage />
                  </LazyPage>
                ),
              },
            ],
          },
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
