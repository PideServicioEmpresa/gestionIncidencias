import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Search, Plus, MoreHorizontal, Building2, Power, Pencil, Eye } from 'lucide-react'
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
import { useEmpresas, useToggleEmpresa } from '../hooks/useEmpresas'
import type { EmpresaResumenDto } from '../services/empresaService'
import { ROUTES, empresaDetailPath, empresaEditPath } from '@constants/index'

export function EmpresasPage() {
  const navigate = useNavigate()
  const [busqueda, setBusqueda] = useState('')
  const [toggleTarget, setToggleTarget] = useState<EmpresaResumenDto | null>(null)

  const { data, isLoading } = useEmpresas({ busqueda: busqueda || undefined })
  const toggleEmpresa = useToggleEmpresa()

  const empresas = data?.items ?? []

  function handleToggle() {
    if (!toggleTarget) return
    toggleEmpresa.mutate(
      { id: toggleTarget.id, activa: toggleTarget.activa },
      { onSuccess: () => setToggleTarget(null) },
    )
  }

  return (
    <div className="px-3 py-3 lg:px-5">
      {/* Header */}
      <div className="mb-4 flex items-center justify-between gap-3">
        <div>
          <h2 className="text-base font-semibold tracking-tight">Empresas</h2>
          <p className="text-xs text-muted-foreground">
            {isLoading
              ? 'Cargando...'
              : `${data?.totalRegistros ?? 0} empresa${(data?.totalRegistros ?? 0) !== 1 ? 's' : ''} registrada${(data?.totalRegistros ?? 0) !== 1 ? 's' : ''}`}
          </p>
        </div>
        <Button size="sm" className="gap-1.5" onClick={() => navigate(ROUTES.EMPRESAS_NEW)}>
          <Plus className="h-3.5 w-3.5" />
          Nueva empresa
        </Button>
      </div>

      {/* Buscador */}
      <div className="relative mb-4">
        <Search className="absolute left-2.5 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
        <Input
          className="h-9 pl-8 text-sm"
          placeholder="Buscar empresa..."
          value={busqueda}
          onChange={(e) => setBusqueda(e.target.value)}
        />
      </div>

      {/* Lista */}
      {isLoading ? (
        <div className="space-y-3">
          {Array.from({ length: 4 }).map((_, i) => (
            <Card key={i}>
              <CardContent className="p-4">
                <div className="flex items-center gap-3">
                  <Skeleton className="h-10 w-10 rounded-lg" />
                  <div className="flex-1 space-y-1.5">
                    <Skeleton className="h-4 w-48" />
                    <Skeleton className="h-3 w-32" />
                  </div>
                  <Skeleton className="h-6 w-16 rounded-full" />
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      ) : empresas.length === 0 ? (
        <Card>
          <CardContent className="flex flex-col items-center gap-3 py-12 text-center">
            <Building2 className="h-10 w-10 text-muted-foreground/40" />
            <div>
              <p className="text-sm font-medium">No se encontraron empresas</p>
              <p className="text-xs text-muted-foreground">
                {busqueda
                  ? 'Intenta con otro término de búsqueda.'
                  : 'Crea la primera empresa para comenzar.'}
              </p>
            </div>
            {!busqueda && (
              <Button size="sm" onClick={() => navigate(ROUTES.EMPRESAS_NEW)}>
                Nueva empresa
              </Button>
            )}
          </CardContent>
        </Card>
      ) : (
        <div className="space-y-2">
          {empresas.map((empresa) => (
            <EmpresaRow
              key={empresa.id}
              empresa={empresa}
              onView={() => navigate(empresaDetailPath(empresa.id))}
              onEdit={() => navigate(empresaEditPath(empresa.id))}
              onToggle={() => setToggleTarget(empresa)}
            />
          ))}
        </div>
      )}

      {/* Confirm toggle */}
      <ConfirmDialog
        open={!!toggleTarget}
        onOpenChange={(open) => {
          if (!open) setToggleTarget(null)
        }}
        title={toggleTarget?.activa ? 'Desactivar empresa' : 'Activar empresa'}
        description={
          toggleTarget?.activa
            ? `¿Desactivar "${toggleTarget.nombreComercial}"? Los usuarios de esta empresa no podrán acceder al sistema.`
            : `¿Activar "${toggleTarget?.nombreComercial}"? Los usuarios de esta empresa podrán volver a acceder.`
        }
        confirmLabel={toggleTarget?.activa ? 'Desactivar' : 'Activar'}
        variant={toggleTarget?.activa ? 'destructive' : 'default'}
        loading={toggleEmpresa.isPending}
        onConfirm={handleToggle}
      />
    </div>
  )
}

// ── EmpresaRow ────────────────────────────────────────────────────────────────

interface EmpresaRowProps {
  empresa: EmpresaResumenDto
  onView: () => void
  onEdit: () => void
  onToggle: () => void
}

function EmpresaRow({ empresa, onView, onEdit, onToggle }: EmpresaRowProps) {
  return (
    <Card className="transition-colors hover:bg-accent/30">
      <CardContent className="p-3">
        <div className="flex items-center gap-3">
          {/* Icono / Logo */}
          <button
            onClick={onView}
            className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
            aria-label={`Ver detalle de ${empresa.nombreComercial}`}
          >
            <Building2 className="h-5 w-5" />
          </button>

          {/* Info principal */}
          <button onClick={onView} className="min-w-0 flex-1 text-left focus-visible:outline-none">
            <p className="truncate text-sm font-semibold leading-tight">
              {empresa.nombreComercial}
            </p>
            <p className="mt-0.5 truncate text-xs text-muted-foreground">
              Registrada el {new Date(empresa.createdAt).toLocaleDateString('es-PE')}
            </p>
          </button>

          {/* Badge estado */}
          <Badge
            className={
              empresa.activa
                ? 'border-transparent bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400'
                : 'border-transparent bg-gray-100 text-gray-500 dark:bg-gray-800 dark:text-gray-400'
            }
          >
            {empresa.activa ? 'Activa' : 'Inactiva'}
          </Badge>

          {/* Menú */}
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
                className={empresa.activa ? 'text-destructive focus:text-destructive' : ''}
              >
                <Power className="mr-2 h-3.5 w-3.5" />
                {empresa.activa ? 'Desactivar' : 'Activar'}
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </CardContent>
    </Card>
  )
}
