import { useState, useEffect } from 'react'
import { Outlet, useLocation } from 'react-router-dom'
import { AppSidebar } from '@app/components/AppSidebar'
import { AppHeader } from '@app/components/AppHeader'
import { MobileNav } from '@app/components/MobileNav'
import { ScrollToTop } from '@app/components/ScrollToTop'
import { PageBreadcrumb } from '@app/components/PageBreadcrumb'
import { CommandMenu } from '@app/components/CommandMenu'
import { Sheet, SheetContent } from '@shared/ui/sheet'
import { ROUTES } from '@constants/index'

const ROUTE_TITLES: Record<string, string> = {
  [ROUTES.DASHBOARD]: 'Dashboard',
  [ROUTES.TICKETS]: 'Tickets',
  [ROUTES.TICKETS_NEW]: 'Nuevo ticket',
  [ROUTES.USERS]: 'Usuarios',
  [ROUTES.USERS_NEW]: 'Nuevo Usuario',
  [ROUTES.NOTIFICATIONS]: 'Notificaciones',
  [ROUTES.REPORTS]: 'Reportes',
  [ROUTES.AUDIT]: 'Auditoría',
  [ROUTES.SETTINGS]: 'Configuración',
  [ROUTES.PROFILE]: 'Mi perfil',
}

function getTitle(pathname: string): string {
  if (ROUTE_TITLES[pathname]) return ROUTE_TITLES[pathname]
  if (pathname.startsWith('/tickets/') && pathname !== '/tickets/nuevo') return 'Detalle de ticket'
  if (pathname.startsWith('/usuarios/')) {
    if (pathname === '/usuarios/nuevo') return 'Nuevo Usuario'
    if (pathname.endsWith('/editar')) return 'Editar Usuario'
    return 'Detalle de Usuario'
  }
  return 'Pidde Servicio'
}

export function AppLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [commandOpen, setCommandOpen] = useState(false)
  const location = useLocation()
  const title = getTitle(location.pathname)

  useEffect(() => {
    function handleKeyDown(e: KeyboardEvent) {
      if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
        e.preventDefault()
        setCommandOpen((prev) => !prev)
      }
    }

    document.addEventListener('keydown', handleKeyDown)
    return () => document.removeEventListener('keydown', handleKeyDown)
  }, [])

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

      <ScrollToTop />

      {/* Main content */}
      <div className="flex flex-1 flex-col overflow-hidden">
        <AppHeader
          onMenuClick={() => setSidebarOpen(true)}
          title={title}
          onCommandOpen={() => setCommandOpen(true)}
        />
        <main className="flex-1 overflow-y-auto pb-20 pt-4 lg:pb-0 lg:pt-5">
          <PageBreadcrumb />
          <Outlet />
        </main>
      </div>

      {/* Mobile bottom nav */}
      <MobileNav />

      {/* Command Menu global */}
      <CommandMenu open={commandOpen} onOpenChange={setCommandOpen} />
    </div>
  )
}
