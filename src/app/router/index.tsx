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

// ResetPasswordPage se carga eager — Supabase la visita sin sesión activa vía token en hash
import { ResetPasswordPage } from '@features/auth/pages/ResetPasswordPage'

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
const EmpresasPage = lazy(() =>
  import('@features/empresas/pages/EmpresasPage').then((m) => ({ default: m.EmpresasPage })),
)
const EmpresaNewPage = lazy(() =>
  import('@features/empresas/pages/EmpresaNewPage').then((m) => ({ default: m.EmpresaNewPage })),
)
const EmpresaDetailPage = lazy(() =>
  import('@features/empresas/pages/EmpresaDetailPage').then((m) => ({
    default: m.EmpresaDetailPage,
  })),
)
const EmpresaEditPage = lazy(() =>
  import('@features/empresas/pages/EmpresaEditPage').then((m) => ({ default: m.EmpresaEditPage })),
)
const SucursalesPage = lazy(() =>
  import('@features/sucursales/pages/SucursalesPage').then((m) => ({ default: m.SucursalesPage })),
)
const SucursalNewPage = lazy(() =>
  import('@features/sucursales/pages/SucursalNewPage').then((m) => ({
    default: m.SucursalNewPage,
  })),
)
const SucursalDetailPage = lazy(() =>
  import('@features/sucursales/pages/SucursalDetailPage').then((m) => ({
    default: m.SucursalDetailPage,
  })),
)
const SucursalEditPage = lazy(() =>
  import('@features/sucursales/pages/SucursalEditPage').then((m) => ({
    default: m.SucursalEditPage,
  })),
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

function RequireAdminRole() {
  const user = useAuthStore((s) => s.user)
  if (!user || (user.rol !== 'admin' && user.rol !== 'superadmin')) {
    return <Navigate to={ROUTES.DASHBOARD} replace />
  }
  return <Outlet />
}

function RequireSuperAdminRole() {
  const user = useAuthStore((s) => s.user)
  if (!user || user.rol !== 'superadmin') {
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

  // Ruta pública sin guard — Supabase procesa el token del hash antes de que haya sesión
  {
    element: <AuthLayout />,
    children: [{ path: ROUTES.RESET_PASSWORD, element: <ResetPasswordPage /> }],
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
              // Sucursales — admin y superadmin
              {
                path: ROUTES.SUCURSALES,
                element: (
                  <LazyPage>
                    <SucursalesPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.SUCURSALES_NEW,
                element: (
                  <LazyPage>
                    <SucursalNewPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.SUCURSALES_DETAIL,
                element: (
                  <LazyPage>
                    <SucursalDetailPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.SUCURSALES_EDIT,
                element: (
                  <LazyPage>
                    <SucursalEditPage />
                  </LazyPage>
                ),
              },
            ],
          },
          // Empresas — solo superadmin
          {
            element: <RequireSuperAdminRole />,
            children: [
              {
                path: ROUTES.EMPRESAS,
                element: (
                  <LazyPage>
                    <EmpresasPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.EMPRESAS_NEW,
                element: (
                  <LazyPage>
                    <EmpresaNewPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.EMPRESAS_DETAIL,
                element: (
                  <LazyPage>
                    <EmpresaDetailPage />
                  </LazyPage>
                ),
              },
              {
                path: ROUTES.EMPRESAS_EDIT,
                element: (
                  <LazyPage>
                    <EmpresaEditPage />
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
