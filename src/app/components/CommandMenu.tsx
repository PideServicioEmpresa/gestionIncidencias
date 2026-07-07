import { useNavigate } from 'react-router-dom'
import {
  LayoutDashboard,
  Ticket,
  Users,
  Bell,
  BarChart2,
  Shield,
  Settings,
  UserCircle,
  PlusCircle,
  UserPlus,
} from 'lucide-react'
import {
  CommandDialog,
  CommandInput,
  CommandList,
  CommandGroup,
  CommandItem,
  CommandSeparator,
  CommandEmpty,
} from '@shared/ui/command'
import { ROUTES } from '@constants/index'

interface CommandMenuProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

interface CommandNavItem {
  label: string
  href: string
  icon: React.ReactNode
}

const ACCIONES_RAPIDAS: CommandNavItem[] = [
  {
    label: 'Crear Ticket',
    href: ROUTES.TICKETS_NEW,
    icon: <PlusCircle className="h-4 w-4" />,
  },
  {
    label: 'Crear Usuario',
    href: ROUTES.USERS,
    icon: <UserPlus className="h-4 w-4" />,
  },
]

const NAVEGACION: CommandNavItem[] = [
  {
    label: 'Dashboard',
    href: ROUTES.DASHBOARD,
    icon: <LayoutDashboard className="h-4 w-4" />,
  },
  {
    label: 'Mis Tickets',
    href: ROUTES.TICKETS,
    icon: <Ticket className="h-4 w-4" />,
  },
  {
    label: 'Usuarios',
    href: ROUTES.USERS,
    icon: <Users className="h-4 w-4" />,
  },
  {
    label: 'Notificaciones',
    href: ROUTES.NOTIFICATIONS,
    icon: <Bell className="h-4 w-4" />,
  },
  {
    label: 'Reportes',
    href: ROUTES.REPORTS,
    icon: <BarChart2 className="h-4 w-4" />,
  },
  {
    label: 'Auditoría',
    href: ROUTES.AUDIT,
    icon: <Shield className="h-4 w-4" />,
  },
]

const CONFIGURACION: CommandNavItem[] = [
  {
    label: 'Configuración',
    href: ROUTES.SETTINGS,
    icon: <Settings className="h-4 w-4" />,
  },
  {
    label: 'Mi Perfil',
    href: ROUTES.PROFILE,
    icon: <UserCircle className="h-4 w-4" />,
  },
]

export function CommandMenu({ open, onOpenChange }: CommandMenuProps) {
  const navigate = useNavigate()

  function handleSelect(href: string) {
    navigate(href)
    onOpenChange(false)
  }

  return (
    <CommandDialog open={open} onOpenChange={onOpenChange}>
      <CommandInput placeholder="Buscar acciones, páginas..." />
      <CommandList>
        <CommandEmpty>Sin resultados.</CommandEmpty>

        <CommandGroup heading="Acciones rápidas">
          {ACCIONES_RAPIDAS.map((item) => (
            <CommandItem
              key={`accion-${item.href}-${item.label}`}
              onSelect={() => handleSelect(item.href)}
            >
              {item.icon}
              <span>{item.label}</span>
            </CommandItem>
          ))}
        </CommandGroup>

        <CommandSeparator />

        <CommandGroup heading="Navegación">
          {NAVEGACION.map((item) => (
            <CommandItem
              key={`nav-${item.href}-${item.label}`}
              onSelect={() => handleSelect(item.href)}
            >
              {item.icon}
              <span>{item.label}</span>
            </CommandItem>
          ))}
        </CommandGroup>

        <CommandSeparator />

        <CommandGroup heading="Configuración">
          {CONFIGURACION.map((item) => (
            <CommandItem
              key={`cfg-${item.href}-${item.label}`}
              onSelect={() => handleSelect(item.href)}
            >
              {item.icon}
              <span>{item.label}</span>
            </CommandItem>
          ))}
        </CommandGroup>
      </CommandList>
    </CommandDialog>
  )
}
