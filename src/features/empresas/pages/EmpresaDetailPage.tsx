import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Pencil, Building2, Globe, Power } from 'lucide-react'
import { useState } from 'react'
import { Button } from '@shared/ui/button'
import { Badge } from '@shared/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Skeleton } from '@shared/ui/skeleton'
import { ConfirmDialog } from '@shared/components/ConfirmDialog'
import { useEmpresa, useToggleEmpresa } from '../hooks/useEmpresas'
import { ROUTES, empresaEditPath } from '@constants/index'

export function EmpresaDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [confirmToggle, setConfirmToggle] = useState(false)

  const { data: empresa, isLoading, isError } = useEmpresa(id ?? '')
  const toggleEmpresa = useToggleEmpresa()

  if (isLoading) {
    return (
      <div className="px-3 py-3 lg:px-5">
        <div className="mb-4 flex items-center gap-2">
          <Skeleton className="h-8 w-8 rounded" />
          <Skeleton className="h-5 w-48" />
        </div>
        <div className="grid gap-4 lg:grid-cols-[1fr_280px]">
          <Card>
            <CardContent className="space-y-3 p-4">
              <Skeleton className="h-4 w-64" />
              <Skeleton className="h-4 w-48" />
              <Skeleton className="h-4 w-56" />
            </CardContent>
          </Card>
        </div>
      </div>
    )
  }

  if (isError || !empresa) {
    return (
      <div className="px-3 py-3 lg:px-5">
        <div className="mb-4 flex items-center gap-2">
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8"
            onClick={() => navigate(ROUTES.EMPRESAS)}
          >
            <ArrowLeft className="h-4 w-4" />
          </Button>
        </div>
        <Card>
          <CardContent className="flex flex-col items-center gap-3 py-12 text-center">
            <Building2 className="h-10 w-10 text-muted-foreground/40" />
            <p className="text-sm font-medium">No se pudo cargar la empresa</p>
            <Button size="sm" onClick={() => navigate(ROUTES.EMPRESAS)}>
              Volver a empresas
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="px-3 py-3 lg:px-5">
      {/* Header */}
      <div className="mb-4 flex items-center justify-between gap-3">
        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8 shrink-0"
            onClick={() => navigate(ROUTES.EMPRESAS)}
            aria-label="Volver a empresas"
          >
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <div>
            <h2 className="text-base font-semibold tracking-tight">{empresa.nombreComercial}</h2>
            <p className="text-xs text-muted-foreground">RUC: {empresa.identificacionFiscal}</p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            className="gap-1.5"
            onClick={() => navigate(empresaEditPath(empresa.id))}
          >
            <Pencil className="h-3.5 w-3.5" />
            Editar
          </Button>
        </div>
      </div>

      <div className="grid gap-4 lg:grid-cols-[1fr_280px]">
        {/* Columna principal */}
        <div className="space-y-4">
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Información general</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 p-3 pt-0">
              <InfoRow
                icon={<Building2 className="h-3.5 w-3.5" />}
                label="Nombre comercial"
                value={empresa.nombreComercial}
              />
              <InfoRow
                icon={<Building2 className="h-3.5 w-3.5" />}
                label="Razón social"
                value={empresa.razonSocial}
              />
              <InfoRow
                icon={<Building2 className="h-3.5 w-3.5" />}
                label="RUC / ID Fiscal"
                value={empresa.identificacionFiscal}
              />
              <InfoRow
                icon={<Globe className="h-3.5 w-3.5" />}
                label="Zona horaria"
                value={empresa.zonaHoraria}
              />
            </CardContent>
          </Card>

          {(empresa.logoUrl || empresa.colorPrimario || empresa.colorSecundario) && (
            <Card>
              <CardHeader className="px-3 pb-2 pt-3">
                <CardTitle className="text-sm font-semibold">Personalización</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3 p-3 pt-0">
                {empresa.logoUrl && (
                  <InfoRow
                    icon={<Building2 className="h-3.5 w-3.5" />}
                    label="Logo URL"
                    value={empresa.logoUrl}
                  />
                )}
                {empresa.colorPrimario && (
                  <div className="flex items-start gap-2.5 text-xs">
                    <span className="mt-0.5 shrink-0 text-muted-foreground">
                      <Building2 className="h-3.5 w-3.5" />
                    </span>
                    <span className="w-28 shrink-0 text-muted-foreground">Color primario</span>
                    <div className="flex items-center gap-1.5">
                      <div
                        className="h-4 w-4 rounded border"
                        style={{ backgroundColor: empresa.colorPrimario }}
                      />
                      <span className="font-medium">{empresa.colorPrimario}</span>
                    </div>
                  </div>
                )}
                {empresa.colorSecundario && (
                  <div className="flex items-start gap-2.5 text-xs">
                    <span className="mt-0.5 shrink-0 text-muted-foreground">
                      <Building2 className="h-3.5 w-3.5" />
                    </span>
                    <span className="w-28 shrink-0 text-muted-foreground">Color secundario</span>
                    <div className="flex items-center gap-1.5">
                      <div
                        className="h-4 w-4 rounded border"
                        style={{ backgroundColor: empresa.colorSecundario }}
                      />
                      <span className="font-medium">{empresa.colorSecundario}</span>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          )}
        </div>

        {/* Panel lateral */}
        <div className="space-y-4">
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Estado</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 p-3 pt-0">
              <div className="flex items-center justify-between text-xs">
                <span className="text-muted-foreground">Estado actual</span>
                <Badge
                  className={
                    empresa.activa
                      ? 'border-transparent bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400'
                      : 'border-transparent bg-gray-100 text-gray-500 dark:bg-gray-800 dark:text-gray-400'
                  }
                >
                  {empresa.activa ? 'Activa' : 'Inactiva'}
                </Badge>
              </div>
              <div className="flex items-center justify-between text-xs">
                <span className="text-muted-foreground">Registrada el</span>
                <span className="font-medium">
                  {new Date(empresa.createdAt).toLocaleDateString('es-PE')}
                </span>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Acciones</CardTitle>
            </CardHeader>
            <CardContent className="p-3 pt-0">
              <Button
                variant="outline"
                size="sm"
                className={`w-full gap-1.5 ${empresa.activa ? 'border-destructive/50 text-destructive hover:bg-destructive/10' : ''}`}
                onClick={() => setConfirmToggle(true)}
              >
                <Power className="h-3.5 w-3.5" />
                {empresa.activa ? 'Desactivar empresa' : 'Activar empresa'}
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>

      <ConfirmDialog
        open={confirmToggle}
        onOpenChange={setConfirmToggle}
        title={empresa.activa ? 'Desactivar empresa' : 'Activar empresa'}
        description={
          empresa.activa
            ? `¿Desactivar "${empresa.nombreComercial}"? Los usuarios de esta empresa no podrán acceder al sistema.`
            : `¿Activar "${empresa.nombreComercial}"? Los usuarios de esta empresa podrán volver a acceder.`
        }
        confirmLabel={empresa.activa ? 'Desactivar' : 'Activar'}
        variant={empresa.activa ? 'destructive' : 'default'}
        loading={toggleEmpresa.isPending}
        onConfirm={() => {
          toggleEmpresa.mutate(
            { id: empresa.id, activa: empresa.activa },
            { onSuccess: () => setConfirmToggle(false) },
          )
        }}
      />
    </div>
  )
}

function InfoRow({ icon, label, value }: { icon: React.ReactNode; label: string; value: string }) {
  return (
    <div className="flex items-start gap-2.5 text-xs">
      <span className="mt-0.5 shrink-0 text-muted-foreground">{icon}</span>
      <span className="w-28 shrink-0 text-muted-foreground">{label}</span>
      <span className="font-medium text-foreground">{value}</span>
    </div>
  )
}
