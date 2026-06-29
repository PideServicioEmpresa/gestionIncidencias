import { useState } from 'react'
import { Outlet, useLocation } from 'react-router-dom'
import { AppSidebar } from '@app/components/AppSidebar'
import { AppHeader } from '@app/components/AppHeader'
import { MobileNav } from '@app/components/MobileNav'
import { Sheet, SheetContent } from '@shared/ui/sheet'
import { ROUTES } from '@constants/index'

const ROUTE_TITLES: Record<string, string> = {
  [ROUTES.DASHBOARD]: 'Dashboard',
  [ROUTES.TICKETS]: 'Tickets',
  [ROUTES.TICKETS_NEW]: 'Nuevo ticket',
  [ROUTES.USERS]: 'Usuarios',
  [ROUTES.NOTIFICATIONS]: 'Notificaciones',
  [ROUTES.REPORTS]: 'Reportes',
  [ROUTES.AUDIT]: 'Auditoría',
  [ROUTES.SETTINGS]: 'Configuración',
  [ROUTES.PROFILE]: 'Mi perfil',
}

function getTitle(pathname: string): string {
  if (ROUTE_TITLES[pathname]) return ROUTE_TITLES[pathname]
  if (pathname.startsWith('/tickets/') && pathname !== '/tickets/nuevo') return 'Detalle de ticket'
  return 'Pide Servicio'
}

export function AppLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const location = useLocation()
  const title = getTitle(location.pathname)

  return (
    <div className="flex h-screen overflow-hidden bg-background">
      {/* Desktop sidebar */}
      <div className="hidden lg:flex">
        <AppSidebar />
      </div>

      {/* Mobile sidebar as Sheet */}
      <Sheet open={sidebarOpen} onOpenChange={setSidebarOpen}>
        <SheetContent side="left" className="w-sidebar p-0">
          <AppSidebar />
        </SheetContent>
      </Sheet>

      {/* Main content */}
      <div className="flex flex-1 flex-col overflow-hidden">
        <AppHeader onMenuClick={() => setSidebarOpen(true)} title={title} />
        <main className="flex-1 overflow-y-auto pb-16 lg:pb-0">
          <Outlet />
        </main>
      </div>

      {/* Mobile bottom nav */}
      <MobileNav />
    </div>
  )
}
