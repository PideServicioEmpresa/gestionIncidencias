import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Search, Plus, MoreHorizontal, MapPin, Power, Pencil, Eye, Building2 } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Badge } from '@shared/ui/badge'
import { Card, CardContent } from '@shared/ui/card'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@shared/ui/dropdown-menu'
import { Skeleton } from '@shared/ui/skeleton'
import { ConfirmDialog } from '@shared/components/ConfirmDialog'
import { useSucursales, useToggleSucursal } from '../hooks/useSucursales'
import type { SucursalResumenDto } from '../services/sucursalService'
import { ROUTES, sucursalDetailPath, sucursalEditPath } from '@constants/index'
import { useEmpresas } from '@features/empresas/hooks/useEmpresas'

export function SucursalesPage() {
  const navigate = useNavigate()
  const [busqueda, setBusqueda] = useState('')
  const [toggleTarget, setToggleTarget] = useState<SucursalResumenDto | null>(null)

  const { data, isLoading } = useSucursales({ busqueda: busqueda || undefined })
  const toggleSucursal = useToggleSucursal()
  const empresasQuery = useEmpresas({ tamanoPagina: 100 })
  const empresaMap: Record<string, string> = {}
  for (const e of empresasQuery.data?.items ?? []) {
    empresaMap[e.id] = e.nombreComercial
  }

  const sucursales = data?.items ?? []

  function handleToggle() {
    if (!toggleTarget) return
    toggleSucursal.mutate(
      { id: toggleTarget.id, activa: toggleTarget.activa },
      { onSuccess: () => setToggleTarget(null) },
    )
  }

  return (
    <div className="px-3 py-3 lg:px-5">
      {/* Header */}
      <div className="mb-4 flex items-center justify-between gap-3">
        <div>
          <h2 className="text-base font-semibold tracking-tight">Sucursales</h2>
          <p className="text-xs text-muted-foreground">
            {isLoading
              ? 'Cargando...'
              : `${data?.totalRegistros ?? 0} sucursal${(data?.totalRegistros ?? 0) !== 1 ? 'es' : ''} registrada${(data?.totalRegistros ?? 0) !== 1 ? 's' : ''}`}
          </p>
        </div>
        <Button size="sm" className="gap-1.5" onClick={() => navigate(ROUTES.SUCURSALES_NEW)}>
          <Plus className="h-3.5 w-3.5" />
          Nueva sucursal
        </Button>
      </div>

      {/* Buscador */}
      <div className="relative mb-4">
        <Search className="absolute left-2.5 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
        <Input
          className="h-9 pl-8 text-sm"
          placeholder="Buscar sucursal..."
          value={busqueda}
          onChange={(e) => setBusqueda(e.target.value)}
        />
      </div>

      {/* Lista */}
      {isLoading ? (
        <div className="space-y-3">
          {Array.from({ length: 5 }).map((_, i) => (
            <Card key={i}>
              <CardContent className="p-3">
                <div className="flex items-center gap-3">
                  <Skeleton className="h-9 w-9 rounded-lg" />
                  <div className="flex-1 space-y-1.5">
                    <Skeleton className="h-4 w-40" />
                    <Skeleton className="h-3 w-28" />
                  </div>
                  <Skeleton className="h-6 w-14 rounded-full" />
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      ) : sucursales.length === 0 ? (
        <Card>
          <CardContent className="flex flex-col items-center gap-3 py-12 text-center">
            <MapPin className="h-10 w-10 text-muted-foreground/40" />
            <div>
              <p className="text-sm font-medium">No se encontraron sucursales</p>
              <p className="text-xs text-muted-foreground">
                {busqueda
                  ? 'Intenta con otro término de búsqueda.'
                  : 'Crea la primera sucursal para comenzar.'}
              </p>
            </div>
            {!busqueda && (
              <Button size="sm" onClick={() => navigate(ROUTES.SUCURSALES_NEW)}>
                Nueva sucursal
              </Button>
            )}
          </CardContent>
        </Card>
      ) : (
        <div className="space-y-2">
          {sucursales.map((sucursal) => (
            <SucursalRow
              key={sucursal.id}
              sucursal={sucursal}
              empresaNombre={empresaMap[sucursal.empresaId]}
              onView={() => navigate(sucursalDetailPath(sucursal.id))}
              onEdit={() => navigate(sucursalEditPath(sucursal.id))}
              onToggle={() => setToggleTarget(sucursal)}
            />
          ))}
        </div>
      )}

      <ConfirmDialog
        open={!!toggleTarget}
        onOpenChange={(open) => {
          if (!open) setToggleTarget(null)
        }}
        title={toggleTarget?.activa ? 'Desactivar sucursal' : 'Activar sucursal'}
        description={
          toggleTarget?.activa
            ? `¿Desactivar "${toggleTarget.nombre}"? Los usuarios de esta sucursal no podrán acceder.`
            : `¿Activar "${toggleTarget?.nombre}"? Los usuarios de esta sucursal podrán volver a acceder.`
        }
        confirmLabel={toggleTarget?.activa ? 'Desactivar' : 'Activar'}
        variant={toggleTarget?.activa ? 'destructive' : 'default'}
        loading={toggleSucursal.isPending}
        onConfirm={handleToggle}
      />
    </div>
  )
}

interface SucursalRowProps {
  sucursal: SucursalResumenDto
  empresaNombre?: string
  onView: () => void
  onEdit: () => void
  onToggle: () => void
}

function SucursalRow({ sucursal, empresaNombre, onView, onEdit, onToggle }: SucursalRowProps) {
  return (
    <Card className="transition-colors hover:bg-accent/30">
      <CardContent className="p-3">
        <div className="flex items-center gap-3">
          <button
            onClick={onView}
            className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-blue-500/10 text-blue-600 focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring dark:text-blue-400"
            aria-label={`Ver ${sucursal.nombre}`}
          >
            <MapPin className="h-4 w-4" />
          </button>

          <button onClick={onView} className="min-w-0 flex-1 text-left focus-visible:outline-none">
            <p className="truncate text-sm font-semibold leading-tight">{sucursal.nombre}</p>
            {empresaNombre && (
              <span className="mt-0.5 inline-flex items-center gap-1 rounded-full bg-violet-50 px-2 py-0.5 text-[10px] font-medium text-violet-700 dark:bg-violet-900/30 dark:text-violet-400">
                <Building2 className="h-2.5 w-2.5" />
                {empresaNombre}
              </span>
            )}
            <p className="mt-0.5 text-xs text-muted-foreground">
              Registrada el {new Date(sucursal.createdAt).toLocaleDateString('es-PE')}
            </p>
          </button>

          <Badge
            className={
              sucursal.activa
                ? 'border-transparent bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400'
                : 'border-transparent bg-gray-100 text-gray-500 dark:bg-gray-800 dark:text-gray-400'
            }
          >
            {sucursal.activa ? 'Activa' : 'Inactiva'}
          </Badge>

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" size="icon" className="h-7 w-7 shrink-0">
                <MoreHorizontal className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-44">
              <DropdownMenuItem onClick={onView}>
                <Eye className="mr-2 h-3.5 w-3.5" />
                Ver detalle
              </DropdownMenuItem>
              <DropdownMenuItem onClick={onEdit}>
                <Pencil className="mr-2 h-3.5 w-3.5" />
                Editar
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onClick={onToggle}
                className={sucursal.activa ? 'text-destructive focus:text-destructive' : ''}
              >
                <Power className="mr-2 h-3.5 w-3.5" />
                {sucursal.activa ? 'Desactivar' : 'Activar'}
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </CardContent>
    </Card>
  )
}
