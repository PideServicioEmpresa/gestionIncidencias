import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import { MainLayout } from '@app/layouts/MainLayout'
import { DesignSystemPage } from '@features/design-system/pages/DesignSystemPage'
import { ROUTES } from '@constants/index'

const router = createBrowserRouter([
  {
    path: ROUTES.HOME,
    element: <MainLayout />,
    children: [
      {
        index: true,
        element: (
          <div className="flex min-h-screen items-center justify-center">
            <p className="text-sm text-muted-foreground">
              Pide Servicio — ir a{' '}
              <a
                href="/ui"
                className="text-primary underline underline-offset-4 hover:text-primary/80"
              >
                /ui
              </a>{' '}
              para ver el Design System
            </p>
          </div>
        ),
      },
      {
        path: 'ui',
        element: <DesignSystemPage />,
      },
    ],
  },
])

export function AppRouter() {
  return <RouterProvider router={router} />
}
